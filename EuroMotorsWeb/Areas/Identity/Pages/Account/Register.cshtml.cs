// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using EuroMotors.Models;
using EuroMotors.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace EuroMotorsWeb.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserStore<IdentityUser> _userStore;
		private readonly IUserEmailStore<IdentityUser> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterModel(
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IUserStore<IdentityUser> userStore,
			SignInManager<IdentityUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Електронна пошта")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "Поле {0} повинно містити мінімум {2} та максимум {1} символів.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Пароль")]
			public string Password { get; set; }

			[Required]
			[DataType(DataType.Password)]
			[Display(Name = "Підтвердіть пароль")]
			[Compare("Password", ErrorMessage = "Пароль та підтвердження пароля не співпадають.")]
			public string ConfirmPassword { get; set; }

			public string? Role { get; set; }

			[ValidateNever]
			public IEnumerable<SelectListItem> RoleList { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "Поле {0} повинно містити мінімум {2} та максимум {1} символів.", MinimumLength = 1)]
			[Display(Name = "Ім'я")]
			public string Name { get; set; }

			public string? StreetAdress { get; set; }
			public string? City { get; set; }
			public string? PostalCode { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "Поле {0} повинно містити мінімум {2} та максимум {1} символів.", MinimumLength = 9)]
			[DataType(DataType.PhoneNumber)]
			[Display(Name = "Номер телефону")]
			public string PhoneNumber { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
			}

			Input = new()
			{
				RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				})
			};

			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = CreateUser();

				await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
				user.StreetAdress = Input.StreetAdress;
				user.Name = Input.Name;
				user.City = Input.City;
				user.PostalCode = Input.PostalCode;
				user.PhoneNumber = Input.PhoneNumber;
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("Користувач створив новий обліковий запис з паролем.");

					if (!String.IsNullOrEmpty(Input.Role))
					{
						await _userManager.AddToRoleAsync(user, Input.Role);
					}
					else
					{
						await _userManager.AddToRoleAsync(user, SD.Role_Customer);
					}

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Підтвердіть вашу електронну пошту",
						$"Будь ласка, підтвердіть свій обліковий запис, натиснувши <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>тут</a>.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// Якщо дійшли до цього місця, щось пішло не так, перевірте форму
			return Page();
		}

		private ApplicationUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<ApplicationUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Неможливо створити екземпляр '{nameof(IdentityUser)}'. " +
					$"Переконайтеся, що '{nameof(IdentityUser)}' не є абстрактним класом та має безпараметричний конструктор, або, як альтернативу, " +
					$"перевизначте сторінку реєстрації в /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<IdentityUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("Модульному інтерфейсу користувача необхідна підтримка електронної пошти.");
			}
			return (IUserEmailStore<IdentityUser>)_userStore;
		}
	}
}
