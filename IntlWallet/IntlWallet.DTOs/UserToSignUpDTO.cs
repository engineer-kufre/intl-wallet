using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntlWallet.DTOs
{
    public class UserToSignUpDTO
    {
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [MaxLength(14, ErrorMessage = "Phone number must not be 14 characters")]
        [RegularExpression(@"^\+\d{3}\d{9,10}$", ErrorMessage = "Must have country-code and must be 13, 14 chars long e.g. +2348050000000")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string MainCurrency { get; set; }
    }
}