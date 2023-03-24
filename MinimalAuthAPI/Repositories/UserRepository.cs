using MinimalAuthAPI.Models;

namespace MinimalAuthAPI.Repositories
{
    public class UserRepository
    {
        public static List<User> Users = new () 
        {
            new () 
            {
                Username = "pesho_admin",
                EmailAdress = "pesho.admin@email.com",
                Password = "MyPass_w0rd",
                GivenName = "Pesho",
                Surname = "Peshov",
                Role = "Administrator"
            },
            new ()
            {
                Username = "gosho_standard",
                EmailAdress = "gosho.standard@email.com",
                Password = "MyPass_w0rd",
                GivenName = "Gosho",
                Surname = "Peshov",
                Role = "Standard"
            },
        };
    }
}
