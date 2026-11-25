using FluentValidation;
using ProductosWeb.Shared.ViewModels;

namespace ProductosWeb.Shared.Validators;

/// <summary>
/// Validador del formulario de Binding avanzado.
/// La cabecera se valida con el mismo validador del producto (lleva dentro, así que se anida
/// con SetValidator) y las colecciones con sus propias reglas: cada fila, a su índice.
/// </summary>
public class BindingFormValidator : AbstractValidator<BindingFormViewModel>
{
    /// <summary>
    /// Reglas del formulario de Binding: cabecera anidada, etiquetas sueltas y filas de recargos.
    /// </summary>
    public BindingFormValidator()
    {
        // Cabecera: aquí llegan los errores con el prefijo Cabecera.Nombre, Cabecera.Precio...
        RuleFor(x => x.Cabecera).SetValidator(new ProductFormValidator());

        // Regla de conjunto sobre la colección, igual que en el formulario de producto.
        RuleFor(x => x.Etiquetas)
            .Must(etiquetas => etiquetas is null || etiquetas.Count <= 10)
            .WithMessage("No se pueden mandar más de 10 etiquetas");

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
