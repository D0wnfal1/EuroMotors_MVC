using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EuroMotorsWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details(int orderId)
		{
			OrderVM orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return View(orderVM);
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]

		public IActionResult UpdateOrderDetail(int orderId)
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.City = OrderVM.OrderHeader.City;
			orderHeaderFromDb.Warehouse = OrderVM.OrderHeader.Warehouse;
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
				return View(OrderVM);
			}
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["Success"] = "Деталі замовлення успішно оновлено.";


			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["Success"] = "Деталі замовлення успішно оновлено.";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Замовлення успішно відправлено.";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult CancelOrder()
		{

			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);



			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

			_unitOfWork.Save();
			TempData["Success"] = "Замовлення успішно скасовано.";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}
		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			if (User.IsInRole(SD.Role_Admin))
			{
				objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				objOrderHeaders = _unitOfWork.OrderHeader
					.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			}
			switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
					break;
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
					break;
				default:
					break;

			}
			return Json(new { data = objOrderHeaders });
		}
		#endregion
	}
}
