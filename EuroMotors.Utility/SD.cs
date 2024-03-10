using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.Utility
{
	public static class SD
	{
		public const string Role_Customer = "Замовник";
		public const string Role_Admin = "Адмін";

		public const string StatusPending = "В очікуванні";
		public const string StatusApproved = "Затверджено";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Обробка";
		public const string StatusCancelled = "Скасовано";
		public const string StatusRefunded = "Повернено";

		public const string PaymentStatusPending = "В очікуванні";
		public const string PaymentStatusApproved = "Затверджено";
		public const string PaymentStatusRejected = "Скасовано";
	}
}
