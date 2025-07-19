using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Services.Mappers;
using EmployeeManagement.Services.Services.DepartmentManagement;
using EmployeeManagement.Services.Services.EmployeeManagement;
using EmployeeManagement.Services.Services.LogHistoryManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ILogHistoryService, LogHistoryService>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);




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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
