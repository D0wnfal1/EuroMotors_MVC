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
			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel,ProductImages");
			IEnumerable<Product> popularProducts = _unitOfWork.Product.GetAll().OrderBy(p => p.Id).Take(4);
			ViewBag.PopularProducts = popularProducts;
			return View(productList);
		}

		public IActionResult Details(int productId)
		{
			ShoppingCart cart = new()
			{
				Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,CarModel,ProductImages"),
				Count = 1,
				ProductId = productId
			};

			return View(cart);
		}
		[HttpPost]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			string userId = "";
			ShoppingCart cartFromDb;
			if (claimsIdentity.IsAuthenticated)
			{
				userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				shoppingCart.ApplicationUserId = userId;
				 cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
				u.ProductId == shoppingCart.ProductId);
			}
			else
			{
				userId = HttpContext.Session.GetString("SessionId");
				if (string.IsNullOrEmpty(userId))
				{
					userId = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("SessionId", userId);
				}
				shoppingCart.GuestId = userId;
				 cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.GuestId == userId &&
				u.ProductId == shoppingCart.ProductId);
			}

			if (cartFromDb != null)
			{
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}
			else
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
			}
			TempData["success"] = "Кошик успішно оновлений!";
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

        public IActionResult ProductsByCategory(string categoryName)
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel,ProductImages")
                .Where(p => p.Category != null && p.Category.Name == categoryName)
                .ToList();
            return View(products);
        }

        public IActionResult ProductsByBrand(string brandName)
		{
			var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel,ProductImages")
			.Where(p => p.CarModel != null && p.CarModel.Brand == brandName)
			.ToList();
			return View(products);
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
				TempData["error"] = "Оплата не була здійснена!";
				return RedirectToAction(nameof(Index));
			}
			if (request_data_dictionary["status"] == "sandbox" || request_data_dictionary["status"] == "success")
			{
				OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == Convert.ToInt32(request_data_dictionary["order_id"]), includeProperties: "ApplicationUser");
				_unitOfWork.OrderHeader.UpdateStatus(Convert.ToInt32(request_data_dictionary["order_id"]), SD.StatusApproved, SD.PaymentStatusApproved);
				TempData["success"] = "Оплата Пройшла Успішно!";
				_unitOfWork.Save();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = "Оплата не була здійснена!";
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
