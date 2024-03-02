using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
		ICarModelRepository CarModel { get; }
		IProductRepository Product { get; }
		IShoppingCartRepository ShoppingCart { get; }
		IApplicationUserRepository ApplicationUser { get; }
		public void Save();
	}
}
