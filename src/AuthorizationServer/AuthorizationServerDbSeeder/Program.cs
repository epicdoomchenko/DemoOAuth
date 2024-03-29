using AuthorizationServer.Data;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SeedData = AuthorizationServerDbSeeder.SeedData;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("seed-config.json", false)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException();

var option = args.FirstOrDefault();

var services = new ServiceCollection();
services.AddLogging();
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

await using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync();

var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

foreach (var user in SeedData.GetData())
{
    var (userData, claims) = user;
    var userInDb = await userMgr.FindByNameAsync(userData.UserName!);
    if (userInDb != null)
    {
        Log.Debug($"{userData.UserName} already exists");
        continue;
    }

    var result = await userMgr.CreateAsync(userData, "Pass123$");
    if (!result.Succeeded)
    {
        throw new Exception(result.Errors.First().Description);
    }

    result = await userMgr.AddClaimsAsync(userData, claims);
    if (!result.Succeeded)
    {
        throw new Exception(result.Errors.First().Description);
    }

    Log.Debug($"{userData.UserName} created");
}