using System.ComponentModel.DataAnnotations;
using static DuendeIdentityServer.Pages.Login.ViewModel;

namespace DuendeIdentityServer.Pages.Create;

public class InputModel
{
    [Required]
    public string? Username { get; set; }
    public string? Email { get; set; }

    public string? Name { get; set; }
    public string LastName { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? ReturnUrl { get; set; }

    public string RoleName { get; set; }
    public string? Button { get; set; }

    public bool? RememberLogin { get; set; }

    public bool AllowRememberLogin { get; set; } = true;
    public bool EnableLocalLogin { get; set; } = true;

    public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
    public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
}
