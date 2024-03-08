using Azure;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace EuroMotorsWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly LiqPayClient _liqPayClient;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork, IOptions<LiqPaySettings> liqPaySettings)
		{
			_unitOfWork = unitOfWork;
			_liqPayClient = new LiqPayClient(liqPaySettings.Value.PublicKey, liqPaySettings.Value.PrivateKey);
		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}
		[HttpPost]
		[ActionName("Summary")]
		public async void SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();
			var domain = "https://localhost:7159/";
			var paymentRequest = new LiqPayRequest
			{
				Amount = ShoppingCartVM.OrderHeader.OrderTotal,
				Currency = "UAH",
				IsSandbox = true,
				OrderId = ShoppingCartVM.OrderHeader.Id.ToString(),
				Action = LiqPayRequestAction.Pay,
				Language = LiqPayRequestLanguage.UK,
				Description = "Оплата заказа #" + ShoppingCartVM.OrderHeader.Id,
				Goods = ShoppingCartVM.ShoppingCartList.Select(cart => new LiqPayRequestGoods
				{
					Amount = cart.Price * 100,
					Count = cart.Count,
					Unit = "шт.",
					Name = cart.Product.Title
				}).ToList(),
				ResultUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				ServerUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
			};

			string paymentForm = _liqPayClient.CNBForm(paymentRequest);
			Response.ContentType = "text/html";
			await Response.WriteAsync(paymentForm);


			_unitOfWork.OrderHeader.UpdateStatus(ShoppingCartVM.OrderHeader.Id, SD.StatusPending, SD.PaymentStatusPending);
			_unitOfWork.Save();
		}
		public IActionResult OrderConfirmation(int id)
		{
			//string paymentStatus = DeterminePaymentStatus(response.Status);

			//_unitOfWork.OrderHeader.UpdateStatus(response.Id, paymentStatus, paymentStatus);
			//_unitOfWork.Save();

			return View(id);
		}
		//private string DeterminePaymentStatus(LiqPayResponseStatus responseStatus)
		//{
		//	switch (responseStatus)
		//	{
		//		case LiqPayResponseStatus.Success:
		//			return SD.PaymentStatusApproved;
		//		case LiqPayResponseStatus.Failure:
		//			return SD.PaymentStatusRejected;
		//		case LiqPayResponseStatus.Error:
		//			return SD.PaymentStatusRejected;
		//		default:
		//			return SD.PaymentStatusPending;
		//	}
		//}

		public IActionResult Plus(int cardId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Minus(int cardId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
			if (cartFromDb.Count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(cartFromDb);
			}
			else
			{
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDb);

			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Remove(int cardId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId);
			_unitOfWork.ShoppingCart.Remove(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
	}
}
