using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EuroMotorsWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_configuration = configuration;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (claim != null)
			{
				HttpContext.Session.SetInt32(SD.SessionCart,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
			}
			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel");
			return View(productList);
		}

		public IActionResult Details(int productId)
		{
			ShoppingCart cart = new()
			{
				Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,CarModel"),
				Count = 1,
				ProductId = productId
			};

			return View(cart);
		}
		[HttpPost]
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCart.ApplicationUserId = userId;
			ShoppingCart cartFromb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
			u.ProductId == shoppingCart.ProductId);

			if (cartFromb != null)
			{
				cartFromb.Count += shoppingCart.Count;
				_unitOfWork.Save();
			}
			else
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Save();
				HttpContext.Session.SetInt32(SD.SessionCart,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
			}
			TempData["success"] = "����� ������ ���������!";

			return RedirectToAction(nameof(Index));
		}
		[HttpPost]
		public IActionResult Redirect()
		{
			var request_dictionary = Request.Form.Keys.ToDictionary(key => key, key => Request.Form[key]);
			byte[] request_data = Convert.FromBase64String(request_dictionary["data"]);
			string decodedString = Encoding.UTF8.GetString(request_data);
			var request_data_dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);
			var mySignature = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(_configuration["LiqPaySettings:PrivateKey"] + request_dictionary["data"] + _configuration["LiqPaySettings:PrivateKey"])));
			if (mySignature != request_dictionary["signature"])
			{
				TempData["error"] = "������ �� ���� ��������!";
				return RedirectToAction(nameof(Index));
			}
			if (request_data_dictionary["status"] == "sandbox" || request_data_dictionary["status"] == "success")
			{
				OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == Convert.ToInt32(request_data_dictionary["order_id"]), includeProperties: "ApplicationUser");
				_unitOfWork.OrderHeader.UpdateStatus(Convert.ToInt32(request_data_dictionary["order_id"]), SD.StatusApproved, SD.PaymentStatusApproved);
				TempData["success"] = "������ ������� ������!";
				_unitOfWork.Save();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = "������ �� ���� ��������!";
				return RedirectToAction(nameof(Index));
			}
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
