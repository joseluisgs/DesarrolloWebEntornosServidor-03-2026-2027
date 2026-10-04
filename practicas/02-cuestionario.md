# Cuestionario de Investigación y Desarrollo (I+D): Desarrollo de Páginas Web Dinámicas en .NET

**Instrucciones:** Responde cada pregunta de forma clara y concisa. Puedes usar ejemplos de código si es necesario.

## PARTE 1 (Puntos 00-12)

1.  **Composición de una portada:** Tu tienda necesita una portada con barra de navegación, listado de destacados y pie de página reutilizable en todas las vistas. Explica qué piezas de Razor Pages usarías (layout, parciales, componentes de vista) y en qué orden las montarías.

2.  **Tag Helpers frente a HTML a mano:** Argumenta por qué un formulario escrito con Tag Helpers de ASP.NET Core es más mantenible que el mismo formulario en HTML puro, con un ejemplo concreto de atributo que el servidor rellena.

3.  **Decisión entre MVC y Razor Pages:** Un compañero defiende controladores para todo y otro defiende páginas para todo. Diseña los criterios de decisión que usarías para elegir la visión de una tienda real y justifica cada criterio.

4.  **Model binding de un objeto complejo:** Un formulario de alta envía `producto.Nombre`, `producto.Precio` y tres categorías en casillas. Explica cómo enlaza el servidor esos datos, qué prefijos usa y qué pasa si el precio llega con punto decimal en un equipo configurado en español.

5.  **Validación en servidor y cliente:** Compara la validación con atributos en la vista (cliente) y la misma validación en el PageModel o controlador (servidor). ¿Por qué la segunda es obligatoria y qué le pasa a un atacante que desactiva JavaScript?

6.  **El patrón PRG:** Describe qué ocurre si un formulario de alta envía el `POST` sin redirección y el usuario pulsa F5. Explica cómo lo resuelve PRG y qué papel juega TempData en el aviso de éxito.

7.  **Subida de ficheros segura:** Una tienda permite subir la foto de un funko. Enumera las comprobaciones que debe hacer el servidor (tipo, tamaño, nombre) y explica con qué defensa cortas un intento de path traversal.

8.  **Estado entre peticiones:** Necesitas que el número de visitas de la portada sobreviva entre peticiones y que un aviso de "producto añadido" solo se pinte una vez. ¿Qué mecanismo usas para cada cosa y por qué?

9.  **Cookies y sesiones por dentro:** Explica qué viaja realmente en la cookie `.AspNetCore.Session` y dónde viven los datos de la sesión. ¿Qué ocurre si cambias un carácter del valor de esa cookie?

10. **Identity en dos visiones:** Con `AddIdentity` configurado, describe el recorrido de una petición autenticada desde que el navegador manda la cookie hasta que la vista pinta `User.Identity?.Name`.

11. **Roles y políticas:** Una zona de tu tienda solo puede verla el rol `Admin` y otra requiere una política `PuedeEditar`. Explica la diferencia entre declarar un rol y declarar una política, y qué respuesta recibe un usuario identificado sin permiso.

12. **Configuración por entornos:** Explica el orden en que se superponen las fuentes de configuración (ficheros, secretos, variables de entorno) y por qué la misma clave puede tener valores distintos en desarrollo y en producción sin tocar el código.

13. **El patrón Infrastructure:** ¿Por qué se mueve el cableado de `Program.cs` a clases estáticas con métodos de extensión? Diseña los concerns que tendría la configuración de una tienda (datos, caché, compresión, localización) y qué registraría cada uno.

## PARTE 2 (Puntos 13-25)

14. **Caché de salida con criterio:** Quieres cachear el listado público de funkos pero no el panel de administración. Explica cómo declaras la caché de salida, por qué la zona privada no puede usarla y cómo invalidas el listado al dar de alta una figura nueva.

15. **Rendimiento medido:** Un compañero afirma que "añadir caché siempre mejora". Diseña la medición que demostraría o desmentiría su afirmación en tu listado, indicando qué herramientas usarías y qué compararías.

16. **Compresión con sentido:** Argumenta por qué tiene sentido comprimir el HTML de tu tienda y no tiene sentido comprimir sus imágenes. ¿Qué cabecera negocia el navegador para pedir la compresión?

17. **Localización completa:** Tu tienda necesita español e inglés. Explica qué vive en los ficheros `.resx`, cómo decide el servidor la cultura de cada petición y qué errores produces si un idioma no tiene fichero de recursos.

18. **Formatos culturales:** El mismo precio `0.21m` se pinta en una vista. ¿Qué muestra en `es-ES` y qué en `en-US`? Explica qué cultura usa el formateo y cómo evitas que la vista decida por su cuenta.

19. **Depuración de un fallo real:** Describe el recorrido que harías desde que un usuario dice "la página de altas no guarda nada" hasta encontrar la causa, usando punto de interrupción, pestaña Red y registro con `ILogger`.

20. **La página de error correcta:** ¿Por qué la misma excepción se cuenta de dos formas según el entorno? Explica qué ve el usuario en producción y qué ve el equipo en desarrollo, y qué riesgo tendría al revés.

21. **Pirámide de pruebas para tu tienda:** Distribuye las pruebas de tu tienda entre unitarias, de integración y de extremo a extremo con Playwright. Da un ejemplo concreto de cada nivel tomado de tu propio proyecto.

22. **Playwright y el token antifalsificación:** Explica qué le pasa a una prueba de flujo de alta si el formulario no lleva el token antifalsificación y cómo se resuelve en la vista.

23. **Publicación y contenedor:** Compara ejecutar tu aplicación con `dotnet run` en tu máquina y ejecutarla desde su imagen en un servidor. ¿Qué lleva la imagen que tu carpeta de desarrollo no necesita y por qué?

24. **Secretos en el flujo automatizado:** Tu flujo de GitHub Actions necesita publicar la imagen en un registro. Explica dónde viven las credenciales, qué pasaría si se escribieran en el YAML y qué ventaja tiene que las pruebas vayan antes que la publicación.

25. **Despliegue en un servicio gestionado:** Conectas tu repositorio a un servicio como Render. Explica qué hace el servicio por ti, dónde pones las variables de entorno y qué cambios de código exigen reconstruir la imagen y cuáles no.

---

**Instrucciones para el alumnado:**

Estas preguntas requieren **investigación, análisis y justificación técnica**. Se espera que:

1. **Investigues** en la documentación oficial de .NET, en los puntos de la unidad y en recursos técnicos.
2. **Comparen** diferentes enfoques (por ejemplo, caché de valores frente a caché de salida, MVC frente a Razor Pages).
3. **Justifiques** tus respuestas con argumentos técnicos sólidos y con ejemplos de tu propia tienda.
4. **Relaciones** los conceptos con principios de diseño (separación de capas, seguridad por defecto, medición antes de optimizar).

**Formato de respuesta esperado:**

- Respuestas de **desarrollo** (no solo definiciones).
- Incluir **ventajas/desventajas**, **casos de uso** y **ejemplos concretos**.
- Mencionar **compromisos** (trade-offs) cuando sea aplicable.
- Citar fuentes cuando sea necesario.
