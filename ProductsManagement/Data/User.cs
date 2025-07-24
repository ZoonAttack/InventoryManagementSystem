using Microsoft.AspNetCore.Identity;

namespace ProductsManagement.Data
{
    public class User: IdentityUser
    {
        public ICollection<Order> Orders { get; set; }

    }
}
