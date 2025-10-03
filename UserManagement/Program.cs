//var builder = WebApplication.CreateBuilder(args);
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Repo Services
builder.Services.AddTransient<IUserService, UserService>();
//Common Services
builder.Services.AddTransient<ICommonService, CommonService>();

//DbContext Services
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(configuration.GetConnectionString("dbcon")));


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
