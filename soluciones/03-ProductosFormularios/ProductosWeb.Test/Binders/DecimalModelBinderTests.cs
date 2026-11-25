using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using ProductosWeb.Shared.Binders;

namespace ProductosWeb.Test.Binders;

/// <summary>
/// Tests del model binding propio de decimales.
/// Se monta a mano el contexto que el binder recibe en ejecución, con un value provider
/// de prueba: así se comprueba el comportamiento sin arrancar el servidor.
/// </summary>
[TestFixture]
public class DecimalModelBinderTests
{
    /// <summary>
    /// Value provider mínimo para los tests: devuelve el valor fijado en la prueba o None.
    /// Se escribe a mano en lugar de usar uno del framework para que se vea la interfaz completa.
    /// </summary>
    /// <param name="valores">Pares nombre de campo y texto enviado por el formulario.</param>
    private sealed class ValueProviderDePrueba(Dictionary<string, string> valores) : IValueProvider
    {
        /// <inheritdoc />
        public bool ContainsPrefix(string prefix) => valores.ContainsKey(prefix);

        /// <inheritdoc />
        public ValueProviderResult GetValue(string key) =>
            valores.TryGetValue(key, out var valor)
                ? new ValueProviderResult(valor)
                : ValueProviderResult.None;
    }

    /// <summary>
    /// Prepara el contexto de binding con el valor que el navegador habría enviado.
    /// </summary>
    /// <param name="valor">Texto tal y como llega del formulario (o null si no llega nada).</param>
    /// <returns>Contexto listo para que el binder haga su trabajo.</returns>
    private static DefaultModelBindingContext CrearContexto(string? valor)
    {
        var proveedor = new EmptyModelMetadataProvider();
        var metadata = proveedor.GetMetadataForType(typeof(decimal));

        var valores = new Dictionary<string, string>();
        if (valor is not null)
        {
            valores["precio"] = valor;
        }

        return new DefaultModelBindingContext
        {
            ModelMetadata = metadata,
            ModelName = "precio",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new ValueProviderDePrueba(valores)
        };
    }

    [TestCase("12,50", 12.50)]
    [TestCase("12.50", 12.50)]
    [TestCase("1234,56", 1234.56)]
    [TestCase("1234.56", 1234.56)]
    [TestCase("0,01", 0.01)]
    public async Task BindModel_ConComaOPunto_DevuelveElDecimal(string valor, decimal esperado)
    {
        // Arrange
        var contexto = CrearContexto(valor);
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.Result.IsModelSet.Should().BeTrue();
        contexto.Result.Model.Should().Be(esperado);
    }

    [Test]
    public async Task BindModel_ConComa_EsElMismoNumeroQueConPunto()
    {
        // Arrange
        var conComa = CrearContexto("99,99");
        var conPunto = CrearContexto("99.99");
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(conComa);
        await binder.BindModelAsync(conPunto);

        // Assert
        conComa.Result.Model.Should().Be(conPunto.Result.Model);
    }

    [Test]
    public async Task BindModel_SinValor_NoMarcaElModelo()
    {
        // Arrange
        var contexto = CrearContexto(null);
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.Result.IsModelSet.Should().BeFalse();
    }

    [Test]
    public async Task BindModel_ValorVacio_NoMarcaElModelo()
    {
        // Arrange
        var contexto = CrearContexto("   ");
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.Result.IsModelSet.Should().BeFalse();
    }

    [Test]
    public async Task BindModel_ValorNoNumerico_AñadeErrorAlModelState()
    {
        // Arrange
        var contexto = CrearContexto("no-es-un-precio");
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.Result.IsModelSet.Should().BeFalse();
        contexto.ModelState.IsValid.Should().BeFalse();
        contexto.ModelState["precio"]!.Errors.Should().ContainSingle();
    }

    [Test]
    public async Task BindModel_ValorInvalido_GuardaElValorOriginalEnElModelState()
    {
        // Arrange: el valor original debe conservarse para poder repintar el formulario.
        var contexto = CrearContexto("también-malo");
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.ModelState["precio"]!.AttemptedValue.Should().Be("también-malo");
    }

    [Test]
    public void Provider_SoloTrataDecimales()
    {
        // Arrange
        var proveedor = new EmptyModelMetadataProvider();
        var provider = new DecimalModelBinderProvider();

        // Act: ModelBinderProviderContext es abstracto, así que se simula con Moq.
        var paraDecimal = provider.GetBinder(Para(typeof(decimal), proveedor));
        var paraDecimalAnulable = provider.GetBinder(Para(typeof(decimal?), proveedor));
        var paraString = provider.GetBinder(Para(typeof(string), proveedor));

        // Assert
        paraDecimal.Should().BeOfType<DecimalModelBinder>();
        paraDecimalAnulable.Should().BeOfType<DecimalModelBinder>();
        paraString.Should().BeNull();
    }

    [Test]
    public async Task BindModel_DecimalAnulable_ConValor_DevuelveElNumero()
    {
        // Arrange
        var proveedor = new EmptyModelMetadataProvider();
        var contexto = new DefaultModelBindingContext
        {
            ModelMetadata = proveedor.GetMetadataForType(typeof(decimal?)),
            ModelName = "precio",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new ValueProviderDePrueba(new Dictionary<string, string> { ["precio"] = "7,25" })
        };
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.Result.Model.Should().Be(7.25m);
    }

    [Test]
    public async Task BindModel_CampoDistinto_NoTocaElModelo()
    {
        // Arrange: el provider no trae la clave "precio", así que no hay nada que enlazar.
        var proveedor = new EmptyModelMetadataProvider();
        var contexto = new DefaultModelBindingContext
        {
            ModelMetadata = proveedor.GetMetadataForType(typeof(decimal)),
            ModelName = "precio",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new ValueProviderDePrueba(new Dictionary<string, string> { ["otra"] = "1" })
        };
        var binder = new DecimalModelBinder();

        // Act
        await binder.BindModelAsync(contexto);

        // Assert
        contexto.ModelState.IsValid.Should().BeTrue();
        contexto.Result.IsModelSet.Should().BeFalse();
    }

    /// <summary>
    /// Crea el contexto simulado que el provider necesita solo para leer el tipo del modelo.
    /// </summary>
    /// <param name="tipo">Tipo del modelo a comprobar.</param>
    /// <param name="proveedorMetadata">Proveedor de metadatos vacío.</param>
    /// <returns>Contexto simulado con ese metadato.</returns>
    private static ModelBinderProviderContext Para(Type tipo, IModelMetadataProvider proveedorMetadata)
    {
        var simulado = new Mock<ModelBinderProviderContext>();
        simulado.Setup(c => c.Metadata).Returns(proveedorMetadata.GetMetadataForType(tipo));
        return simulado.Object;
    }

    [Test]
    public async Task BindModel_ConCulturaDistinta_SigueLeyendoLaComa()
    {
        // Arrange: aunque el hilo esté en una cultura anglosajona, la coma española debe valer.
        var culturaOriginal = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        try
        {
            var contexto = CrearContexto("1234,56");
            var binder = new DecimalModelBinder();

            // Act
            await binder.BindModelAsync(contexto);

            // Assert
            contexto.Result.Model.Should().Be(1234.56m);
        }
        finally
        {
            CultureInfo.CurrentCulture = culturaOriginal;
        }
    }
}
