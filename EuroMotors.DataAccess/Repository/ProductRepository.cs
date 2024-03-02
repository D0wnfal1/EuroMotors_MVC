using EuroMotors.DataAccess.Data;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{

		private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Product obj)
		{
			var objFromDb = _db.Products.FirstOrDefault(u=>u.Id==obj.Id);
		    if (objFromDb != null)
			{
				objFromDb.Title = obj.Title;
				objFromDb.Desctiption = obj.Desctiption;
				objFromDb.VendorCode = obj.VendorCode;
				objFromDb.Brand = obj.Brand;
				objFromDb.ListPrice = obj.ListPrice;
				objFromDb.Price = obj.Price;
				objFromDb.CategoryId = obj.CategoryId;
				objFromDb.CarModelId = obj.CarModelId;
				if (obj.ImageUrl != null)
				{
					objFromDb.ImageUrl = obj.ImageUrl;
				}
			}
		}
	}
}
