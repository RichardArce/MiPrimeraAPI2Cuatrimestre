using Scalar.AspNetCore;
using MiPrimeraAPI2Cuatrimestre.BLL.Servicios;
using MiPrimeraAPI2Cuatrimestre.DAL.Repositorios;
using MiPrimeraAPI2Cuatrimestre.DAL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configurar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios de inyección de dependencias
builder.Services.AddScoped<ICorreoServicio, CorreoServicio>();
builder.Services.AddScoped<IPersonaRepositorio, RepositorioPersona>();
builder.Services.AddScoped<IPersonaServicio, PersonaServicio>();

var app = builder.Build();
app.MapScalarApiReference();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
