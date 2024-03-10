using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroMotors.Utility
{
	public static class SD
	{
		public const string Role_Customer = "Customer";
		public const string Role_Admin = "Admin";

		public const string StatusPending = "В очікуванні";
		public const string StatusApproved = "Затверджено";
		public const string StatusInProcess = "Обробка";
		public const string StatusShipped = "Відправлено";
		public const string StatusCancelled = "Скасовано";

		public const string PaymentStatusPending = "В очікуванні";
		public const string PaymentStatusApproved = "Затверджено";
		public const string PaymentStatusRejected = "Скасовано";
	}
}
