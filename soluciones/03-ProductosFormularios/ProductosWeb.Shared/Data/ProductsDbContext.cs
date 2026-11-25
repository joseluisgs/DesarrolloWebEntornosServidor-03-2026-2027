using Microsoft.EntityFrameworkCore;
using ProductosWeb.Shared.Models;

namespace ProductosWeb.Shared.Data;

/// <summary>
/// Contexto de EF Core contra SQLite.
/// Vive en Data/, que es donde va el DbContext; los servicios lo reciben
/// por inyección de dependencias y nunca crean uno nuevo a mano.
/// </summary>
/// <param name="options">Opciones del contexto (las monta DatabaseConfig).</param>
public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    /// <summary>Productos del catálogo.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Descripcion).HasMaxLength(1000).IsRequired();

            // Índice único en el nombre: es la garantía de la base de datos de que no se
            // puede repetir, y el motivo de que CreateAsync devuelva null si ya existe.
            entity.HasIndex(p => p.Nombre).IsUnique();
        });
    }
}
