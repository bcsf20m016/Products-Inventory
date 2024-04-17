using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Inventory.Models.ViewModels;
using Product_Inventory.Repositories.IRepos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Product_Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepo accountRepo;

        public AccountController(IAccountRepo accountRepo)
        {
            this.accountRepo = accountRepo;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var users = accountRepo.getAllUsers();
                return View(users);
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                if (accountRepo.isUserSignedIn())
                {
                    if (accountRepo.isAdmin())
                    {
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Product");
                    }
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginResult = await accountRepo.Login(model);
                    if (loginResult == "admin")
                    {
                        return RedirectToAction("Index", "Account");
                    }
                    else if (loginResult == "user")
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, loginResult);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
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
        public IActionResult Register()
        {
            try
            {
                if (accountRepo.isUserSignedIn())
                {
                    if (accountRepo.isAdmin())
                    {
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Product");
                    }
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isAccountNew = await accountRepo.doesAccountExist(model.Email);
                    if (isAccountNew)
                    {
                        ModelState.AddModelError(string.Empty, "Account with this email already exist");
                        return View(model);
                    }

                    var result = await accountRepo.Register(model);
                    if (result.Succeeded)
                    {
                        var assignRoleResult = await accountRepo.assignUserRole(model.Email);
                        if (!assignRoleResult.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description!);
                            }
                        }
                        else
                        {
                            var toastrData = new string[] { "success", "Account Created", "Your account has been created successfully." };
                            TempData["ToastrData"] = toastrData;
                            return RedirectToAction("Login");
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description!);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Attempt Failed for Registration");
                }
                return View(model);
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await accountRepo.logOutUser();
                return RedirectToAction("Login");
            }
            catch
            {
                var toastrData = new string[] { "error", "Error", "There is an error while processing your request." };
                TempData["ToastrData"] = toastrData;
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                var result = await accountRepo.deleteUser(userId);
                if (result)
                {
                    var toastrData = new string[] { "success", "Account Deleted", "The account has been deleted successfully." };
                    TempData["ToastrData"] = toastrData;
                }
                else
                {
                    var toastrData = new string[] { "error", "Error", "This account can not be deleted." };
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

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> LockUser(string userId)
        {
            try
            {
                var result = await accountRepo.lockUser(userId);
                if (result)
                {
                    var toastrData = new string[] { "success", "Account Locked", "The account has been locked successfully." };
                    TempData["ToastrData"] = toastrData;
                }
                else
                {
                    var toastrData = new string[] { "error", "Error", "This account can not be locked." };
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

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            try
            {
                var result = await accountRepo.unlockUser(userId);
                if (result)
                {
                    var toastrData = new string[] { "success", "Account Unlocked", "The account has been unlocked successfully." };
                    TempData["ToastrData"] = toastrData;
                }
                else
                {
                    var toastrData = new string[] { "error", "Error", "This account can not be unlocked." };
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
    }
}

