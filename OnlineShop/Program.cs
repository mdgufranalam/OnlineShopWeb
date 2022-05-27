using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.ServiceHelper;
using OnlineShop.ServiceHelper.Interface;
using OnlineShop.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();;

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
var constr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(constr));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe")); //Payment Options

builder.Services.AddHttpClient();
builder.Services.AddTransient<IHttpClientHelper, HttpClientHelper>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

var APIUrl = builder.Configuration["OnlineShopAPIUrl"];
builder.Services.AddHttpClient("ShopApi", httpClient =>
{
    httpClient.BaseAddress = new Uri(APIUrl);
});

builder.Services.AddRazorPages();

//binding the auth0 property
var auth0 = Auth0.Instance;
builder.Configuration.Bind("auth0", auth0);
builder.Services.AddSingleton(auth0);
builder.Services.AddSingleton<IAuth0ClientHelper, Auth0ClientHelper>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "areas",
//      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//    );
//});

app.Run();
