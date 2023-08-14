using HELMo_bilite_2023_MartinDavid;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionStringsDevelop") ?? throw new InvalidOperationException("Connection string not found.");
    builder.Services.AddDbContext<DbContextHelmoBilite>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsDevelop")));
    builder.Services.AddScoped<CamionRepository>();
    builder.Services.AddScoped<ChauffeurRepository>();
    builder.Services.AddScoped<ClientRepository>();
    builder.Services.AddScoped<LivraisonRepository>();
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionStringsProd") ?? throw new InvalidOperationException("Connection string 'HelmobiliteDbContextConnection' not found.");
    builder.Services.AddDbContext<DbContextHelmoBilite>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsProd")));
    builder.Services.AddScoped<CamionRepository>();
    builder.Services.AddScoped<ChauffeurRepository>();
    builder.Services.AddScoped<ClientRepository>();
    builder.Services.AddScoped<LivraisonRepository>();
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<Utilisateur, IdentityRole>()
    .AddEntityFrameworkStores<DbContextHelmoBilite>()
    .AddDefaultUI();

// builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<DbContextUtilisateurLivraison>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Utilisateur>>();
    DataInitializer.SeedRole(roleManager);
    DataInitializer.Seed(userManager);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();