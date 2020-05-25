using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Web.Admin.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
    }

    public class ChangeAccountDetails
    {
        [Required(ErrorMessage = "Account name is required")]
        [Display(Name = "Account Name:")]
        [RegularExpression(@"^[A-Za-z0-9\s\[\]?%@*!&#:'().,\/_-]+$", ErrorMessage = "Alphabets and numbers only")]
        [MaxLength(200, ErrorMessage = "Max characters is 200")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Primary phone number is required")]
        [Display(Name = "Primary Phone Number:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter a valid phone number")]
        [MaxLength(12, ErrorMessage = "Enter a maximum of 12 numbers")]
        public string PrimaryPhone { get; set; }

        [Display(Name = "Secondary Phone Number:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter a valid phone number")]
        [MaxLength(12, ErrorMessage = "Enter a maximum of 12 numbers")]
        public string SecondaryPhone { get; set; }

        [Display(Name = "Mobile Money Number:")]
        [Required(ErrorMessage = "Mobile money number is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        public string MobileMoneyPhone { get; set; }

        [Display(Name = "Mobile Money Network:")]
        [Required(ErrorMessage = "Mobile money network is required")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%&*()\s?'"":;.,_+={}\[\]\\/-]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        public string MobileMoneyNetwork { get; set; }

        [Display(Name = "Location:")]
        [Required(ErrorMessage = "Location is required")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%&*()\s?'"":;.,_+={}\[\]\\/-]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        public string Location { get; set; }

        public string CourierCode { get; set; }
    }

    public class ChangeAccountName
    {
        [Required(ErrorMessage = "Account name is required")]
        [Display(Name = "Account Name:")]
        [RegularExpression(@"^[A-Za-z0-9\s\[\]?%@*!&#:'().,\/_-]+$", ErrorMessage = "Alphabets and numbers only")]
        [MaxLength(200, ErrorMessage = "Max characters is 200")]
        public string AccountName { get; set; }
    }

    public class ChangeAccountPhone
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone number:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter a valid phone number")]
        [MaxLength(12, ErrorMessage = "Max characters is 12")]
        public string Phone { get; set; }
    }

    public class VerifyPhone
    {
        [Required(ErrorMessage = "Code is required")]
        [Display(Name = "Code:")]
        [RegularExpression(@"^[A-Za-z0-9\s\[\]?%@*!&#:'().,\/_-]+$", ErrorMessage = "Alphabets and numbers only")]
        [MaxLength(10, ErrorMessage = "Max characters is 10")]
        public string Code { get; set; }
    }

    public class AccountViewModel
    {
        [Required(ErrorMessage = "Account name is required")]
        [Display(Name = "Account Name:")]
        [RegularExpression(@"^[A-Za-z0-9\s\[\]?%@*!&#:'().,\/_-]+$", ErrorMessage = "Alphabets and numbers only")]
        [MaxLength(200, ErrorMessage = "Max characters is 200")]
        public string AccountName { get; set; }

        [Display(Name = "Username:")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone number:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter a valid phone number")]
        [MaxLength(50, ErrorMessage = "Max characters is 50")]
        public string Phone { get; set; }
    }

    public class ChangeUsernameViewModel
    {
        [Display(Name = "New Username:")]
        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(50, ErrorMessage = "Max characters is 50")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter a valid email address")]
        public string Username { get; set; }
    }
}