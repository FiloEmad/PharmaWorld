//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();
//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// Add session services
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();  // This is for Razor Pages, not MVC
builder.Services.AddSingleton<WebApplication3.Pages.Models.DB>();
builder.Services.AddDistributedMemoryCache();  // Session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout
    options.Cookie.HttpOnly = true;  // Session cookie is HTTP only
    options.Cookie.IsEssential = true;  // Cookie is essential for the app to work
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Show error page in production
    app.UseHsts();  // Use HTTP Strict Transport Security
}
else
{
    app.UseDeveloperExceptionPage();  // Show developer exceptions in development
}

app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
app.UseStaticFiles();  // Serve static files (like images, CSS, JS)

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();  // Handle user authorization

// Map Razor Pages routes
app.MapRazorPages();  // Maps the Razor Pages routes

app.Run();  // Run the application

