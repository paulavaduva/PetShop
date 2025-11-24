using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PetShop.Models;

namespace PetShop.Repositories.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(string firstName, string lastName, string phoneNumber, string email, string password);
        Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task<(bool succeeded, bool requires2FA, bool isLockedOut, string errorMessage)> LoginAsync(string email, string password, bool rememberMe);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
