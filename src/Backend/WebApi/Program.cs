using Core.Repositories;
using Core.Repositories.Security;
using Core.Services;
using Core.Services.Security;
using Repository;
using Repository.Clientes;
using Repository.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// registro de servicios y repositorios
builder.Services.AddTransient<IConnectionProvider, ConnectionProvider>();
builder.Services.AddTransient<IClienteServicio, ClienteServicio>();
builder.Services.AddTransient<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddTransient<IMenuPermissionService, MenuPermissionService>();
builder.Services.AddTransient<IMenuPermissionRepository, MenuPermissionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
