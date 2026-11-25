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
    /// Reglas del formulario de producto: propiedades sueltas y reglas que cruzan varias.
    /// El fichero subido no se valida aquí; lo decide el servicio de almacenamiento.
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

        // El año no es un dato libre: el catálogo cubre el rango que la tienda securiza.
        RuleFor(x => x.Anio)
            .InclusiveBetween(2000, 2030).WithMessage("El año debe estar entre 2000 y 2030");
    }
}
