using FluentAssertions;
using NUnit.Framework;
using ProductosWeb.Shared.Models.Enums;
using ProductosWeb.Shared.ViewModels;
using ProductosWeb.Shared.Validators;

namespace ProductosWeb.Test.Validators;

/// <summary>
/// Tests del validador FluentValidation del formulario.
/// Se comprueba que las reglas que NO caben en DataAnnotations se apliquen:
/// longitudes mínimas, límite de etiquetas y validación de cada recargo.
/// </summary>
[TestFixture]
public class ProductFormValidatorTests
{
    private ProductFormValidator _validador = null!;

    [SetUp]
    public void SetUp() => _validador = new ProductFormValidator();

    /// <summary>
    /// Fórmula un formulario válido para partir de él y romper solo lo que interesa en cada test.
    /// </summary>
    /// <returns>Formulario que pasa todas las reglas.</returns>
    private static ProductFormViewModel FormularioValido() => new()
    {
        Nombre = "Smartphone Aurora X5",
        Descripcion = "Pantalla AMOLED de 6,5 pulgadas y batería de 5000 mAh.",
        Precio = 449.99m,
        Stock = 10,
        Categoria = ProductCategory.Smartphones
    };

    [Test]
    public void Validate_FormularioValido_SinErrores()
    {
        // Act
        var resultado = _validador.Validate(FormularioValido());

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_NombreDeUnCaracter_AñadeElErrorDeLongitudMinima()
    {
        // Arrange: DataAnnotations solo piden que no esté vacío; el mínimo de 3 es de FluentValidation.
        var formulario = FormularioValido();
        formulario.Nombre = "X";

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("al menos 3 caracteres"));
    }

    [Test]
    public void Validate_DescripcionCorta_AñadeElErrorDeLongitudMinima()
    {
        // Arrange
        var formulario = FormularioValido();
        formulario.Descripcion = "corta";

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("al menos 10 caracteres"));
    }

    [Test]
    public void Validate_PrecioCero_AñadeElErrorDeMayorQueCero()
    {
        // Arrange
        var formulario = FormularioValido();
        formulario.Precio = 0;

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("mayor que cero"));
    }

    [Test]
    public void Validate_MasDeDiezEtiquetas_AñadeElErrorDeLimite()
    {
        // Arrange: el formulario puede añadir etiquetas con JavaScript, y el límite se comprueba aquí.
        var formulario = FormularioValido();
        formulario.Etiquetas.AddRange(Enumerable.Range(1, 11).Select(i => $"etiqueta-{i}"));

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("más de 10 etiquetas"));
    }

    [Test]
    public void Validate_EtiquetaVacia_AñadeElErrorEnElIndice()
    {
        // Arrange
        var formulario = FormularioValido();
        formulario.Etiquetas.Add("   ");

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Etiquetas[0]");
    }

    [Test]
    public void Validate_RecargoSinConcepto_AñadeElErrorEnElIndice()
    {
        // Arrange: los recargos se validan con su propio validador, anidado con SetValidator.
        var formulario = FormularioValido();
        formulario.Recargos.Add(new RecargoViewModel { Concepto = "", Importe = 5 });

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Recargos[0].Concepto");
    }

    [Test]
    public void Validate_RecargoConImporteCero_AñadeElErrorEnElIndice()
    {
        // Arrange
        var formulario = FormularioValido();
        formulario.Recargos.Add(new RecargoViewModel { Concepto = "Envío urgente", Importe = 0 });

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Recargos[0].Importe");
    }

    [Test]
    public void Validate_DosRecargosValidos_NoAñadeErrores()
    {
        // Arrange
        var formulario = FormularioValido();
        formulario.Recargos.Add(new RecargoViewModel { Concepto = "Envío urgente", Importe = 4.95m });
        formulario.Recargos.Add(new RecargoViewModel { Concepto = "Grabado", Importe = 12m });

        // Act
        var resultado = _validador.Validate(formulario);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }
}
