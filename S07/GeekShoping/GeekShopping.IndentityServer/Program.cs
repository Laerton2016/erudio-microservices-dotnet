using Duende.IdentityServer.Services;
using GeekShopping.IndentityServer.Configuration;
using GeekShopping.IndentityServer.Initializer;
using GeekShopping.IndentityServer.Model;
using GeekShopping.IndentityServer.Model.Context;
using GeekShopping.IndentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

#region Configurações de banco
var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration["MySqlConnection:MySQLConnetionString"];
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));

#endregion

#region Configurações do Identity Server
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MySQLContext>()
    .AddDefaultTokenProviders();

var builderIdentity = builder.Services
    .AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
    options.Discovery.ShowKeySet = true;
    options.IssuerUri = "https://localhost:4436";

})
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddAspNetIdentity<ApplicationUser>();

#region Injeção de dependência 

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IProfileService, ProfileService>();

#endregion

//builderIdentity.AddDeveloperSigningCredential();
#endregion


// IMPORTANTE: Adicione o suporte para Razor Pages
builder.Services.AddRazorPages();

// Mantenha este se você também for usar Controllers (API ou MVC)
builder.Services.AddControllersWithViews();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();


//Posição estrategica para as configurações e uso do Identity server
app.UseIdentityServer();

app.UseAuthorization();


// --- INÍCIO DO BLOCO DE INICIALIZAÇÃO ---
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    initializer.Initialize(); // Ou await initializer.Initialize() se for assíncrono
}
// --- FIM DO BLOCO ---

app.MapRazorPages().RequireAuthorization();

app.Run();