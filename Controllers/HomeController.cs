using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RIFA.Data;
using RIFA.Models;
using RIFA.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Security.Cryptography; // Para SHA256
using System.Text; // Para Encoding.UTF8
using Microsoft.Extensions.Logging;
using System.Threading.Tasks; // Para Task
using Microsoft.AspNetCore.Mvc.Rendering; // �IMPORTANTE para SelectListItem!



namespace RIFA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RIFADbContext _context;

        public HomeController(ILogger<HomeController> logger, RIFADbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Acciones de vista simples
        public IActionResult reglas() => View();
        public IActionResult dudas() => View();
        public IActionResult paginainformativa() => View();
        public IActionResult Index() => View();
        public IActionResult politica() => View();
        public IActionResult nosotros() => View();

        // --- P�GINA DE ADMINISTRACI�N (ALBA) EN HomeController ---
        // SOLO ADMINISTRADORES (de la tabla Administrador) pueden acceder.
        [Authorize(Roles = "Administrador")] // Solo usuarios con el rol "Administrador" (otorgado en el login de Admin)
        [HttpGet]
        public async Task<IActionResult> Alba(string activeTab = "lista") // Permite especificar qu� pesta�a activar
        {
            var viewModel = new AlbaViewModel
            {
                // Cargar la lista de todos los usuarios (de la tabla Usuario, sin sus contrase�as)
                // NO se cargan los administradores de la tabla Administrador aqu�.
                Usuarios = await _context.Usuarios
                                         .Select(u => new PublicUserViewModel
                                         {
                                             UsuarioId = u.UsuarioId,
                                             Nombre = u.Nombre,
                                             Email = u.Email,
                                             PuntosAcumulados = u.PuntosAcumulados,
                                             Rol = u.Rol, // El Rol del modelo Usuario
                                             NumeroBoleta = u.NumeroBoleta
                                         })
                                         .ToListAsync(),
                Mensaje = "Panel de Administraci�n de Usuarios (Tabla Usuario)",
                ActiveTab = activeTab
            };

            if (TempData["Message"] != null)
            {
                viewModel.Message = TempData["Message"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                viewModel.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(viewModel);
        }



        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaUsuario(AlbaViewModel model)
        {
            // Limpiar errores del modelo que no son de NuevoUsuario, si existieran.
            ModelState.Remove("NombreBaja");
            ModelState.Remove("NumeroBoletaBaja");
            ModelState.Remove("UserIdToChangeRole");
            ModelState.Remove("NewRole");

            // ELIMINA ESTAS L�NEAS COMPLETAMENTE, SON LAS QUE CAUSAN EL ERROR CS1061:
            // if (!ModelState.IsValidField("NuevoUsuario.Nombre")) ModelState.Remove("NuevoUsuario.Nombre");
            // if (!ModelState.IsValidField("NuevoUsuario.Email")) ModelState.Remove("NuevoUsuario.Email");
            // if (!ModelState.IsValidField("NuevoUsuario.Contrasena")) ModelState.Remove("NuevoUsuario.Contrasena");
            // if (!ModelState.IsValidField("NuevoUsuario.ConfirmarContrasena")) ModelState.Remove("NuevoUsuario.ConfirmarContrasena");
            // if (!ModelState.IsValidField("NuevoUsuario.NumeroBoleta")) ModelState.Remove("NuevoUsuario.NumeroBoleta");


            // NO necesites TryValidateModel(model.NuevoUsuario); aqu� si los atributos de validaci�n
            // est�n en RegisterViewModel, ASP.NET Core ya los procesa.

            if (ModelState.IsValid) // Esto verificar� las validaciones de RegisterViewModel autom�ticamente.
            {
                var existingUserByEmail = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.NuevoUsuario.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("NuevoUsuario.Email", "Este correo electr�nico ya est� registrado para un usuario.");
                }

                var existingUserByBoleta = await _context.Usuarios.FirstOrDefaultAsync(u => u.NumeroBoleta == model.NuevoUsuario.NumeroBoleta);
                if (existingUserByBoleta != null)
                {
                    ModelState.AddModelError("NuevoUsuario.NumeroBoleta", "Este n�mero de boleta ya est� en uso.");
                }

                var existingAdminByEmail = await _context.Administradors.FirstOrDefaultAsync(a => a.Email == model.NuevoUsuario.Email);
                if (existingAdminByEmail != null)
                {
                    ModelState.AddModelError("NuevoUsuario.Email", "Este correo electr�nico ya est� registrado como administrador principal.");
                }

                // Volver a verificar ModelState.IsValid despu�s de las validaciones de duplicidad
                if (ModelState.IsValid)
                {
                    // Aqu� deber�as tener tu m�todo HashPassword con BCrypt
                    string hashedPassword = HashPassword(model.NuevoUsuario.Contrasena);

                    var newUser = new Usuario
                    {
                        Nombre = model.NuevoUsuario.Nombre,
                        Email = model.NuevoUsuario.Email,
                        Contrasena = hashedPassword,
                        NumeroBoleta = model.NuevoUsuario.NumeroBoleta,
                        PuntosAcumulados = 0,
                        Rol = "Usuario"
                    };

                    _context.Usuarios.Add(newUser);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"�Usuario '{newUser.Nombre}' (Boleta: {newUser.NumeroBoleta}) agregado exitosamente a la tabla de Usuarios!";
                    return RedirectToAction("Alba", new { activeTab = "alta" });
                }
            }

            // Si hay errores (despu�s de ModelState.IsValid inicial o las validaciones de duplicidad),
            // recarga el ViewModel con los datos y la lista de usuarios, y los errores de validaci�n.
            model.Usuarios = await _context.Usuarios
                                             .Select(u => new PublicUserViewModel
                                             {
                                                 UsuarioId = u.UsuarioId,
                                                 Nombre = u.Nombre,
                                                 Email = u.Email,
                                                 PuntosAcumulados = u.PuntosAcumulados,
                                                 Rol = u.Rol,
                                                 NumeroBoleta = u.NumeroBoleta
                                             })
                                             .ToListAsync();
            model.Mensaje = "Panel de Administraci�n de Usuarios (Tabla Usuario)";
            model.ErrorMessage = "Hubo errores al agregar el usuario. Por favor, revisa el formulario.";
            model.ActiveTab = "alta";
            return View("Alba", model);
        }

        // POST: Dar de Baja (Eliminar) un usuario desde Alba (solo de la tabla Usuario)
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BajaUsuario(AlbaViewModel model)
        {
            // Limpiar errores del modelo que no son de Baja
            ModelState.Remove("NuevoUsuario.Nombre");
            ModelState.Remove("NuevoUsuario.Email");
            ModelState.Remove("NuevoUsuario.Contrasena");
            ModelState.Remove("NuevoUsuario.NumeroBoleta");
            ModelState.Remove("NuevoUsuario.ConfirmarContrasena"); // Tambi�n limpiar este
            ModelState.Remove("UserIdToChangeRole");
            ModelState.Remove("NewRole");

            if (string.IsNullOrWhiteSpace(model.NombreBaja) || string.IsNullOrWhiteSpace(model.NumeroBoletaBaja))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingresa el nombre y el n�mero de boleta del usuario a eliminar.");
            }

            if (ModelState.IsValid)
            {
                var usuarioToDelete = await _context.Usuarios
                                                     .FirstOrDefaultAsync(u => u.Nombre == model.NombreBaja && u.NumeroBoleta == model.NumeroBoletaBaja);

                if (usuarioToDelete == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado con el nombre y n�mero de boleta proporcionados en la tabla de Usuarios.");
                }
                else
                {
                    // Eliminar registros relacionados (Historials, Canjes) antes de eliminar al usuario
                    var historials = _context.Historials.Where(h => h.UsuarioId == usuarioToDelete.UsuarioId);
                    _context.Historials.RemoveRange(historials);

                    var canjes = _context.Canjes.Where(c => c.UsuarioId == usuarioToDelete.UsuarioId);
                    _context.Canjes.RemoveRange(canjes);

                    _context.Usuarios.Remove(usuarioToDelete);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"�Usuario '{usuarioToDelete.Nombre}' (Boleta: {usuarioToDelete.NumeroBoleta}) eliminado exitosamente de la tabla de Usuarios!";
                    return RedirectToAction("Alba", new { activeTab = "baja" });
                }
            }

            // Si hay errores, recarga el ViewModel con los datos y la lista de usuarios
            model.Usuarios = await _context.Usuarios
                                             .Select(u => new PublicUserViewModel
                                             {
                                                 UsuarioId = u.UsuarioId,
                                                 Nombre = u.Nombre,
                                                 Email = u.Email,
                                                 PuntosAcumulados = u.PuntosAcumulados,
                                                 Rol = u.Rol,
                                                 NumeroBoleta = u.NumeroBoleta
                                             })
                                             .ToListAsync();
            model.Mensaje = "Panel de Administraci�n de Usuarios (Tabla Usuario)";
            model.ErrorMessage = "Hubo errores al eliminar el usuario. Por favor, revisa los datos.";
            model.ActiveTab = "baja";
            return View("Alba", model);
        }

        // POST: Cambiar Rol de Usuario desde Alba (solo para usuarios de la tabla Usuario)
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRolUsuario(AlbaViewModel model)
        {
            // Limpiar errores del modelo que no son de cambio de rol
            ModelState.Remove("NuevoUsuario.Nombre");
            ModelState.Remove("NuevoUsuario.Email");
            ModelState.Remove("NuevoUsuario.Contrasena");
            ModelState.Remove("NuevoUsuario.NumeroBoleta");
            ModelState.Remove("NuevoUsuario.ConfirmarContrasena"); // Tambi�n limpiar este
            ModelState.Remove("NombreBaja");
            ModelState.Remove("NumeroBoletaBaja");

            if (model.UserIdToChangeRole <= 0 || string.IsNullOrWhiteSpace(model.NewRole))
            {
                TempData["ErrorMessage"] = "Datos incompletos para cambiar el rol.";
                return RedirectToAction("Alba", new { activeTab = "roles" });
            }

            var usuario = await _context.Usuarios.FindAsync(model.UserIdToChangeRole);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado en la tabla de Usuarios.";
                return RedirectToAction("Alba", new { activeTab = "roles" });
            }

            // Validar que el nuevo rol sea "Usuario" solamente
            // Si quieres permitir que se marque un usuario como "Administrador" dentro de la tabla Usuario
            // pero sin que le de permisos de admin reales, podr�as dejar "Administrador" aqu�.
            // PERO, para la restricci�n de los 5 admins �nicos, es mejor que un usuario de la tabla Usuario
            // NUNCA tenga un rol que se confunda con un admin real.
            if (model.NewRole != "Usuario")
            {
                TempData["ErrorMessage"] = "Rol inv�lido para un usuario de la tabla de usuarios. El �nico rol permitido es 'Usuario'. Los administradores se gestionan en una tabla separada.";
                return RedirectToAction("Alba", new { activeTab = "roles" });
            }

            usuario.Rol = model.NewRole; // Asigna el rol al usuario de la tabla Usuario
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"�Rol del usuario '{usuario.Nombre}' (en la tabla Usuarios) actualizado a '{usuario.Rol}' exitosamente!";
            return RedirectToAction("Alba", new { activeTab = "roles" });
        }


        // --- P�GINA DE PRODUCTOS CANJEABLES ---
        [HttpGet]
        [Authorize(Roles = "Usuario,Administrador")] // Ambos roles pueden ver los productos
        public async Task<IActionResult> Productos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            // Obtener puntos del usuario logueado.
            int puntosUsuario = 0;

            // Solo si el rol en los claims es "Usuario", buscamos sus puntos en la tabla Usuarios.
            // Los administradores (de la tabla Administradors) no tienen puntos acumulados en este sistema.
            if (userRoleClaim?.Value == "Usuario")
            {
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int usuarioId))
                {
                    var usuario = await _context.Usuarios.FindAsync(usuarioId);
                    if (usuario != null)
                    {
                        puntosUsuario = usuario.PuntosAcumulados;
                    }
                }
            }
            // Si el rol es "Administrador" (proveniente de la tabla Administradors), puntosUsuario permanece en 0.

            var productos = await _context.Productos.ToListAsync();

            var viewModel = new ProductosViewModel
            {
                ProductosDisponibles = productos,
                PuntosUsuario = puntosUsuario // Los puntos ser�n 0 para administradores "puros"
            };

            if (TempData["Message"] != null)
            {
                viewModel.Message = TempData["Message"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                viewModel.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")] // Solo usuarios normales pueden canjear productos
        public async Task<IActionResult> ComprarProducto(int productoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                TempData["ErrorMessage"] = "Error al identificar el usuario. Por favor, inicia sesi�n de nuevo.";
                return RedirectToAction("ingresar", "Home");
            }

            // Buscar en la tabla de Usuarios (ya que solo usuarios con rol "Usuario" pueden canjear)
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            var producto = await _context.Productos.FindAsync(productoId);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction("Productos", "Home");
            }
            if (producto == null)
            {
                TempData["ErrorMessage"] = "Producto no encontrado.";
                return RedirectToAction("Productos", "Home");
            }
            if (producto.Stock <= 0)
            {
                TempData["ErrorMessage"] = $"Lo sentimos, {producto.Nombre} est� agotado.";
                return RedirectToAction("Productos", "Home");
            }
            if (usuario.PuntosAcumulados < producto.PrecioPuntos)
            {
                TempData["ErrorMessage"] = $"No tienes suficientes puntos para comprar {producto.Nombre}. Necesitas {producto.PrecioPuntos} puntos y solo tienes {usuario.PuntosAcumulados}.";
                return RedirectToAction("Productos", "Home");
            }

            usuario.PuntosAcumulados -= producto.PrecioPuntos;
            producto.Stock -= 1;

            _context.Update(usuario);
            _context.Update(producto);

            var canje = new Canje
            {
                UsuarioId = usuario.UsuarioId,
                ProductoId = producto.ProductoId,
                PuntosCanjeados = producto.PrecioPuntos,
                FechaCanje = DateTime.Now
            };
            _context.Canjes.Add(canje);

            await _context.SaveChangesAsync();

            TempData["Message"] = $"�Has canjeado {producto.Nombre} por {producto.PrecioPuntos} puntos! Te quedan {usuario.PuntosAcumulados} puntos.";
            return RedirectToAction("Productos", "Home");
        }

        // --- INICIO DE SESI�N (ingresar) ---
        [HttpGet]
        public IActionResult ingresar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.Contrasena);
                List<Claim> claims = new List<Claim>();
                string userRoleForRedirection = ""; // Para saber a d�nde redirigir

                if (model.IsAdminLogin)
                {
                    // **MODIFICACI�N CLAVE AQU�:** Solo busca en la tabla Administradors para logins de Admin.
                    // Esto asegura que solo los 5 administradores �nicos (que estar�n en esta tabla) puedan acceder.
                    var adminUser = await _context.Administradors.FirstOrDefaultAsync(a => a.Email == model.Email && a.Contrasena == hashedPassword);
                    if (adminUser != null)
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, adminUser.AdministradorId.ToString()));
                        claims.Add(new Claim(ClaimTypes.Name, adminUser.Nombre));
                        claims.Add(new Claim(ClaimTypes.Email, adminUser.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Administrador")); // Este es el rol que da acceso a funcionalidades de admin
                        userRoleForRedirection = "Administrador";
                    }
                    else
                    {
                        // Si el email/contrase�a no coincide en Administradors, no se autentica como Admin.
                        TempData["ErrorMessage"] = "Credenciales de administrador inv�lidas.";
                        ModelState.AddModelError("", "Correo o contrase�a de administrador incorrectos.");
                        return View(model); // Retorna de inmediato
                    }
                }
                else // Es un login de usuario normal
                {
                    // **MODIFICACI�N CLAVE AQU�:** Solo busca en la tabla Usuarios para logins de usuario normal.
                    var regularUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email && u.Contrasena == hashedPassword);
                    if (regularUser != null)
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, regularUser.UsuarioId.ToString()));
                        claims.Add(new Claim(ClaimTypes.Name, regularUser.Nombre));
                        claims.Add(new Claim(ClaimTypes.Email, regularUser.Email));
                        // El rol del usuario se toma de su propiedad 'Rol' en la tabla Usuarios.
                        // Incluso si un usuario de la tabla Usuarios tiene Rol = "Administrador",
                        // no le dar� acceso a las funciones de admin si no inici� sesi�n con IsAdminLogin = true
                        // y no est� en la tabla 'Administradors'.
                        claims.Add(new Claim(ClaimTypes.Role, regularUser.Rol));
                        claims.Add(new Claim("PuntosAcumulados", regularUser.PuntosAcumulados.ToString())); // Para mostrar en el _Layout
                        userRoleForRedirection = regularUser.Rol; // Podr�a ser "Usuario" o incluso "Administrador" si lo permites en la tabla Usuarios.
                    }
                    else
                    {
                        // Si el email/contrase�a no coincide en Usuarios, no se autentica como Usuario.
                        TempData["ErrorMessage"] = "Credenciales de usuario inv�lidas.";
                        ModelState.AddModelError("", "Correo o contrase�a de usuario incorrectos.");
                        return View(model); // Retorna de inmediato
                    }
                }

                if (claims.Any()) // Si se encontraron claims, el usuario fue autenticado
                {
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // 60 minutos de sesi�n
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["Message"] = "�Inicio de sesi�n exitoso!";

                    if (userRoleForRedirection == "Administrador") // Este rol solo se asigna si viene de la tabla Administradors
                    {
                        return RedirectToAction("Dash", "Home");
                    }
                    else // Si el rol es "Usuario" (o cualquier otro que no sea "Administrador" de la tabla Administradors)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Este bloque ya no deber�a ser alcanzado tan f�cilmente porque los errores se manejan antes.
                    // Pero lo dejo como fallback.
                    TempData["ErrorMessage"] = "Credenciales inv�lidas. Correo o contrase�a incorrectos, o no tienes los permisos para el tipo de inicio de sesi�n seleccionado.";
                    ModelState.AddModelError("", "Correo o contrase�a incorrectos.");
                }
            }
            return View(model);
        }

        // Acci�n para cerrar sesi�n
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Has cerrado sesi�n correctamente.";
            return RedirectToAction("ingresar", "Home");
        }

        // --- RESTABLECIMIENTO DE CONTRASE�A (solo para USUARIOS de la tabla Usuario) ---
        [HttpGet]
        public IActionResult PasswordReset()
        {
            return View(new PasswordResetViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                                            .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (usuario == null)
                {
                    // Por seguridad, no reveles si el correo existe o no
                    ViewBag.ErrorMessage = "Si el correo electr�nico existe en nuestro sistema, la contrase�a ha sido actualizada.";
                    ModelState.Clear();
                    return View(new PasswordResetViewModel());
                }

                string hashedPassword = HashPassword(model.NuevaContrasena);
                usuario.Contrasena = hashedPassword;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                ViewBag.Message = "�Contrase�a de usuario actualizada exitosamente! Ya puedes iniciar sesi�n con tu nueva contrase�a.";
                ModelState.Clear();
                return View(new PasswordResetViewModel());
            }

            ViewBag.ErrorMessage = "Por favor, corrige los errores en el formulario.";
            return View(model);
        }

        // --- REGISTRO DE USUARIOS (register - solo crea USUARIOS normales) ---
        [HttpGet]
        public IActionResult register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> register(RegisterViewModel model)
        {
            // Forzar el rol a "Usuario" para el registro p�blico
            model.Rol = "Usuario";

            if (ModelState.IsValid)
            {
                var existingUserByEmail = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Este correo electr�nico ya est� registrado para un usuario.");
                }

                var existingUserByBoleta = await _context.Usuarios.FirstOrDefaultAsync(u => u.NumeroBoleta == model.NumeroBoleta);
                if (existingUserByBoleta != null)
                {
                    ModelState.AddModelError("NumeroBoleta", "Este n�mero de boleta ya est� en uso por un usuario.");
                }

                // Verificar si el email ya existe en la tabla de Administradores para evitar conflictos
                // con los 5 administradores �nicos.
                var existingAdminByEmail = await _context.Administradors.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (existingAdminByEmail != null)
                {
                    ModelState.AddModelError("Email", "Este correo electr�nico ya est� registrado como administrador principal.");
                }

                if (ModelState.IsValid)
                {
                    var newUser = new Usuario
                    {
                        Nombre = model.Nombre,
                        Email = model.Email,
                        Contrasena = HashPassword(model.Contrasena),
                        NumeroBoleta = model.NumeroBoleta,
                        PuntosAcumulados = 0,
                        Rol = model.Rol // Siempre "Usuario" para registros p�blicos
                    };

                    _context.Usuarios.Add(newUser);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "�Cuenta de usuario creada exitosamente! Ya puedes iniciar sesi�n.";
                    return RedirectToAction("ingresar");
                }
            }
            return View(model);
        }

        // --- REGISTRO DE PUNTOS POR RECICLAJE (solo para administradores) ---
        [Authorize(Roles = "Administrador")] // Solo administradores (de la tabla Administradors) pueden registrar puntos
        [HttpGet]
        public async Task<IActionResult> puntosacumulados()
        {
            var materiales = await _context.MaterialesReciclados.ToListAsync();
            var viewModel = new RegistroPuntosViewModel
            {
                MaterialesDisponibles = materiales.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = m.TipoMaterial,
                    Text = $"{m.TipoMaterial} ({m.PuntosPorUnidad} pts/unidad)"
                }).ToList()
            };
            // Aseg�rate de que los TempData o ViewBag se pasen para mensajes al cargar la vista
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")] // Reafirmar que solo admins pueden postear aqu�
        public async Task<IActionResult> RegistrarPuntos(RegistroPuntosViewModel model)
        {
            // Limpiar errores de otras secciones del modelo si es necesario (similar a Alba)
            // Aunque RegistroPuntosViewModel es m�s simple, es buena pr�ctica si compartiera errores con otros VMs.
            // Para este caso, no es estrictamente necesario, pero lo dejo como nota.

            if (!ModelState.IsValid)
            {
                // Recargar los materiales para que la vista no falle
                model.MaterialesDisponibles = (await _context.MaterialesReciclados.ToListAsync())
                    .Select(m => new SelectListItem // Usar SelectListItem
                    {
                        Value = m.TipoMaterial,
                        Text = $"{m.TipoMaterial} ({m.PuntosPorUnidad} pts/unidad)"
                    }).ToList();
                ViewBag.ErrorMessage = "Por favor, corrige los errores en el formulario.";
                return View("puntosacumulados", model);
            }

            var usuario = await _context.Usuarios
                                         .FirstOrDefaultAsync(u => u.NumeroBoleta == model.NumeroBoleta);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "N�mero de Boleta no encontrado para ning�n usuario.");
                ViewBag.ErrorMessage = "El n�mero de boleta ingresado no corresponde a ning�n usuario registrado.";
                // Recargar los materiales para que la vista no falle
                model.MaterialesDisponibles = (await _context.MaterialesReciclados.ToListAsync())
                    .Select(m => new SelectListItem // Usar SelectListItem
                    {
                        Value = m.TipoMaterial,
                        Text = $"{m.TipoMaterial} ({m.PuntosPorUnidad} pts/unidad)"
                    }).ToList();
                return View("puntosacumulados", model);
            }

            var materialReciclado = await _context.MaterialesReciclados
                                                    .FirstOrDefaultAsync(m => m.TipoMaterial.ToLower() == model.TipoMaterial.ToLower());

            if (materialReciclado == null)
            {
                ModelState.AddModelError(string.Empty, "Tipo de material inv�lido o no configurado.");
                ViewBag.ErrorMessage = "El tipo de material seleccionado no es v�lido o no est� configurado. Contacta al administrador.";
                // Recargar los materiales para que la vista no falle
                model.MaterialesDisponibles = (await _context.MaterialesReciclados.ToListAsync())
                    .Select(m => new SelectListItem // Usar SelectListItem
                    {
                        Value = m.TipoMaterial,
                        Text = $"{m.TipoMaterial} ({m.PuntosPorUnidad} pts/unidad)"
                    }).ToList();
                return View("puntosacumulados", model);
            }

            // Asegurar que Cantidad es tratada como un double y luego casteada a int para puntos, si PuntosPorUnidad es int.
            // Si Cantidad en el ViewModel es int, no necesita el (int) cast aqu�. Asumo que Cantidad puede ser double.
            // Si PuntosPorUnidad es int, y Cantidad es int, entonces el resultado es int.
            int puntosGanados = materialReciclado.PuntosPorUnidad * model.Cantidad; // Asumo model.Cantidad es int

            // Crear entrada en Historial (para el usuario)
            var nuevoHistorial = new Historial
            {
                UsuarioId = usuario.UsuarioId,
                MaterialRecicladoId = materialReciclado.MaterialRecicladoId,
                Cantidad = model.Cantidad,
                PuntosGanados = puntosGanados,
                FechaRegistro = DateTime.Now,
                TipoMaterial = materialReciclado.TipoMaterial
            };

            _context.Historials.Add(nuevoHistorial);

            usuario.PuntosAcumulados += puntosGanados;
            _context.Usuarios.Update(usuario);

            // Crear entrada en Registro (para la auditor�a del administrador)
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim != null && int.TryParse(adminIdClaim.Value, out int adminId))
            {
                var registro = new Registro
                {
                    AdministradorId = adminId,
                    Descripcion = $"Registro de {model.Cantidad} de {model.TipoMaterial} para el usuario {usuario.Nombre} (Boleta: {usuario.NumeroBoleta}). Puntos ganados: {puntosGanados}.",
                    Fecha = DateTime.Now,
                    MaterialRecicladoId = materialReciclado.MaterialRecicladoId,
                    CantidadRegistrada = model.Cantidad // Guardar la cantidad registrada en el log del administrador
                };
                _context.Registros.Add(registro);
            }
            else
            {
                _logger.LogWarning("AdminId no encontrado en los Claims al registrar puntos. No se cre� entrada en la tabla Registro.");
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = $"Puntos registrados exitosamente. Se han a�adido {puntosGanados} puntos a {usuario.Nombre}.";
            // Limpiar el modelo para que el formulario se resetee
            ModelState.Clear();
            model.NumeroBoleta = string.Empty;
            model.Cantidad = 0;
            // Recargar los materiales para que la vista no falle
            model.MaterialesDisponibles = (await _context.MaterialesReciclados.ToListAsync())
                .Select(m => new SelectListItem // Usar SelectListItem
                {
                    Value = m.TipoMaterial,
                    Text = $"{m.TipoMaterial} ({m.PuntosPorUnidad} pts/unidad)"
                }).ToList();
            return View("puntosacumulados", model);
        }

        // --- HISTORIAL DE RECICLAJE DEL USUARIO LOGUEADO ---
        [Authorize(Roles = "Usuario,Administrador")] // Ambos roles pueden ver su historial
        [HttpGet]
        public async Task<IActionResult> historial()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Error al identificar tu usuario. Por favor, intenta iniciar sesi�n nuevamente.";
                return RedirectToAction("ingresar");
            }

            var esAdminClaim = User.IsInRole("Administrador");

            // Para verificar si el usuario logueado como "Administrador" (por el claim)
            // es realmente un registro de la tabla 'Administradors'.
            // Si el ID del claim viene de la tabla 'Administradors', entonces isPureAdmin ser� true.
            // Si viene de la tabla 'Usuarios' (aunque su rol sea "Administrador"), isPureAdmin ser� false.
            var isPureAdmin = esAdminClaim && await _context.Administradors.AnyAsync(a => a.AdministradorId == userId);

            if (isPureAdmin)
            {
                TempData["ErrorMessage"] = "Los administradores principales no tienen un historial de reciclaje personal en esta vista. Si iniciaste sesi�n como administrador principal, solo puedes ver el Dashboard y registrar puntos.";
                return RedirectToAction("Dash");
            }

            // Si es un usuario normal (rol "Usuario") O un usuario de la tabla Usuario que casualmente
            // tiene un rol "Administrador" (pero no es un admin principal), entonces se le muestra el historial.
            var historialDelUsuario = await _context.Historials
                                                    .Where(h => h.UsuarioId == userId)
                                                    .Include(h => h.MaterialReciclado)
                                                    .OrderByDescending(h => h.FechaRegistro)
                                                    .ToListAsync();

            return View(historialDelUsuario);
        }

        // --- M�todo GET para mostrar el Dashboard (solo administradores) ---
        [HttpGet]
        [Authorize(Roles = "Administrador")] // Solo administradores (de la tabla Administradors) pueden acceder al dashboard
        public async Task<IActionResult> Dash()
        {
            var viewModel = new DashboardViewModel();

            // Estad�sticas de Usuarios (tabla Usuario)
            viewModel.TotalUsuarios = await _context.Usuarios.CountAsync();

            // Estad�sticas de reciclaje (tabla Historial)
            var materialesRecicladosStats = await _context.Historials
                .Include(h => h.MaterialReciclado)
                .GroupBy(h => h.MaterialReciclado.TipoMaterial.ToLower())
                .Select(g => new { Tipo = g.Key, TotalCantidad = g.Sum(h => (double)h.Cantidad) })
                .ToListAsync();

            viewModel.TotalBotellaPET = materialesRecicladosStats.FirstOrDefault(m => m.Tipo == "pet")?.TotalCantidad ?? 0.0;
            viewModel.TotalTapas = materialesRecicladosStats.FirstOrDefault(m => m.Tipo == "tapas")?.TotalCantidad ?? 0.0;
            viewModel.TotalHojasPapel = materialesRecicladosStats.FirstOrDefault(m => m.Tipo == "papel")?.TotalCantidad ?? 0.0;
            viewModel.TotalMaterialesReciclados = materialesRecicladosStats.Sum(s => s.TotalCantidad);

            // Estad�sticas de canjes (tabla Canje)
            viewModel.TotalProductosCanjeados = await _context.Canjes.CountAsync();
            viewModel.TotalPuntosCanjeadosGlobal = await _context.Canjes.SumAsync(c => c.PuntosCanjeados);

            viewModel.Mensaje = "Bienvenido al Dashboard de Administraci�n";

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // --- FUNCIONES DE HASHING DE CONTRASE�AS ---
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // La verificaci�n de contrase�a es simple si usas el mismo algoritmo de hashing
        // Nota: Este m�todo no es usado directamente en 'Ingresar' actualmente,
        // pero es una buena pr�ctica tenerlo si el hashing es m�s complejo.
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        // Helper para obtener el ID del usuario actual (del claim)
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // O lanzar una excepci�n si no se puede obtener el ID
        }
    }
}