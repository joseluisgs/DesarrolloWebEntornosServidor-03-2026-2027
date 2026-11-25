using FluentValidation;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Validators;

/// <summary>
/// Validador FluentValidation del formulario de producto.
/// Es la capa complementaria de los DataAnnotations: lo que se puede expresar en una sola
/// propiedad ya está en el ViewModel (obligatoriedad y longitudes); aquí van las reglas que
/// cruzan varios campos o las que se leen mejor en lenguaje de validación.
/// </summary>
public class ProductFormValidator : AbstractValidator<ProductFormViewModel>
{
    /// <summary>
    /// Reglas del formulario de producto. El SetValidator anidado valida cada recargo con su propio validador.
    /// </summary>
    public ProductFormValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria")
            .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero");

        // Regla de conjunto: el formulario puede añadir etiquetas con JavaScript,
        // y el límite se comprueba aquí, no en la propiedad.
        RuleFor(x => x.Etiquetas)
            .Must(etiquetas => etiquetas is null || etiquetas.Count <= 10)
            .WithMessage("Un producto no puede llevar más de 10 etiquetas");

        RuleForEach(x => x.Etiquetas)
            .ChildRules(etiqueta =>
            {
                etiqueta.RuleFor(e => e)
                    .NotEmpty().WithMessage("La etiqueta no puede estar vacía")
                    .MaximumLength(30).WithMessage("La etiqueta no puede superar los 30 caracteres");
            });

        RuleForEach(x => x.Recargos).SetValidator(new RecargoViewModelValidator());
    }
}

/// <summary>
/// Validador de cada fila de recargos del formulario.
/// Va en su fichero normalmente; aquí convive con el del producto para que el ejemplo quepa en una unidad.
/// </summary>
public class RecargoViewModelValidator : AbstractValidator<RecargoViewModel>
{
    /// <summary>
    /// Reglas de un recargo: concepto con texto y importe positivo.
    /// </summary>
    public RecargoViewModelValidator()
    {
        RuleFor(x => x.Concepto)
            .NotEmpty().WithMessage("El concepto del recargo es obligatorio")
            .MaximumLength(100).WithMessage("El concepto no puede superar los 100 caracteres");

        RuleFor(x => x.Importe)
            .GreaterThan(0).WithMessage("El importe del recargo debe ser mayor que cero");
    }
}
