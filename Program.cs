using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RIFA.Data; // Aseg�rate de que RIFADbContext est� en este namespace o en el correcto.

var builder = WebApplication.CreateBuilder(args);

// *** CAMBIO AQU�: Se usaba "DefaultConnection", ahora se usa "StringCONSQLlocal" ***
builder.Services.AddDbContext<RIFADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StringCONSQLlocal")));
// Ahora busca la cadena de conexi�n con el nombre "StringCONSQLlocal"
// que es el nombre que le diste en tu appsettings.json.

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/ingresar"; // La ruta a tu p�gina de login
        options.LogoutPath = "/Home/Logout";   // La ruta para cerrar sesi�n
        options.AccessDeniedPath = "/Home/AccessDenied"; // Opcional: p�gina para acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de vida de la cookie de autenticaci�n
        options.SlidingExpiration = true; // Renueva la cookie si est� a mitad de expirar
    });


// Agrega los controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware y configuraci�n de rutas
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