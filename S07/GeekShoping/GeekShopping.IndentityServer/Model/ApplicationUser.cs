using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.IndentityServer.Model
{
    public class ApplicationUser :IdentityUser
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
