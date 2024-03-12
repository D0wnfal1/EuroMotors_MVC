using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Signature { get; set; }
        public string? Data { get; set; }
		[Required(ErrorMessage = "Поле Номер Телефону є обов'язковим.")]
		public string? PhoneNumber { get; set; }
		[Required(ErrorMessage = "Поле Адрес Відправки є обов'язковим.")]
		public string? StreetAddress { get; set; }
		[Required(ErrorMessage = "Поле Місто є обов'язковим.")]
		public string? City { get; set; }
		[Required(ErrorMessage = "Поле Поштовий Індекс є обов'язковим.")]
		public string? PostalCode { get; set; }
		[Required(ErrorMessage = "Поле Ім'я є обов'язковим.")]
		public string Name { get; set; }
    }
}
