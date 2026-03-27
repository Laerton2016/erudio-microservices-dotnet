using AutoMapper;
using GeekShoping.ProductAPI.Config;
using GeekShoping.ProductAPI.Model.Context;
using GeekShoping.ProductAPI.Repository;
using GeekShoping.ProductAPI.Repository.Implematation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration["MySqlConnection:MySQLConnetionString"];
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Criação de scoped para o repositório
builder.Services.AddScoped<IProductRopository, ProductRopository>();

builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(opts =>
{
    opts.Authority = "https://localhost:4436";
    opts.RequireHttpsMetadata = false;

    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };

    opts.Events = new JwtBearerEvents
    {
        OnMessageReceived = async context =>
        {
            var config = await context.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);

            // SE O JWKS_URI VIER NULO, NÓS INJETAMOS ELE NA MÃO!
            if (string.IsNullOrEmpty(config.JwksUri))
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(opts.MetadataAddress);

                    // Use um parser genérico para ver se o campo existe no objeto bruto
                    var json = System.Text.Json.JsonDocument.Parse(response);
                    config.JwksUri = json.RootElement.GetProperty("jwks_uri").GetString();

                    // 2. Faz a chamada manual para o endpoint de chaves (JWKS)
                    var jwksResponse = await httpClient.GetStringAsync(config.JwksUri);

                    // 3. Converte o JSON bruto para um conjunto de chaves (JsonWebKeySet)
                    var jwks = new JsonWebKeySet(jwksResponse);

                    // 4. Povoa a lista de SecurityKey
                    // O JsonWebKeySet já possui uma propriedade 'GetSigningKeys()' que retorna IEnumerable<SecurityKey>
                    IEnumerable<SecurityKey> keys = jwks.GetSigningKeys();

                    // 5. Injeta manualmente nos parâmetros de validação da API
                    context.Options.TokenValidationParameters.IssuerSigningKeys = keys;

                    Console.WriteLine($"[DEBUG] Sucesso! {keys.Count()} chaves injetadas manualmente.");
                }
            }

            Console.WriteLine($"[DEBUG] JWKS URI: {config.JwksUri}");
            Console.WriteLine($"[DEBUG] Chaves encontradas: {config.SigningKeys.Count}");
        },
        OnAuthenticationFailed = context =>
        {

            if (context.Exception is SecurityTokenExpiredException)
            {
                Console.WriteLine("- Causa: O token está expirado (campo 'exp').");
            }

            if (context.Exception is SecurityTokenExpiredException expiredExp)
            {
                Console.WriteLine($"\n[DEBUG TIME ERROR]");
                Console.WriteLine($"> Token Expira em (UTC): {expiredExp.Expires:HH:mm:ss}");
                Console.WriteLine($"> Relógio da API (UTC): {DateTime.UtcNow:HH:mm:ss}");
                Console.WriteLine($"> Diferença: {expiredExp.Expires - DateTime.UtcNow}");
            }
            Console.WriteLine($"DEBUG: Falha: {context.Exception.Message}");
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            var expiration = context.SecurityToken.ValidTo;
            Console.WriteLine($"DEBUG: Token Válido até: {expiration.ToLocalTime()}");
            Console.WriteLine($"DEBUG: Agora são: {DateTime.Now}");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "geek_shopping");

    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeekShoping.ProductAPI", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = @"Enter 'Bearer' [space] and your token!",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme ="oauth2",
                Name ="Bearer",
                In = ParameterLocation.Header
            },

            new List<String>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
