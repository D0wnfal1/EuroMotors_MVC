using EuroMotors.DataAccess.Data;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotorsWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CarModelController : Controller
	{
		private readonly  ApplicationDbContext _db;
		public CarModelController(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			List<CarModel> objCategoryList = _db.CarModel.ToList();
			return View(objCategoryList);
		}
	}
}
