using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LisansEşlemeUyg.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Gerekli Servislerin Eklenmesi
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache(); // MemoryCache servisi eklendi

// Logging ayarları
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// 2. Middleware'in Entegrasyonu
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Lisans doğrulama middleware'ini buraya ekleyin
app.UseMiddleware<LicenseValidationMiddleware>();

app.UseAuthorization();

// 3. Route Ayarları
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=License}/{action=ValidateLicense}/{id?}");

app.Run();
