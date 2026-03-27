using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using DuendeIdentityServer.Pages.Login;
using GeekShopping.IndentityServer.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using static DuendeIdentityServer.Pages.Login.ViewModel;

namespace DuendeIdentityServer.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser>? _userManager;
    private readonly RoleManager<IdentityRole>? _roleManager;
    private readonly SignInManager<ApplicationUser>? _signInManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IEventService _events;
    private List<string> _roles;
    [BindProperty]
    public InputModel Input { get; set; } = default!;
    public List<string> Roles => _roles;
    public Index(
        IIdentityServerInteractionService interaction,
        UserManager<ApplicationUser>? userManager,
        SignInManager<ApplicationUser>? signInManager,
        RoleManager<IdentityRole>? roleManager,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events,
        IClientStore clientStore
        )
    {
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userManager = userManager;
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _clientStore = clientStore;
        _events = events;
        List<string> roles = new List<string>();
        roles.Add("Admin");
        roles.Add("Client");
        _roles = roles;
    }

    [HttpGet]
    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        Input = await BuildRegisterViewModelAsync(returnUrl);
        return Page();
    }

    private async Task<InputModel> BuildRegisterViewModelAsync(string returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
       
        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            var vm = new InputModel
            {
                EnableLocalLogin = local,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
            };

            if (!local)
            {
                vm.ExternalProviders = new[] { new ExternalProvider(context.IdP) };
            }

            return vm;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.Name)
            {
                DisplayName = x.DisplayName ?? x.Name

            }).ToList();

        var allowLocal = true;
        if (context?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        return new InputModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ReturnUrl = returnUrl,
            Username = context?.LoginHint,
            ExternalProviders = providers.ToArray()
        };
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPost()
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        // the user clicked the "cancel" button
        if (Input.Button != "create")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }
        }
        var userCheck = await _userManager.FindByNameAsync(Input.Name);
        if (userCheck != null)
        {
            ModelState.AddModelError("Input.Username", "Invalid username");
        }

        //if (ModelState.IsValid)
        //{
            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
                FirstName = Input.Name,
                LastName = Input.LastName
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
                {
                    var userRole = new IdentityRole
                    {
                        Name = Input.RoleName,
                        NormalizedName = Input.RoleName,

                    };
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(user, Input.RoleName);

                await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(JwtClaimTypes.Name, Input.Username),
                    new Claim(JwtClaimTypes.Email, Input.Email),
                    new Claim(JwtClaimTypes.FamilyName, Input.Name),
                    new Claim(JwtClaimTypes.GivenName, Input.LastName),
                    new Claim(JwtClaimTypes.WebSite, $"http://{Input.Username}.com"),
                    new Claim(JwtClaimTypes.Role,"User") });


                var loginresult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);
                if (loginresult.Succeeded)
                {
                    var checkuser = await _userManager.FindByNameAsync(Input.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl);
                        }

                        // we can trust Input.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
        }
        return Page();
    }

}
