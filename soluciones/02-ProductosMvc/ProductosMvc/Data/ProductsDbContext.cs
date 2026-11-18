using Microsoft.EntityFrameworkCore;
using ProductosMvc.Models;

namespace ProductosMvc.Data;

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

            // Dos decimales justos: el precio no admite centavos fraccionados
            entity.Property(p => p.Precio).HasPrecision(10, 2);

            // El enum se guarda como texto ("Smartphones"), legible al abrir el .db
            entity.Property(p => p.Categoria).HasConversion<string>().HasMaxLength(40);

            entity.HasIndex(p => p.Nombre).IsUnique();
        });
    }
}
