using EuroMotors.DataAccess.Data;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotorsWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CarModelController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CarModelController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<CarModel> objCategoryList = _unitOfWork.CarModel.GetAll().ToList();
			return View(objCategoryList);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(CarModel obj)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.CarModel.Add(obj);
				_unitOfWork.Save();
				TempData["success"] = "Модель Авто успішно створена!";
				return RedirectToAction("Index");
			}
			return View();
		}
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			CarModel? carModelFromDb = _unitOfWork.CarModel.Get(u => u.Id == id);
			if (carModelFromDb == null)
			{
				return NotFound();
			}
			return View(carModelFromDb);
		}
		[HttpPost]
		public IActionResult Edit(CarModel obj)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.CarModel.Update(obj);
				_unitOfWork.Save();
				TempData["success"] = "Модель Авто успішно оновлена!";
				return RedirectToAction("Index");
			}
			return View();
		}
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			CarModel? carModelFromDb = _unitOfWork.CarModel.Get(u => u.Id == id);
			if (carModelFromDb == null)
			{
				return NotFound();
			}
			return View(carModelFromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			CarModel? obj = _unitOfWork.CarModel.Get(u => u.Id == id);
			if (obj == null)
			{
				return NotFound();
			}
			_unitOfWork.CarModel.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Модель Авто успішно видалена!";
			return RedirectToAction("Index");
		}
	}
}
