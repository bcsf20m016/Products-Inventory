using System;
using System.Security.Claims;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;

namespace Product_Inventory.Repositories.IRepos
{
	public interface IProductRepo
	{
		public IQueryable<ProductViewModel> getUserProducts();
        public string addProduct(Product product);
        public bool deleteProduct(int prodId);
        public string editProduct(ProductViewModel product);
        public bool isProductDuplicate(int prodId, string prodName);
        public string saveProductImage(IFormFile image);
        public Product? findProduct(int prodId);
        public void deleteProductImage(string imagePath);
        public List<Product> searchProducts(string searchTerm);
        public string getUserId();

    }
}

