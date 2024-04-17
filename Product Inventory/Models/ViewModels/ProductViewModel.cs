using System;
using System.ComponentModel.DataAnnotations;

namespace Product_Inventory.Models.ViewModels
{
	public class ProductViewModel
	{
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Enter product name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter quantity of product")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid Number.")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter Valid Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Kindly provide short product description")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter discount")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid Number.")]
        [Range(0, 100, ErrorMessage = "Enter Valid Discount")]
        public int Discount { get; set; } = 0;

        [Required(ErrorMessage = "Price is mandatory")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid Number.")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter Valid Price")]
        public int Price { get; set; }
        
        public string? Image { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}

