using EuroMotors.DataAccess.Data;
using EuroMotors.DataAccess.Repository.IRepository;
using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{

		private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(ShoppingCart obj)
		{
			_db.ShoppingCarts.Update(obj);
		}
	}
}
