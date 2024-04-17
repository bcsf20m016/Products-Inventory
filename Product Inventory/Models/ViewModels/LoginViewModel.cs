using System;
using System.ComponentModel.DataAnnotations;

namespace Product_Inventory.Models.ViewModels
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
		public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }
}

