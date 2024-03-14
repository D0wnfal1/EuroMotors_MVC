using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovaPoshtaApi;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EuroMotorsWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	//[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
        private readonly NovaPoshtaClient _novaPoshtaClient;
        [BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork, IConfiguration configuration, NovaPoshtaClient novaPoshtaClient)
		{
			_unitOfWork = unitOfWork;
			_configuration = configuration;
            _novaPoshtaClient = novaPoshtaClient;
        }
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";
			if (claimsIdentity.IsAuthenticated)
			{
				userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				ShoppingCartVM = new()
				{
					ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
					OrderHeader = new()
				};
			}
			else
			{
				userId = HttpContext.Session.GetString("SessionId");
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
				ShoppingCartVM = new()
				{
					ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product"),
					OrderHeader = new()
				};
			}
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
				cart.Price = cart.Product.Price;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";
			if (claimsIdentity.IsAuthenticated)
			{
                var citiesResponse = _novaPoshtaClient.Address.GetCities().GetAwaiter().GetResult();
                var cities = citiesResponse?.Select(c => c.Description);
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				ShoppingCartVM = new()
				{
					ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
					OrderHeader = new(),
                    Cities = cities
                };
				ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

				ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
				ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
				//ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
				//ShoppingCartVM.OrderHeader.Warehouse = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
				//ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			}
			else
			{
                var citiesResponse = _novaPoshtaClient.Address.GetCities().GetAwaiter().GetResult();
                var cities = citiesResponse?.Select(c => c.Description);
                userId = HttpContext.Session.GetString("SessionId");
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
				ShoppingCartVM = new()
				{
					ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product"),
					OrderHeader = new(),
                    Cities = cities
                };
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
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";
			if (claimsIdentity.IsAuthenticated)
			{
				userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
				ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
				ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			}
			else
			{
				userId = HttpContext.Session.GetString("SessionId");
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
				OrderDetail orderDetail = new()
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

			ShoppingCartVM = new ShoppingCartVM
			{
				OrderHeader = new OrderHeader()
			};

			if (claimsIdentity.IsAuthenticated)
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
				ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
				ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			}
			else
			{
				ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.GuestId == userId, includeProperties: "Product");
				ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
				ShoppingCartVM.OrderHeader.GuestId = userId;
			}

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
			var domain = "https://localhost:7159/";
			var paymentRequest = new LiqPayRequest
			{
				PublicKey = _configuration["LiqPaySettings:PublicKey"],
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
				ResultUrl = domain + $"Customer/Home/Redirect",
			};

			var json_string = JsonConvert.SerializeObject(paymentRequest);
			var data_hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(json_string));
			var signature_hash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(_configuration["LiqPaySettings:PrivateKey"] + data_hash + _configuration["LiqPaySettings:PrivateKey"])));
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
        public async Task<IActionResult> GetWarehousesByCityName(string cityName)
        {
            var warehousesResponse = await _novaPoshtaClient.Address.GetWarehousesByCityName(cityName);
            var warehouses = warehousesResponse?.Select(w => w.Description);

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
