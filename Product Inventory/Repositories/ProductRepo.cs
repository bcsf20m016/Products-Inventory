using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Product_Inventory.Data;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;
using Product_Inventory.Repositories.IRepos;

namespace Product_Inventory.Repositories
{
    public class ProductRepo : IProductRepo
	{
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AppUser> userManager;

        public ProductRepo(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
		{
            _db = db;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public string addProduct(Product product)
        {
            var userId = getUserId();

            //Checking duplicate product
            if(isProductDuplicate(product.ProductId, product.Name))
            {
                return "Product with same name already exists";
            }
            
            //Storing product into DB
            var user = _db.Users
            .Include(u => u.Products)
            .FirstOrDefault(u => u.Id == userId);

            if (user!.Products is null)
                user!.Products = new List<Product>();

            user!.Products.Add(product);
            _db.SaveChanges();

            return string.Empty;
        }

        public bool deleteProduct(int prodId)
        {
            var delProd = findProduct(prodId);
            if(delProd is not null)
            {
                if(delProd.Image != "ProductImages/product.png") //We don't want to delete our default product image
                    deleteProductImage(delProd.Image);

                _db.Products.Remove(delProd);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public IQueryable<ProductViewModel> getUserProducts()
        {
            var userId = getUserId();

            var products = _db.Products.
                            Where(prod => prod.User.Id == userId).
                            Select(prod => new ProductViewModel
                            {
                                ProductId = prod.ProductId,
                                Name = prod.Name,
                                Price = prod.Price,
                                Discount = prod.Discount,
                                Quantity = prod.Quantity,
                                Image = prod.Image,
                                Description = prod.Description
                            });

            return products;
        }

        public bool isProductDuplicate(int prodId, string prodName)
        {
            var userId = getUserId();

            var user = _db.Users
            .Include(u => u.Products)
            .FirstOrDefault(u => u.Id == userId);

            var products = user!.Products;

            if(products is not null)
            {
                var dupProd = products.FirstOrDefault(prod => (prod.ProductId != prodId) &&  string.Equals(prod.Name, prodName, StringComparison.OrdinalIgnoreCase));
                return dupProd is null ? false : true;
            }
            return false;
        }

        public string saveProductImage(IFormFile image)
        {
            var filePath = "wwwroot/ProductImages/";
            var filename = Guid.NewGuid().ToString() + "_" + image.FileName;
            var fullPath = Path.Combine(filePath, filename);

            using(var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                image.CopyTo(stream);
            }

            var imgURL = "ProductImages/" + filename;
            return imgURL;
        }

        public Product? findProduct(int prodId)
        {
            var userId = getUserId();

            var user = _db.Users
            .Include(u => u.Products)
            .FirstOrDefault(u => u.Id == userId);

            if(user!.Products is not null)
            {
                var product = user!.Products.FirstOrDefault(prod => prod.ProductId == prodId);
                return product;
            }

            return null;
        }

        public void deleteProductImage(string imagePath)
        {
            string fullPath = "wwwroot/" + imagePath;
            if(File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public string editProduct(ProductViewModel product)
        {
            var userId = getUserId();

            //Checking duplicate product
            if (isProductDuplicate(product.ProductId, product.Name))
            {
                return "Product with same name already exists";
            }

            //Updating product in DB
            var user = _db.Users
            .Include(u => u.Products)
            .FirstOrDefault(u => u.Id == userId);

            var prod = user!.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            prod!.Description = product.Description;
            prod.Discount = product.Discount;
            prod.Image = product.Image!;
            prod.Name = product.Name;
            prod.Price = product.Price;
            prod.Quantity = product.Quantity;

            _db.SaveChanges();

            return string.Empty;
        }

        public List<Product> searchProducts(string searchTerm)
        {
            var userId = getUserId();

            var user = _db.Users
            .Include(u => u.Products)
            .FirstOrDefault(u => u.Id == userId);

            if(user!.Products is not null)
            {
                var products = user!.Products.Where(prod => prod.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                return products;
            }

            return null;
        }

        public string getUserId()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
            return userId!;
        }

    }
}

