using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================================
// 1. CONFIGURACIÓN DE CULTURA (PARA DECIMALES CON PUNTO)
// ==========================================================================
// Esto asegura que el sistema acepte el punto "." como separador decimal 
// sin importar el idioma de la computadora donde se ejecute.
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// ==========================================================================
// 2. CONFIGURACIÓN DE SERVICIOS
// ==========================================================================

// Configuración de SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configuración de Identity (Seguridad)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuración de Cookies (Accesos)
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==========================================================================
// 3. INICIALIZACIÓN DE DATOS (ROLES Y USUARIOS)
// ==========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Aplica migraciones pendientes automáticamente
    context.Database.Migrate();

    // --- Crear Roles ---
    string[] roles = { "Administrador", "Farmacéutico", "Cliente" };
    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }

    // --- Crear ADMINISTRADOR ---
    var adminEmail = "admin@farmacia.com";
    if (userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult() == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nombre = "Administrador",
            Apellido = "Principal",
            EmailConfirmed = true
        };
        var result = userManager.CreateAsync(adminUser, "Admin123!").GetAwaiter().GetResult();
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Administrador").GetAwaiter().GetResult();
        }
    }

    // --- Crear FARMACÉUTICO ---
    var farmaEmail = "farma@farmacia.com";
    if (userManager.FindByEmailAsync(farmaEmail).GetAwaiter().GetResult() == null)
    {
        var farmaUser = new ApplicationUser
        {
            UserName = farmaEmail,
            Email = farmaEmail,
            Nombre = "Farmacéutico",
            Apellido = "De Turno",
            EmailConfirmed = true
        };
        var result = userManager.CreateAsync(farmaUser, "Farma123!").GetAwaiter().GetResult();
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(farmaUser, "Farmacéutico").GetAwaiter().GetResult();
        }
    }
}

// ==========================================================================
// 4. PIPELINE DE HTTP (MIDDLEWARE)
// ==========================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// El orden es vital: Autenticación primero, luego Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();