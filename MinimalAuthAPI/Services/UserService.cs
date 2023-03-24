using MinimalAuthAPI.Models;
using MinimalAuthAPI.Repositories;

namespace MinimalAuthAPI.Services
{
    public class UserService : IUserService
    {
        public User Get(UserLogin userLogin)
        {
            User user = UserRepository.Users
                .FirstOrDefault(u => u.Username
                    .Equals(userLogin.UserName, StringComparison.OrdinalIgnoreCase) && u.Password
                    .Equals(userLogin.Password));

            return user;
        }
    }
}
