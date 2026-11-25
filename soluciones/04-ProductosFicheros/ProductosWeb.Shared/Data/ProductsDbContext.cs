using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

            // Las etiquetas son una colección en memoria pero una sola columna de texto en
            // SQLite. Esta conversión de valor es la que traduce entre las dos formas: al
            // guardar une las etiquetas con "oferta;recomendado" y al leer las vuelve a partir.
            // Los textos llevan la coma y el punto y coma escapados para que una etiqueta que
            // contenga ese carácter no rompa el formato.
            //
            // El comparador de valor es obligatorio en una colección con conversión: sin él,
            // EF Core no sabe si dos listas son la misma y avisa en el arranque. Compara
            // elemento a elemento y devuelve una copia al guardar la instantánea.
            var comparadorEtiquetas = new ValueComparer<List<string>>(
                (a, b) => a != null && b != null && a.SequenceEqual(b),
                lista => lista.Aggregate(0, (total, etiqueta) => HashCode.Combine(total, etiqueta.GetHashCode())),
                lista => lista.ToList());

            entity.Property(p => p.Etiquetas)
                .HasConversion(
                    lista => string.Join(';', lista.Select(Escape)),
                    texto => string.IsNullOrWhiteSpace(texto)
                        ? new List<string>()
                        : texto.Split(';').Select(Unescape).ToList())
                .Metadata.SetValueComparer(comparadorEtiquetas);

            // Índice único en el nombre: es la garantía de la base de datos de que no se
            // puede repetir, y el motivo de que CreateAsync devuelva null si ya existe.
            entity.HasIndex(p => p.Nombre).IsUnique();
        });
    }

    /// <summary>Escapa el separador y el punto y coma dentro de una etiqueta al guardarla.</summary>
    /// <param name="etiqueta">Texto de la etiqueta.</param>
    /// <returns>Etiqueta lista para concatenar.</returns>
    private static string Escape(string etiqueta) =>
        etiqueta.Replace("\\", "\\\\").Replace(";", "\\s").Replace(",", "\\c");

    /// <summary>
    /// Invierte <see cref="Escape"/> al leer una etiqueta de la base de datos.
    /// Se recorre el texto una sola vez: si se encadenaran <c>Replace</c>, un texto que ya
    /// contenía <c>\s</c> literal volvería a coincidir con el patrón siguiente y saldría mal.
    /// </summary>
    /// <param name="etiqueta">Texto guardado.</param>
    /// <returns>Etiqueta con sus caracteres originales.</returns>
    private static string Unescape(string etiqueta)
    {
        var salida = new StringBuilder(etiqueta.Length);

        for (var i = 0; i < etiqueta.Length; i++)
        {
            if (etiqueta[i] == '\\' && i + 1 < etiqueta.Length)
            {
                var siguiente = etiqueta[i + 1];

                if (siguiente is '\\' or 's' or 'c')
                {
                    salida.Append(siguiente switch
                    {
                        '\\' => '\\',
                        's' => ';',
                        _ => ','
                    });

                    i++;
                    continue;
                }
            }

            salida.Append(etiqueta[i]);
        }

        return salida.ToString();
    }
}
