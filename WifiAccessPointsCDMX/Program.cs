using OfficeOpenXml;
using FluentValidation;
using Scalar.AspNetCore;
using WifiAccessPointsCDMX.Data;
using WifiAccessPointsCDMX.Models;
using WifiAccessPointsCDMX.GraphQL;
using Microsoft.EntityFrameworkCore;
using WifiAccessPointsCDMX.Services;
using WifiAccessPointsCDMX.Interfaces;
using WifiAccessPointsCDMX.Validators;
using WifiAccessPointsCDMX.Middlewares;
using WifiAccessPointsCDMX.Repositories;
using WifiAccessPointsCDMX.Interfaces.Services;
using WifiAccessPointsCDMX.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string? secretPath = builder.Configuration["ConnectionStrings:DefaultConnection"];

// 2. If the file exists, read it and overwrite the DefaultConnection
if (!string.IsNullOrEmpty(secretPath) && File.Exists(secretPath))
{
    string secretValue = File.ReadAllText(secretPath).Trim();
    builder.Configuration["ConnectionStrings:DefaultConnection"] = secretValue;
}

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Unit of Work map
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<AccessPointsDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Respositories map
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IAlcaldiaRepository, AlcaldiaRepository>();
builder.Services.AddScoped<IAccessPointRepository, AccessPointRepository>();
// Services map
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IAccessPointService, AccessPointService>();
// Validator map
builder.Services.AddScoped<IValidator<ExcelAccessPointModel>, ExcelAccessPointValidator>();
// EPPlus
ExcelPackage.License.SetNonCommercialPersonal(builder.Configuration["EPPlus:fullName"]);
// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGraphQL();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();