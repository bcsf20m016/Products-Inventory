using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;

namespace Product_Inventory.Repositories.IRepos
{
	public interface IAccountRepo
	{
		public List<AppUser> getAllUsers();
		public Task<string> Login(LoginViewModel model);
		public bool isAdmin();
		public Task<IdentityResult> Register(RegisterViewModel model);
        public Task<IdentityResult> assignUserRole(string email);
        public Task logOutUser();
		public Task<bool> doesAccountExist(string email);
        public Task<bool> deleteUser(string userId);
        public Task<bool> lockUser(string userId);
        public Task<bool> unlockUser(string userId);
        public Task<bool> isUserLocked();
        public bool isUserSignedIn();
        public string getUserId();
    }
}

