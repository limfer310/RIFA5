using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RIFA.Data; // Asegúrate de que RIFADbContext esté en este namespace o en el correcto.

var builder = WebApplication.CreateBuilder(args);

// *** CAMBIO AQUÍ: Se usaba "DefaultConnection", ahora se usa "StringCONSQLlocal" ***
builder.Services.AddDbContext<RIFADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StringCONSQLlocal")));
// Ahora busca la cadena de conexión con el nombre "StringCONSQLlocal"
// que es el nombre que le diste en tu appsettings.json.

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/ingresar"; // La ruta a tu página de login
        options.LogoutPath = "/Home/Logout";   // La ruta para cerrar sesión
        options.AccessDeniedPath = "/Home/AccessDenied"; // Opcional: página para acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de vida de la cookie de autenticación
        options.SlidingExpiration = true; // Renueva la cookie si está a mitad de expirar
    });


// Agrega los controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware y configuración de rutas
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();