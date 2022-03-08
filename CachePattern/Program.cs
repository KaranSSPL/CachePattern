using CachePattern.Data;
using CachePattern.Services;
using CachePattern.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


var cacheSettings = builder.Configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>();

builder.Services.AddTransient<ISerializerService, NewtonSoftService>();
if (cacheSettings.UseDistributedCache)
{
    if (cacheSettings.PreferRedis)
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheSettings.RedisURL;
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
            {
                AbortOnConnectFail = true,
                EndPoints = { cacheSettings.RedisURL }
            };
        });
    else builder.Services.AddDistributedMemoryCache();

    builder.Services.AddTransient<ICacheService, DistributedCacheService>();
}
else
{
    builder.Services.AddMemoryCache();
    builder.Services.AddTransient<ICacheService, LocalCacheService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
