using logisticsSystem.Controllers;
using logisticsSystem.Data;
using logisticsSystem.Exceptions;
using logisticsSystem.MiddleWares;
using logisticsSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LogisticsSystemContext>();
builder.Services.AddScoped<TruckService>();
builder.Services.AddScoped<ItensShippedService>();
builder.Services.AddScoped<EmployeeWageService>();
builder.Services.AddScoped<HandleException>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalErrorHandingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
