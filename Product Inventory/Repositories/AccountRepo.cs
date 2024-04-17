using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Inventory.Data;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;

namespace Product_Inventory.Repositories.IRepos
{
	public class AccountRepo : IAccountRepo
	{
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountRepo(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Login(LoginViewModel model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists is not null)
            {
                var isPasswordCorrect = await userManager.CheckPasswordAsync(userExists, model.Password);
                if (isPasswordCorrect)
                {
                    var signInResult = await signInManager.PasswordSignInAsync(userExists, model.Password, model.RememberMe, false);
                    if (signInResult.Succeeded)
                    {
                        if (isAdmin())
                            return "admin";
                        else
                            return "user";
                    }
                    else
                    {
                        if(signInResult.IsLockedOut)
                            return "Your account is locked by admin";

                        return "Invalid login attempt";
                    }
                }
                else
                {
                    return "Invalid Password";
                }
            }
            else
            {
                return "This account doesn't exist";
            }
        }

        public bool isAdmin()
        {
            return httpContextAccessor.HttpContext.User.IsInRole("admin");
        }

        public async Task<IdentityResult> Register(RegisterViewModel model)
        {
            var newUser = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(newUser, model.Password);

            return result;
        }

        public async Task<IdentityResult> assignUserRole(string email)
        {
            var userForRole = await userManager.FindByEmailAsync(email);
            var result = await userManager.AddToRoleAsync(userForRole!, "user");
            return result;
        }

        public async Task logOutUser()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<bool> doesAccountExist(string email)
        {
            var result = await userManager.FindByEmailAsync(email);
            if(result is not null)
            {
                return true;
            }
            return false;
        }

        public List<AppUser> getAllUsers()
        {
            return userManager.Users
            .Include(u => u.Products)
            .ToList();
        }

        public async Task<bool> deleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user is not null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return true;
                return false;
            }
            return false;
        }

        public async Task<bool> lockUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user is not null)
            {
                var result = await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddDays(30));
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<bool> unlockUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var result = await userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                    return true;
                return false;
            }
            return false;
        }

        public async Task<bool> isUserLocked()
        {
            var userId = getUserId();

            var user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                if (user.LockoutEnd is not null)
                    return true;
                return false;
            }
            return true;
        }

        public bool isUserSignedIn()
        {
            return signInManager.IsSignedIn(httpContextAccessor.HttpContext.User);
        }

        public string getUserId()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
            return userId!;
        }
    }
}

