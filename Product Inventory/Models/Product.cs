using System;
using System.ComponentModel.DataAnnotations;

namespace Product_Inventory.Models
{
	public class Product
	{
		[Key]
		public int ProductId { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public string Description { get; set; }
		public int Discount { get; set; }
		public int Price { get; set; }
		public string Image { get; set; }

        //Navigation Property
        public virtual AppUser User { get; set; }
	}
}

