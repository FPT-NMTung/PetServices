using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors();

// Add session and time
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(100));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Error/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManaOnly", policy => policy.RequireRole("MANAGER"));
    options.AddPolicy("CusOnly", policy => policy.RequireRole("CUSTOMER"));
    options.AddPolicy("PartnerOnly", policy => policy.RequireRole("PARTNER"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});

app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
    name: "manager",
    areaName: "Manager",
    pattern: "Manager/{controller=Information}/{action=Index}/{id?}");

    endpoints.MapAreaControllerRoute(
    name: "customer",
    areaName: "Customer",
    pattern: "Customer/{controller=MenuCustomer}/{action=Index}/{id?}");

    endpoints.MapAreaControllerRoute(
    name: "partner",
    areaName: "Partner",
    pattern: "Partner/{controller=HomePartner}/{action=Index}/{id?}");

    endpoints.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Account}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
