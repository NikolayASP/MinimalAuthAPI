using MinimalAuthAPI.Models;

namespace MinimalAuthAPI.Services
{
    public interface IUserService
    {
        public User Get(UserLogin userLogin);
    }
}
