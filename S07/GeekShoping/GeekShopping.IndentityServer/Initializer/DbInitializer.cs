using Duende.IdentityModel;
using GeekShopping.IndentityServer.Configuration;
using GeekShopping.IndentityServer.Model;
using GeekShopping.IndentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace GeekShopping.IndentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DbInitializer(MySQLContext context, UserManager<ApplicationUser> user, RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        public void Initialize()
        {
            if (_role.FindByIdAsync(IdentityConfiguration.Admin).Result != null) return;

            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();

            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "LaertonAdmin",
                Email = "laerton240311@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (83) 9 90107 6105",
                FirstName = "Laerton",
                LastName = "Marques de Figueiredo",

            };

            _user.CreateAsync(admin, "Sil@495798").GetAwaiter().GetResult();
            _user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();
            IdentityResult adminClaims = _user.AddClaimsAsync(admin, new Claim[]
            {
                new (JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new (JwtClaimTypes.GivenName, admin.FirstName),
                new (JwtClaimTypes.FamilyName, admin.LastName),
                new (JwtClaimTypes.Role,IdentityConfiguration.Admin)

            }).Result;


            ApplicationUser client = new ApplicationUser()
            {
                UserName = "LaertonClient",
                Email = "laerton240311@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (83) 9 90107 6105",
                FirstName = "Laerton",
                LastName = "Marques de Figueiredo",

            };

            _user.CreateAsync(client, "Sil@495798").GetAwaiter().GetResult();
            _user.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();
            IdentityResult clientClaims = _user.AddClaimsAsync(client, new Claim[]
            {
                new (JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new (JwtClaimTypes.GivenName, client.FirstName),
                new (JwtClaimTypes.FamilyName, client.LastName),
                new (JwtClaimTypes.Role,IdentityConfiguration.Client)

            }).Result;

        }
    }
}
