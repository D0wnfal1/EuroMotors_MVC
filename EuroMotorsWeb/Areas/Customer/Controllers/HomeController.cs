using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EuroMotorsWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel");
            return View(productList);
        }

		public IActionResult Details(int productId)
		{
			Product product = _unitOfWork.Product.Get(u=>u.Id== productId, includeProperties: "Category,CarModel");
			return View(product);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
