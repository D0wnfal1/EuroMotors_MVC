using Baroque.NovaPoshta.Client.Domain.Address;
using Baroque.NovaPoshta.Client.Services.Address;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EuroMotorsWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly AddressService _addressService;

        [BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork, AddressService addressService)
		{
			_unitOfWork = unitOfWork;
			_addressService = addressService;
		}

		private string GetUserId()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";

			if (claimsIdentity.IsAuthenticated)
			{
				userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			else
			{
				userId = HttpContext.Session.GetString("SessionId");
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
			}
			return userId;
		}
		public IActionResult Index()
		{
			var userId = GetUserId();
			var claimsIdentity = (ClaimsIdentity)User.Identity;

			ShoppingCartVM ShoppingCartVM = new ShoppingCartVM();
			if (claimsIdentity != null && claimsIdentity.IsAuthenticated)
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
			}
			else
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product");
			}
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
			ShoppingCartVM.OrderHeader = new OrderHeader();

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}
		public async Task<IActionResult> Summary(string cityName = "")
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";

			if (claimsIdentity.IsAuthenticated)
			{
				userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			}
			else
			{
				userId = HttpContext.Session.GetString("SessionId");
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
			}

			ShoppingCartVM = new ShoppingCartVM()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId || u.GuestId == userId, includeProperties: "Product"),
				OrderHeader = new OrderHeader(),
			};

			if (claimsIdentity.IsAuthenticated)
			{
				ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
				ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
				ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			}

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var userId = GetUserId();
			var claimsIdentity = (ClaimsIdentity)User.Identity;

			if (claimsIdentity.IsAuthenticated)
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
				ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
				ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			}
			else
			{
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product");
				ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
				ShoppingCartVM.OrderHeader.GuestId = userId;
			}

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}
			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}

		public IActionResult OrderConfirmation(int id)
		{
			var userId = GetUserId();
			var claimsIdentity = (ClaimsIdentity)User.Identity;

			ShoppingCartVM = new ShoppingCartVM
			{
				OrderHeader = new OrderHeader()
			};

			if (claimsIdentity.IsAuthenticated)
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
			}
			else
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product");
			}

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.GuestId = userId;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			ShoppingCartVM.OrderHeader.Id = id;
			OrderHeader orderHeader;

			if (claimsIdentity.IsAuthenticated)
			{
				orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
			}
			else
			{
				orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
			}

			var developmentDomain = "https://localhost:7159/";
			var productionDomain = "https://euromotors-d0wnfal1.azurewebsites.net/";
            var paymentRequest = new LiqPayRequest
            {
                PublicKey = Environment.GetEnvironmentVariable("LIQPAY_PUBLIC_KEY"),
                Amount = ShoppingCartVM.OrderHeader.OrderTotal,
                Currency = "UAH",
                OrderId = ShoppingCartVM.OrderHeader.Id.ToString(),
                Action = LiqPayRequestAction.Pay,
                Language = LiqPayRequestLanguage.UK,
                Description = "Оплата замовлення #" + ShoppingCartVM.OrderHeader.Id,
                Goods = ShoppingCartVM.ShoppingCartList.Select(cart => new LiqPayRequestGoods
                {
                    Amount = cart.Price * 100,
                    Count = cart.Count,
                    Unit = "шт.",
                    Name = cart.Product.Title
                }).ToList(),
                ResultUrl = developmentDomain + $"Customer/Home/Redirect",
            };
            var json_string = JsonConvert.SerializeObject(paymentRequest);
            var data_hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(json_string));
			var signature_hash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("LIQPAY_PRIVATE_KEY") + data_hash + Environment.GetEnvironmentVariable("LIQPAY_PRIVATE_KEY"))));
			ShoppingCartVM.OrderHeader.Data = data_hash;

            ShoppingCartVM.OrderHeader.Signature = signature_hash;
            _unitOfWork.OrderHeader.UpdateLiqPayPaymentID(id, ShoppingCartVM.OrderHeader.Signature, ShoppingCartVM.OrderHeader.Data);
            _unitOfWork.Save();

            List<ShoppingCart> shoppingCarts;
			if (claimsIdentity.IsAuthenticated)
			{
				shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			}
			else
			{
				shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == orderHeader.GuestId).ToList();
			}
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();
			return View(ShoppingCartVM);
		}

		[HttpGet]
		public IActionResult SearchCities(string query)
		{
			var citiesResponse = _addressService.GetCities(new CitiesGetRequest { FindByString = query });
			var cities = citiesResponse.Data.Select(city => city.Description).ToList();

			return Json(cities);
		}
		[HttpGet]
		public async Task<IActionResult> GetWarehouses(string city, string query)
		{
			var warehousesResponse = _addressService.GetWarehouses(city);
			var warehouses = warehousesResponse.Data.Select(warehouse => warehouse.Description).Where(warehouse => warehouse.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
			return Json(warehouses);
		}

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
