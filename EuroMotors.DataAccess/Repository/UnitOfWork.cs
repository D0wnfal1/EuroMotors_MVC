using EuroMotors.DataAccess.Data;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository Category { get; private set; }
		public ICarModelRepository CarModel { get; private set; }
		public IProductRepository Product { get; private set; }
		private readonly ApplicationDbContext _db;
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Category = new CategoryRepository(_db);
			CarModel = new CarModelRepository(_db);
			Product = new ProductRepository(_db);
		}
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
