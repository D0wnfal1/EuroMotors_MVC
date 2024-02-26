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
	public class CarModelRepository : Repository<CarModel>, ICarModelRepository
	{

		private readonly ApplicationDbContext _db;
        public CarModelRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(CarModel obj)
		{
			_db.CarModel.Update(obj);
		}
	}
}
