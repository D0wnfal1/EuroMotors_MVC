using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using EuroMotors.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EuroMotorsWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel").ToList();
			return View(objProductList);
		}
		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				CarModelList = _unitOfWork.CarModel
				.GetAll().Select(u => new SelectListItem
				{
					Text = $"{u.Brand} - {u.Model} - {u.Year}",
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};
			if (id == null || id == 0)
			{
				return View(productVM);
			}
			else
			{
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
				return View(productVM);
			}
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
		{
			if (ModelState.IsValid)
			{
				if (productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
					_unitOfWork.Save();
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
					_unitOfWork.Save();
				}
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null)
				{
					foreach (IFormFile file in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\products\product-"+ productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);
						if (!Directory.Exists(finalPath))
						{
							Directory.CreateDirectory(finalPath);
						}
						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}
						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId=productVM.Product.Id
						};
						if (productVM.Product.ProductImages == null)
						{
							productVM.Product.ProductImages = new List<ProductImage>();
						}
						productVM.Product.ProductImages.Add(productImage);
					}
					_unitOfWork.Product.Update(productVM.Product);
					_unitOfWork.Save();
				}
				TempData["success"] = "Товар успішно створен/оновлен!";
				return RedirectToAction("Index");
			}
			else
			{

				productVM.CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
				productVM.CarModelList = _unitOfWork.CarModel
				.GetAll().Select(u => new SelectListItem
				{
					Text = $"{u.Brand} - {u.Model}",
					Value = u.Id.ToString()
				});

				return View(productVM);
			}

		}

		public IActionResult DeleteImage(int imageId)
		{
			var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
			int productId = imageToBeDeleted.ProductId;
			if (imageToBeDeleted != null)
			{
				if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
				{
					var oldImagePath =
								   Path.Combine(_webHostEnvironment.WebRootPath,
								   imageToBeDeleted.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				_unitOfWork.ProductImage.Remove(imageToBeDeleted);
				_unitOfWork.Save();

				TempData["success"] = "Успішно видалено!";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
		}



		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,CarModel").ToList();
			return Json(new { data = objProductList });
		}



		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Помилка під час видалення" });
			}
			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}

				Directory.Delete(finalPath);
			}
			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Видалення виконано успішно" });
		}

		#endregion
	}
}
