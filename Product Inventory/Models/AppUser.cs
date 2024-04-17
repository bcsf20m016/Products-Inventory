using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Product_Inventory.Models
{
	public class AppUser : IdentityUser
	{
        [StringLength(100)]
		public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }

        //Navigation Property
        public virtual List<Product> Products { get; set; }
    }
}

