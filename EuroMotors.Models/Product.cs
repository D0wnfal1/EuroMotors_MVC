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
		[Required]
		public string Title { get; set; }
		public string Desctiption { get; set; }
		[Required]
		public string VendorCode { get; set; }
		[Required]
		public string Brand { get; set;}
		[Required]
		[Display(Name = "Ціна за прейскурантом")]
		public double ListPrice { get; set; }
		[Required]
		[Display(Name = "Ціна")]
		public double Price { get; set; }

	}
}
