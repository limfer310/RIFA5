// Data/RIFADbContext.cs
using Microsoft.EntityFrameworkCore;
using RIFA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Ya incluido, pero lo mantengo por si acaso
using System.Security.Cryptography; // Para SHA256
using System.Text; // Para Encoding.UTF8

namespace RIFA.Data
{
    // CORRECTO: La clase es 'partial'
    public partial class RIFADbContext : DbContext
    {
        public RIFADbContext(DbContextOptions<RIFADbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MaterialReciclado> MaterialesReciclados { get; set; }
        public DbSet<Historial> Historials { get; set; } // Nombre del DbSet en plural
        public DbSet<Administrador> Administradors { get; set; } // Nombre del DbSet en plural
        public DbSet<Producto> Productos { get; set; } // Nombre del DbSet en plural
        public DbSet<Registro> Registros { get; set; } // Nombre del DbSet en plural
        public DbSet<Canje> Canjes { get; set; } // Nombre del DbSet en plural

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CONFIGURACIÓN DEL MODELO ADMINISTRADOR
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.AdministradorId);
                entity.ToTable("Administrador");
                entity.Property(e => e.AdministradorId)
                    .HasColumnName("AdministradorID")
                    .ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Contrasena).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            });

            // CONFIGURACIÓN DEL MODELO MATERIALRECICLADO
            modelBuilder.Entity<MaterialReciclado>(entity =>
            {
                entity.HasKey(e => e.MaterialRecicladoId);
                entity.ToTable("MaterialReciclado");
                entity.Property(e => e.MaterialRecicladoId)
                    .HasColumnName("MaterialRecicladoID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.TipoMaterial).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PuntosPorUnidad).IsRequired();
                // Si estas propiedades no existen en tu modelo MaterialReciclado.cs, ¡debes añadirlas!
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            });

            // CONFIGURACIÓN DEL MODELO HISTORIAL
            modelBuilder.Entity<Historial>(entity =>
            {
                entity.HasKey(e => e.HistorialId);
                entity.ToTable("Historial");
                entity.Property(e => e.HistorialId)
                    .HasColumnName("HistorialID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("GETDATE()")
                    .HasColumnType("datetime");
                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
                entity.Property(e => e.MaterialRecicladoId).HasColumnName("MaterialRecicladoID");

                entity.HasOne(d => d.MaterialReciclado)
                    .WithMany(p => p.Historials)
                    .HasForeignKey(d => d.MaterialRecicladoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.Historials)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.TipoMaterial) // Esto causaba un CS1061 si no existía en el modelo
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PuntosGanados).IsRequired();
            });

            // CONFIGURACIÓN DEL MODELO PRODUCTO (CANJEABLE)
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.ProductoId);
                entity.ToTable("Producto");
                entity.Property(e => e.ProductoId)
                    .HasColumnName("ProductoID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.PrecioPuntos).IsRequired();
                entity.Property(e => e.ImagenUrl).HasMaxLength(500); // Asegurado que esté aquí
                entity.Property(e => e.Stock).IsRequired(); // Asegurado que esté aquí
            });

            // CONFIGURACIÓN DEL MODELO REGISTRO
            modelBuilder.Entity<Registro>(entity =>
            {
                entity.HasKey(e => e.RegistroId);
                entity.ToTable("Registro");
                entity.Property(e => e.RegistroId)
                    .HasColumnName("RegistroID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.AdministradorId).HasColumnName("AdministradorID");
                // Esto causaba un CS1061 si no existía en el modelo
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("GETDATE()")
                    .HasColumnType("datetime");
                entity.Property(e => e.MaterialRecicladoId).HasColumnName("MaterialRecicladoID");
                entity.Property(e => e.CantidadRegistrada).IsRequired(); // Asegurado que esté aquí y sea requerido.

                entity.HasOne(d => d.MaterialReciclado)
                    .WithMany(p => p.Registros)
                    .HasForeignKey(d => d.MaterialRecicladoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Administrador)
                    .WithMany(p => p.Registros)
                    .HasForeignKey(d => d.AdministradorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CONFIGURACIÓN DEL MODELO USUARIO
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.UsuarioId);
                entity.ToTable("Usuario");
                entity.Property(e => e.UsuarioId)
                    .HasColumnName("UsuarioID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Contrasena).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PuntosAcumulados).HasDefaultValue(0);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NumeroBoleta).HasMaxLength(50); // REMOVIDO .IsRequired() si es string?
                entity.HasIndex(e => e.Email).IsUnique();
                // Solo si NumeroBoleta NUNCA es nulo. Si puede ser nulo, esta línea causaría problemas.
                // Si puede ser nulo, necesitas un índice filtrado si tu base de datos lo soporta (ej. SQL Server)
                // entity.HasIndex(e => e.NumeroBoleta).IsUnique().HasFilter("NumeroBoleta IS NOT NULL");
                // Para bases de datos que no soportan HasFilter, simplemente remueve el .IsUnique() si NumeroBoleta puede ser nulo.
                entity.HasIndex(e => e.NumeroBoleta).IsUnique(); // Asumiendo que es siempre único y no nulo

                entity.Property(e => e.Rol)
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue("Usuario");
            });

            // CONFIGURACIÓN DEL MODELO CANJE
            modelBuilder.Entity<Canje>(entity =>
            {
                entity.HasKey(e => e.CanjeId);
                entity.ToTable("Canje");

                entity.Property(e => e.CanjeId)
                    .HasColumnName("CanjeID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
                entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

                entity.Property(e => e.PuntosCanjeados).IsRequired();
                entity.Property(e => e.FechaCanje).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");

                entity.HasOne(c => c.Usuario)
                    .WithMany(u => u.Canjes)
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Producto)
                    .WithMany() // Si Producto no tiene una colección de Canjes, esto está bien
                    .HasForeignKey(c => c.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict para evitar eliminar productos si hay canjes asociados
            });

            // --- SEMBRADO DE DATOS INICIALES (Seed Data) ---

            // Sembrar MaterialesReciclados
            modelBuilder.Entity<MaterialReciclado>().HasData(
                new MaterialReciclado { MaterialRecicladoId = 1, TipoMaterial = "Botellas PET", PuntosPorUnidad = 5, Descripcion = "Botellas de plástico PET" },
                new MaterialReciclado { MaterialRecicladoId = 2, TipoMaterial = "Tapas", PuntosPorUnidad = 1, Descripcion = "Tapas de plástico de botellas"},
                new MaterialReciclado { MaterialRecicladoId = 3, TipoMaterial = "Hojas de Papel", PuntosPorUnidad = 2, Descripcion = "Hojas de papel de desecho"}
            );

            // Sembrar Productos (canjeables)
            modelBuilder.Entity<Producto>().HasData(
                new Producto { ProductoId = 1, Nombre = "Copias", Descripcion = "1 copia blanco y negro.", PrecioPuntos = 2, Stock = 900, ImagenUrl = "/imagenes/copias.jpg" },
                new Producto { ProductoId = 2, Nombre = "Plumas", Descripcion = "1 pluma negra.", PrecioPuntos = 10, Stock = 900, ImagenUrl = "/imagenes/pluma.jpg" },
                new Producto { ProductoId = 3, Nombre = "Impresiones", Descripcion = "1 impresión a b/n tamaño carta.", PrecioPuntos = 4, Stock = 900, ImagenUrl = "/imagenes//impresion.jpg" },
                new Producto { ProductoId = 4, Nombre = "Lápices", Descripcion = "1 lápiz de grafito HB.", PrecioPuntos = 10, Stock = 900, ImagenUrl = "/imagenes/lapiz.jpg" },
                new Producto { ProductoId = 5, Nombre = "Tijeras", Descripcion = "Tijeras de acero inoxidable.", PrecioPuntos = 20, Stock = 900, ImagenUrl = "/imagenes/tijera.jpg" },
                new Producto { ProductoId = 6, Nombre = "Cuadernos", Descripcion = "Cuaderno profesional de 100 hojas", PrecioPuntos = 60, Stock = 900, ImagenUrl = "/imagenes/cuaderno.jpg" },
                new Producto { ProductoId = 7, Nombre = "Reglas", Descripcion = "Regla de plástico de 20cm.", PrecioPuntos = 15, Stock = 900, ImagenUrl = "/imagenes/regla.jpg" },
                new Producto { ProductoId = 8, Nombre = "Folders", Descripcion = "Folder marrón de cartón.", PrecioPuntos = 5, Stock = 900, ImagenUrl = "/imagenes/folder.jpg" }
            );

            // Sembrar Administradores (los 5 únicos)
            // IMPORTANTE: Los AdministradorId aquí deben ser ÚNICOS y no deben chocar con IDs generados automáticamente si ya tienes datos.
            // Si tu tabla de Administradors está vacía, puedes empezar con 1, 2, 3, etc.
            // Si ya tienes IDs, ajusta estos o limpia la tabla antes de la migración si no te importa perder esos datos.
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador { AdministradorId = 1, Nombre = "Bruno Perez", Email = "brunoarroyo1801@gmail.com", Contrasena = HashPassword("elchango13805") },
                new Administrador { AdministradorId = 2, Nombre = "Daniela Huizar", Email = "munozhuizardaniela@gmail.com", Contrasena = HashPassword("d4n13l4mun0z") },
                new Administrador { AdministradorId = 3, Nombre = "Maria Leyva", Email = "leyvahernandez310@gmail.com", Contrasena = HashPassword("limfer310") },
                new Administrador { AdministradorId = 4, Nombre = "Lilian Ramirez", Email = "lilianlizetteramirezchaparro@gmail.com", Contrasena = HashPassword("Michael2009!") },
                new Administrador { AdministradorId = 5, Nombre = "Enok Lobato", Email = "enokvoca13@gmail.com", Contrasena = HashPassword("Enok2007") }
            );


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        // Función para hashear contraseñas (la misma que en HomeController)
        // La puse aquí para que HasData pueda usarla.
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}