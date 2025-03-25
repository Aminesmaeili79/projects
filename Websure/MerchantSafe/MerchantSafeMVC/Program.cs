using MerchantSafeAPI.Services;
using MerchantSafeAPI.Models;
using MerchantSafeAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MerchantSafe settings
builder.Services.Configure<MerchantSafeConfig>(
    builder.Configuration.GetSection("MerchantSafe"));

// Register IMerchantSafeService
builder.Services.AddScoped<IMerchantSafeService, MerchantSafeService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Add this before app.Run()
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Also make sure you have proper CORS settings if needed
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();