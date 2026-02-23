using AutoMapper;
using GeekShoping.ProductAPI.Config;
using GeekShoping.ProductAPI.Model.Context;
using GeekShoping.ProductAPI.Repository;
using GeekShoping.ProductAPI.Repository.Implematation;
using Microsoft.EntityFrameworkCore;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
