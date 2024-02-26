using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.Models
{
	public class CarModel
	{
        [Key]
        public int Id { get; set; }
		[Required(ErrorMessage = "Ви не ввели Бренд Автомобіля!")]
		[DisplayName("Бренд Автомобіля")]
		[MaxLength(30)]
		public string Brand { get; set; }
		[Required(ErrorMessage = "Ви не ввели Марку Автомобіля!")]
		[DisplayName("Марка Автомобіля")]
		[MaxLength(30)]
		public string Model { get; set; }
		[Required(ErrorMessage = "Рік Випуску обов'язковий!")]
		[DisplayName("Рік Випуску")]
		public int? Year { get; set; }
		[Required(ErrorMessage = "Порядок відображення обов'язковий!")]
		[DisplayName("Порядок Відображення")]
		[Range(1, 100, ErrorMessage = "Значення повинно бути від 1 до 100!")]
		public int? DisplayOrder { get; set; }
	}
}
