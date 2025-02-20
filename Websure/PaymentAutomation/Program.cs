using PaymentAutomation.Services;
using PaymentAutomation.Models;
using PaymentAutomation.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MerchantSafeConfig>(
    builder.Configuration.GetSection("MerchantSafe"));

builder.Services.AddHttpClient<IMerchantSafeService, MerchantSafeService>();

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