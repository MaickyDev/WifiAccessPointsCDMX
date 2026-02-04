using OfficeOpenXml;
using FluentValidation;
using Scalar.AspNetCore;
using WifiAccessPointsCDMX.Data;
using WifiAccessPointsCDMX.Models;
using Microsoft.EntityFrameworkCore;
using WifiAccessPointsCDMX.Services;
using WifiAccessPointsCDMX.Interfaces;
using WifiAccessPointsCDMX.Validators;
using WifiAccessPointsCDMX.Repositories;
using WifiAccessPointsCDMX.Interfaces.Services;
using WifiAccessPointsCDMX.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Unit of Work map
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<AccessPointsDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();