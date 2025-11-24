using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterAsync(string firstName, string lastName, string phoneNumber, string email, string password)
        {
            var user = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                UserName = email
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }

        public async Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<(bool succeeded, bool requires2FA, bool isLockedOut, string errorMessage)> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return (true, false, false, null);

            if (result.RequiresTwoFactor)
                return (false, true, false, null);

            if (result.IsLockedOut)
                return (false, false, true, null);

            return (false, false, false, "Invalid login attempt.");
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
