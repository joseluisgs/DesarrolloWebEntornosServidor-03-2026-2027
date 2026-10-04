# Desarrollo Web en Entorno Servidor - 03 - Desarrollo de páginas web dinámicas en .NET

UD03. Desarrollo de páginas web dinámicas en .NET. 2DAW. Curso 2026-2027

![imagen](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-00-2023-2024/raw/master/images/servicios.png)

- [Desarrollo web en entorno servidor - 03 - desarrollo de páginas web dinámicas en .NET](#desarrollo-web-en-entorno-servidor---03---desarrollo-de-páginas-web-dinámicas-en-net)
  - [Contenido](#contenido)
  - [Proyecto integrador](#proyecto-integrador)
  - [Contenido en YouTube](#contenido-en-youtube)
  - [Resultados de aprendizaje y criterios de evaluación](#resultados-de-aprendizaje-y-criterios-de-evaluación)
  - [Autor](#autor)
    - [Contacto](#contacto)
  - [Licencia de uso](#licencia-de-uso)


## Contenido

0.  [Guía de supervivencia: comandos .NET CLI](00-comandos.md)
1.  [Fundamentos de Razor: páginas dinámicas con código embebido](01-razor-fundamentos.md)
2.  [Directivas y sintaxis de Razor](02-directivas-sintaxis.md)
3.  [Estructuras de control en la interfaz](03-estructuras-control.md)
4.  [Funciones y métodos en las vistas](04-funciones-metodos.md)
5.  [Layouts, partials y componentes de vista](05-layouts-partials-componentes.md)
6.  [Tag Helpers: controles en el servidor](06-taghelpers.md)
7.  [Arquitectura MVC: separación de presentación y negocio](07-mvc-arquitectura.md)
8.  [Controladores y vistas en MVC](08-mvc-controladores.md)
9.  [ViewModels y programación orientada a objetos](09-mvc-vistas-viewmodels.md)
10. [Razor Pages: fundamentos](10-razorpages-fundamentos.md)
11. [Razor Pages: PageModel y handlers](11-razorpages-pagemodel-handlers.md)
12. [MVC vs Razor Pages: comparativa y migración](12-mvc-vs-razorpages.md)
13. [Formularios web y generación dinámica](13-formularios-web.md)
14. [Model Binding: del formulario al servidor](14-model-binding.md)
15. [Validaciones, seguridad y ataques](15-validaciones-seguridad.md)
16. [Subida y almacenamiento de ficheros](16-ficheros-almacenamiento.md)
17. [Gestión del estado de la aplicación](17-estado-app.md)
18. [Cookies, sesiones y almacenamiento en el cliente](18-cookies-sesiones-cliente.md)
19. [Autenticación de usuarios con ASP.NET Core Identity](19-autenticacion-identity.md)
20. [Configuración de la aplicación web](20-configuracion-entornos.md)
21. [Optimización y rendimiento](21-optimizacion-rendimiento.md)
22. [Internacionalización (I18n) y localización](22-i18n-localizacion.md)
23. [Herramientas de programación, prueba y depuración](23-herramientas-debug.md)
24. [Pruebas y documentación del código de presentación](24-testing-documentacion.md)
25. [Despliegue con Docker y nube](25-docker-despliegue.md)
26. [Resumen](26-resumen.md)

## Proyecto integrador
Los proyectos realizados en clase:
- [Proyecto integrador web dinámica](https://github.com/joseluisgs/TiendaDawWeb-NetCore)

## Contenido en YouTube
- [Resumen]()
- [Lista de reproducción](https://www.youtube.com/playlist?list=PLLiuVpAc3Gv4)

## Resultados de aprendizaje y criterios de evaluación

- RA2: Escribe sentencias ejecutables por un servidor web reconociendo y aplicando procedimientos de integración del código en lenguajes de marcas.

    - CCEE:
        - a) Se han reconocido los mecanismos de generación de páginas web a partir de lenguajes de marcas con código embebido.
        - b) Se han identificado las principales tecnologías asociadas.
        - c) Se han utilizado etiquetas para la inclusión de código en el lenguaje de marcas.
        - d) Se ha reconocido la sintaxis del lenguaje de programación que se ha de utilizar.
        - e) Se han escrito sentencias simples y se han comprobado sus efectos en el documento resultante.
        - f) Se han utilizado directivas para modificar el comportamiento predeterminado.
        - g) Se han utilizado los distintos tipos de variables y operadores disponibles en el lenguaje.
        - h) Se han identificado los ámbitos de utilización de las variables.

- RA3: Escribe bloques de sentencias embebidos en lenguajes de marcas, seleccionando y utilizando las estructuras de programación.

    - CCEE:
        - a) Se han utilizado mecanismos de decisión en la creación de bloques de sentencias.
        - b) Se han utilizado bucles y se ha verificado su funcionamiento.
        - c) Se han utilizado matrices (arrays) para almacenar y recuperar conjuntos de datos.
        - d) Se han creado y utilizado funciones.
        - e) Se han utilizado formularios web para interactuar con el usuario del navegador web.
        - f) Se han empleado métodos para recuperar la información introducida en el formulario.
        - g) Se han añadido comentarios al código.

