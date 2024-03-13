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
	public class ProductImageyRepository : Repository<ProductImage>, IProductImageRepository
	{

		private readonly ApplicationDbContext _db;
        public ProductImageyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(ProductImage obj)
		{
			_db.ProductImages.Update(obj);
		}
	}
}
