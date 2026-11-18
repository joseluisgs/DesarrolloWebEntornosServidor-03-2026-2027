using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace ProductosRazor.Test.Helpers;

/// <summary>
/// Fábrica del contexto mínimo que un PageModel necesita para existir fuera del servidor.
/// El framework, en una petición real, le daría el PageContext (con su ViewData), el TempData
/// y el enrutado; en los tests se los montamos a mano para llamar a los handlers directamente.
/// </summary>
internal static class PageModelHelper
{
    /// <summary>
    /// Devuelve el PageModel recibido, ya con PageContext, ViewData y TempData asignados.
    /// </summary>
    /// <param name="pageModel">PageModel a preparar.</param>
    /// <typeparam name="T">Tipo del PageModel.</typeparam>
    /// <returns>El mismo PageModel, listo para ser probado.</returns>
    public static T Preparar<T>(T pageModel) where T : PageModel
    {
        var contexto = new PageContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()));

        // ViewData cuelga del PageContext: sin esto, escribir en ViewData desde el
        // handler revienta con NullReferenceException fuera del servidor.
        contexto.ViewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(), new ModelStateDictionary());

        pageModel.PageContext = contexto;

        pageModel.TempData = new TempDataDictionary(
            new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        return pageModel;
    }
}
