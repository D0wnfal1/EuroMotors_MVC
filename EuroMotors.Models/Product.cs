using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Ви не ввели Назву Товару!")]
		[DisplayName("Назва Товару")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Ви не ввели Опис Товару!")]
		[DisplayName("Опис Товару")]
		public string Desctiption { get; set; }
		[Required(ErrorMessage = "Ви не ввели Артикул Товару!")]
		[DisplayName("Артикул Товару")]
		public string VendorCode { get; set; }
		[Required(ErrorMessage = "Ви не ввели Бренд Виробника Товару!")]
		[DisplayName("Бренд Виробника Товару")]
		[MaxLength(30)]
		public string Brand { get; set;}
		[Required(ErrorMessage = "Ви не ввели Ціну за прейскурантом!")]
		[DisplayName ( "Ціна за прейскурантом")]
		public double ListPrice { get; set; }
		[Required(ErrorMessage = "Ви не ввели Ціну!")]
		[DisplayName ("Ціна")]
		public double Price { get; set; }

	}
}
