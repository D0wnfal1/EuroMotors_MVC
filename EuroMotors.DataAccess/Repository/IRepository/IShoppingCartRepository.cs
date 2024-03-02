using EuroMotors.Models;
using EuroMotors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.DataAccess.Repository.IRepository
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		void Update(ShoppingCart obj);
	}
}