- RA4: Desarrolla aplicaciones web embebidas en lenguajes de marcas analizando e incorporando funcionalidades según especificaciones.

    - CCEE:
        - a) Se han identificado los mecanismos disponibles para el mantenimiento de la información que concierne a un cliente web concreto y se han señalado sus ventajas.
        - b) Se han utilizado mecanismos para mantener el estado de las aplicaciones web.
        - c) Se han utilizado mecanismos para almacenar información en el cliente web y para recuperar su contenido.
        - d) Se han identificado y caracterizado los mecanismos disponibles para la autentificación de usuarios.
        - e) Se han escrito aplicaciones que integren mecanismos de autentificación de usuarios.
        - f) Se han utilizado herramientas y entornos para facilitar la programación, prueba y depuración del código.

- RA5: Desarrolla aplicaciones web identificando y aplicando mecanismos para separar el código de presentación de la lógica de negocio

    - CCEE:
        - a) Se han identificado las ventajas de separar la lógica de negocio de los aspectos de presentación de la aplicación.
        - b) Se han analizado y utilizado mecanismos y frameworks que permiten realizar esta separación y sus características principales.
        - c) Se han utilizado objetos y controles en el servidor para generar el aspecto visual de la aplicación web en el cliente.
        - d) Se han utilizado formularios generados de forma dinámica para responder a los eventos de la aplicación web.
        - e) Se han identificado y aplicado los parámetros relativos a la configuración de la aplicación web.
        - f) Se han escrito aplicaciones web con mantenimiento de estado y separación de la lógica de negocio.
        - g) Se han aplicado los principios y patrones de diseño de la programación orientada a objetos.
        - h) Se ha probado y documentado el código.



## Autor

Codificado con :sparkling_heart: por [José Luis González Sánchez](https://joseluisgs.dev)

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://x.com/JoseLuisGSDev)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)
[![GitHub](https://img.shields.io/github/stars/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto

<p>
  Cualquier cosa que necesites házmelo saber por si puedo ayudarte 💬.
</p>
<p>
    <a href="https://joseluisgs.dev/" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/favicon.png" height="32">
    </a>&nbsp;
    <a href="https://github.com/joseluisgs" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/github.svg" height="32">
    </a>&nbsp;
    <a href="https://www.linkedin.com/in/JoseLuisGSDev" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/linkedin.png" height="32">
    </a>&nbsp;
    <a href="https://www.youtube.com/@joseluisgs" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/youtube.png" height="32">
    </a>&nbsp;
    <a href="https://x.com/JoseLuisGSDev" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/twitter.png" height="32">
    </a>&nbsp;
    <a href="https://www.instagram.com/joseluisgs.dev/" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/instagram.png" height="32">
    </a>
</p>

## Licencia de uso

Este repositorio y todo su contenido está licenciado bajo licencia **Creative Commons**, si desea saber más, vea
la [LICENSE](https://joseluisgs.dev/docs/license/). Por favor si compartes, usas o modificas este proyecto cita a su
autor, y usa las mismas condiciones para su uso docente, formativo o educativo y no comercial.

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/"><img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">
JoseLuisGS</span> by <a xmlns:cc="http://creativecommons.org/ns#" href="https://joseluisgs.dev/" property="cc:attributionName" rel="cc:attributionURL">
José Luis González Sánchez</a> is licensed under
<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons
Reconocimiento-NoComercial-CompartirIgual 4.0 Internacional License</a>.<br />Creado a partir de la obra
en <a xmlns:dct="http://purl.org/dc/terms/" href="https://github.com/joseluisgs" rel="dct:source">https://github.com/joseluisgs</a>.
