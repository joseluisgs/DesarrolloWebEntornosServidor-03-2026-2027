using FluentAssertions;
using NUnit.Framework;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Validators;

/// <summary>
/// Tests del validador del formulario de Binding avanzado.
/// Comprueba que la cabecera anidada se valide con las mismas reglas que el producto
/// y que las colecciones lo hagan por índice.
/// </summary>
[TestFixture]
public class BindingFormValidatorTests
{
    private BindingFormValidator _validador = null!;

    [SetUp]
    public void SetUp() => _validador = new BindingFormValidator();

    /// <summary>
    /// Fórmula un formulario de Binding válido para partir de él en cada prueba.
    /// </summary>
    /// <returns>Formulario que pasa todas las reglas.</returns>
    private static BindingFormViewModel BindingValido() => new()
    {
        Cabecera = new ProductFormViewModel
        {
            Nombre = "Smartphone Aurora X5",
            Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
            Precio = 449.99m,
            Stock = 10,
            Categoria = ProductCategory.Smartphones
        },
        Etiquetas = ["oferta"],
        Recargos = [new RecargoViewModel { Concepto = "Envío urgente", Importe = 4.95m }]
    };

    [Test]
    public void Validate_BindingValido_SinErrores()
    {
        // Act
        var resultado = _validador.Validate(BindingValido());

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_CabeceraSinNombre_AñadeElErrorConElPrefijo()
    {
        // Arrange: el error aterriza en Cabecera.Nombre, con el prefijo del modelo anidado.
        var binding = BindingValido();
        binding.Cabecera.Nombre = "X";

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Cabecera.Nombre");
    }

    [Test]
    public void Validate_CabeceraConPrecioCero_AñadeElErrorEnLaCabecera()
    {
        // Arrange
        var binding = BindingValido();
        binding.Cabecera.Precio = 0;

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Cabecera.Precio");
    }

    [Test]
    public void Validate_EtiquetaVacia_AñadeElErrorEnSuIndice()
    {
        // Arrange
        var binding = BindingValido();
        binding.Etiquetas.Add("   ");

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Etiquetas[1]");
    }

    [Test]
    public void Validate_MasDeDiezEtiquetas_AñadeElErrorDeLimite()
    {
        // Arrange
        var binding = BindingValido();
        binding.Etiquetas.AddRange(Enumerable.Range(1, 11).Select(i => $"etiqueta-{i}"));

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("más de 10 etiquetas"));
    }

    [Test]
    public void Validate_RecargoConImporteCero_AñadeElErrorEnSuIndice()
    {
        // Arrange
        var binding = BindingValido();
        binding.Recargos.Add(new RecargoViewModel { Concepto = "Grabado", Importe = 0 });

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Recargos[1].Importe");
    }

    [Test]
    public void Validate_DosRecargosValidos_NoAñadeErrores()
    {
        // Arrange
        var binding = BindingValido();
        binding.Recargos.Add(new RecargoViewModel { Concepto = "Grabado", Importe = 12m });

        // Act
        var resultado = _validador.Validate(binding);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }
}
