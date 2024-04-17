using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;
using Product_Inventory.Repositories.IRepos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Product_Inventory.Controllers
{
    [Authorize(Roles = "user")]
    public class ProductController : Controller
    {
        private readonly IProductRepo productRepo;
        private readonly IAccountRepo accountRepo;
        private readonly IMapper _mapper;

        public ProductController(IProductRepo productRepo, IAccountRepo accountRepo, IMapper autoMapper)
        {
            this.productRepo = productRepo;
            this.accountRepo = accountRepo;
            _mapper = autoMapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber=0, int pageSize=5)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                var products = productRepo.getUserProducts();

                // Ensure pageNumber is at least 1
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }

                return View(await PaginatedList<ProductViewModel>.CreateAsync(products, pageNumber, pageSize));
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                return View();
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel product)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                if (ModelState.IsValid)
                {
                    var newProd = _mapper.Map<Product>(product);

                    //Storing Image
                    if (product.ImageFile is not null)
                        newProd.Image = productRepo.saveProductImage(product.ImageFile);
                    else
                        newProd.Image = "ProductImages/product.png";

                    newProd.Name = newProd.Name.Trim();
                    newProd.Description = newProd.Description.Trim();

                    var result = productRepo.addProduct(newProd);
                    if (!string.IsNullOrEmpty(result))
                    {
                        ModelState.AddModelError(string.Empty, result);
                    }
                    else
                    {
                        var toastrData = new string[] { "success", "Product Added", "Product has been added successfully." };
                        TempData["ToastrData"] = toastrData;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Can not add this product");
                }
                return View();
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int prodId)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                var result = productRepo.deleteProduct(prodId);
                if (result)
                {
                    var toastrData = new string[] { "success", "Product Deleted", "Product has been deleted successfully." };
                    TempData["ToastrData"] = toastrData;
                }
                else
                {
                    var toastrData = new string[] { "error", "Error", "The product does not exist." };
                    TempData["ToastrData"] = toastrData;
                }
                return RedirectToAction("Index");
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int prodId)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                var product = productRepo.findProduct(prodId);
                if (product is null)
                {
                    var toastrData = new string[] { "error", "Error", "The product does not exist." };
                    TempData["ToastrData"] = toastrData;
                    return RedirectToAction("Index");
                }
                var result = _mapper.Map<ProductViewModel>(product);
                return View(result);
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel product)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                if (ModelState.IsValid)
                {
                    var prod = productRepo.findProduct(product.ProductId);
                    if (prod is null)
                    {
                        ModelState.AddModelError(string.Empty, "Product doesn't exist");
                        return RedirectToAction("Index");
                    }

                    //Handling Image
                    if (product.ImageFile is not null)
                    {
                        productRepo.deleteProductImage(product.Image!);
                        product.Image = productRepo.saveProductImage(product.ImageFile);
                    }
                    else
                    {
                        product.Image = prod.Image;
                    }

                    product.Name = product.Name.Trim();
                    product.Description = prod.Description.Trim();

                    var result = productRepo.editProduct(product);
                    if (!string.IsNullOrEmpty(result))
                    {
                        ModelState.AddModelError(string.Empty, result);
                    }
                    else
                    {
                        var toastrData = new string[] { "success", "Product Updated", "The product has been updated successfully." };
                        TempData["ToastrData"] = toastrData;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Can not update this product");
                }
                return View(product);
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            try
            {
                //Checking if the user account is locked
                if (await accountRepo.isUserLocked())
                {
                    return RedirectToAction("Logout", "Account");
                }

                ViewBag.searchTerm = search;
                var products = productRepo.searchProducts(search);
                if (products is not null)
                {
                    var result = products.Select(_mapper.Map<ProductViewModel>).ToList();
                    return View(result);
                }
                return View(new List<ProductViewModel>());
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}

