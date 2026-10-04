- [00. Guía de supervivencia: comandos .NET CLI](#00-guía-de-supervivencia-comandos-net-cli)
  - [1. Crear proyectos y soluciones](#1-crear-proyectos-y-soluciones)
  - [2. Compilar y ejecutar](#2-compilar-y-ejecutar)
  - [3. Paquetes NuGet](#3-paquetes-nuget)
  - [4. .NET tools: herramientas globales](#4-net-tools-herramientas-globales)
  - [5. Scaffolding: generar código](#5-scaffolding-generar-código)
  - [6. Entity Framework Core](#6-entity-framework-core)
  - [7. Tests](#7-tests)
  - [8. Formateo de código](#8-formateo-de-código)
  - [9. User Secrets (secretos de desarrollo)](#9-user-secrets-secretos-de-desarrollo)
  - [10. Certificados de desarrollo](#10-certificados-de-desarrollo)
  - [11. Docker](#11-docker)
  - [12. Git](#12-git)



# 00. Guía de supervivencia: comandos .NET CLI

> 💡 **Punto de partida:** Esta guía recopila los comandos de `dotnet` que vas a necesitar para crear, compilar, ejecutar y desplegar las aplicaciones de esta unidad: Razor Pages, MVC, formularios, Identity y Docker. Guárdala como referencia rápida; el día del examen, esta página te saca de más de un aprieto.

En este punto aprenderás a usar la línea de comandos de .NET: crear proyectos y aplicaciones, compilar, ejecutar, observar en vivo y publicar, y resolver las tareas comunes de un proyecto sin salir de la terminal.

**Objetivos de aprendizaje:**

- Crear proyectos, soluciones y aplicaciones con la CLI de .NET 10
- Compilar, ejecutar, observar en vivo y publicar con los comandos esenciales
- Gestionar paquetes NuGet, herramientas globales y scaffolding
- Resolver tareas comunes: tests, formateo, secretos, certificados, Docker y Git

📌 **Ejemplo real:** Cuando en una práctica leas *"crea el proyecto de ProductosApp"*, el comando es `dotnet new webapp -n ProductosApp -f net10.0` (o `dotnet new mvc` si toca la visión MVC). Esta guía recopila todos esos comandos en un solo lugar.

## 1. Crear proyectos y soluciones

### Plantillas disponibles

```bash
# Ver todas las plantillas instaladas
dotnet new list

# Filtrar por familia web
dotnet new list web

# Buscar una plantilla concreta
dotnet new list razor
dotnet new list mvc

# Buscar en NuGet.org
dotnet new search razor
```

### Crear proyecto

```bash
# Razor Pages (la visión de páginas de esta unidad)
dotnet new webapp -n ProductosApp -f net10.0

# MVC (la visión de controladores y vistas)
dotnet new mvc -n ProductosAppMvc -f net10.0

# ASP.NET Core vacío (para pruebas rápidas)
dotnet new web -n MiPrueba -f net10.0

# Console App
dotnet new console -n MiConsola -f net10.0

# Class Library (biblioteca de clases)
dotnet new classlib -n MiLibreria -f net10.0

# Proyecto de tests NUnit
dotnet new nunit -n ProductosApp.Test -f net10.0
```

> 📝 **Nota:** `webapp` y `razor` son dos nombres para la misma plantilla de Razor Pages. En esta unidad se usa `webapp`, que es el nombre histórico.

### Crear solución

```bash
# Crear solución (en .NET 10 genera un .slnx, XML legible)
dotnet new sln -n ProductosApp

# Añadir proyectos a la solución
dotnet sln ProductosApp.slnx add ProductosApp/ProductosApp.csproj
dotnet sln ProductosApp.slnx add ProductosApp.Test/ProductosApp.Test.csproj

# Listar los proyectos de la solución
dotnet sln ProductosApp.slnx list

# Quitar un proyecto de la solución
dotnet sln ProductosApp.slnx remove ProductosApp.Test/ProductosApp.Test.csproj

# Migrar una solución antigua .sln al formato .slnx
dotnet sln migrate
```

## 2. Compilar y ejecutar

```bash
# Compilar (el primer gran comprobador de errores)
dotnet build

# Compilar en Release
dotnet build --configuration Release

# Ejecutar
dotnet run

# Ejecutar un proyecto concreto (imprescindible con varios proyectos)
dotnet run --project ProductosApp

# Ejecutar en un puerto concreto
dotnet run --urls "http://localhost:5000"

# Ejecutar sin recompilar (si ya está compilado)
dotnet run --no-build

# Desarrollo con recarga en vivo: guardas y la app se actualiza
dotnet watch

# Recarga en vivo de un proyecto concreto
dotnet watch --project ProductosApp

# Publicar para producción
dotnet publish -c Release -o ./publish

# Publicar autocontenido (sin instalar .NET en el servidor)
dotnet publish -c Release --self-contained -r linux-x64 -o ./publish

# Limpiar las carpetas de compilación (bin/ y obj/)
dotnet clean

# Restaurar los paquetes NuGet
dotnet restore

# Ejecutar un archivo .cs suelto (scripting de C# en .NET 10)
dotnet run archivo.cs
```

> 💡 **Consejo:** en cuanto la solución tenga más de un proyecto, `dotnet run` necesita `--project`; si no, te preguntará cuál quieres arrancar. Y antes de un `git commit`, borra `bin/` y `obj/` con `dotnet clean`.

## 3. Paquetes NuGet

```bash
# Añadir un paquete
dotnet add package FluentValidation

# Añadir una versión concreta
dotnet add package NUnit --version 4.3.2

# Eliminar un paquete
dotnet remove package FluentValidation

# Listar los paquetes del proyecto
dotnet list package

# Paquetes con vulnerabilidades conocidas
dotnet list package --vulnerable

# Paquetes desactualizados
dotnet list package --outdated
```

### Paquetes habituales de esta unidad

```bash
# Validación y seguridad (puntos 15 y 16)
dotnet add package FluentValidation

# Programación funcional: Result<T> (punto 15)
dotnet add package CSharpFunctionalExtensions

# Logging estructurado (punto 20)
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File

# Localización (punto 22)
dotnet add package Microsoft.Extensions.Localization
dotnet add package Microsoft.AspNetCore.Mvc.Localization

# Tests (punto 24)
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package NUnit.Analyzers
dotnet add package FluentAssertions
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package coverlet.collector
```

## 4. .NET tools: herramientas globales

```bash
# Listar las herramientas instaladas globalmente
dotnet tool list -g

# Instalar una herramienta global
dotnet tool install -g dotnet-ef
dotnet tool install -g dotnet-aspnet-codegenerator

# Actualizar una herramienta
dotnet tool update -g dotnet-ef

# Desinstalar una herramienta
dotnet tool uninstall -g dotnet-ef

# Herramientas locales del proyecto (se comparten por Git)
dotnet new tool-manifest
dotnet tool install --local dotnet-ef
dotnet tool list
```

## 5. Scaffolding: generar código

El scaffolding genera código a partir de tus modelos y tu contexto de datos. En esta unidad lo usarás sobre todo con Identity (punto 19):

```bash
# Instalar el generador (solo una vez)
dotnet tool install -g dotnet-aspnet-codegenerator

# Añadir el paquete de diseño al proyecto
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

# Generar las páginas de Identity (registro, login, logout...)
dotnet aspnet-codegenerator identity \
    -dc ProductosApp.Data.AppDbContext \
    --files "Account.Register;Account.Login;Account.Logout"
```

## 6. Entity Framework Core

Lo verás a fondo en la unidad de datos (UD02); aquí dejo los comandos que puedes necesitar al cruzarte con ella:

```bash
# Instalar la herramienta (solo una vez)
dotnet tool install -g dotnet-ef

# Crear una migración
dotnet ef migrations add InitialCreate

# Especificar proyecto y contexto
dotnet ef migrations add InitialCreate -p ProductosApp/ProductosApp.csproj -c AppDbContext

# Listar migraciones
dotnet ef migrations list

# Aplicar las migraciones a la base de datos
dotnet ef database update

# Generar el script SQL equivalente
dotnet ef migrations script -o script.sql
```

## 7. Tests

```bash
# Ejecutar todos los tests
dotnet test

# Con verbosidad
dotnet test --verbosity normal

# Un test concreto por nombre
dotnet test --filter "FullyQualifiedName~NombreDelTest"

# Los tests de un proyecto concreto
dotnet test ProductosApp.Test/ProductosApp.Test.csproj

# Cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

### E2E con Playwright (punto 24)

Los tests de extremo a extremo simulan a un usuario real con un navegador. Se escriben como tests NUnit y se ejecutan con los mismos comandos:

```bash
# Paquetes del navegador
dotnet add package Microsoft.Playwright
dotnet add package Microsoft.Playwright.NUnit

# Compilar (hace falta antes de instalar los navegadores)
dotnet build

# Instalar los navegadores desde la carpeta de salida
pwsh bin/Debug/net10.0/playwright.ps1 install

# Instalar solo un navegador concreto
pwsh bin/Debug/net10.0/playwright.ps1 install chromium

# Alternativa con la herramienta global
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium

# Ejecutar los tests E2E como cualquier otro test
dotnet test --filter "TestCategory=E2E"
```

> 📝 **Nota:** La instalación de navegadores se hace una sola vez por equipo; los binarios ocupan unos cientos de megas y quedan en tu perfil de usuario, no dentro del proyecto.

## 8. Formateo de código

```bash
# Formatear según el .editorconfig del proyecto
dotnet format

# Solo estilo de código
dotnet format style

# Comprobar sin modificar (útil en integración continua)
dotnet format --verify-no-changes
```

## 9. User Secrets (secretos de desarrollo)

```bash
# Inicializar los secretos en el proyecto (solo una vez)
dotnet user-secrets init

# Guardar un secreto
dotnet user-secrets set "Storage:UploadPath" "uploads"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=productosapp"

# Ver todos los secretos
dotnet user-secrets list

# Eliminar un secreto
dotnet user-secrets remove "Storage:UploadPath"
```

> 📝 **Nota:** Los user secrets solo viven en tu equipo de desarrollo y no se suben a Git. Son el sitio correcto para connection strings, claves y contraseñas mientras construyes la aplicación (punto 20).

## 10. Certificados de desarrollo

```bash
# Crear el certificado de desarrollo para HTTPS
dotnet dev-certs https

# Comprobar si ya existe
dotnet dev-certs https --check

# Limpiar los certificados
dotnet dev-certs https --clean
```

## 11. Docker

```bash
# Compilar la imagen
docker build -t productosapp .

# Ejecutar el contenedor
docker run -p 8080:8080 productosapp

# Con variable de entorno
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development productosapp

# Con un volumen (para conservar las subidas de ficheros)
docker run -p 8080:8080 -v uploads:/app/wwwroot/uploads productosapp

# Docker Compose
docker compose up -d              # Levantar
docker compose down               # Detener
docker compose ps                 # Estado
docker compose logs -f            # Logs en tiempo real
docker compose up -d --build      # Reconstruir y levantar
```

## 12. Git

```bash
# Estado del repositorio
git status
git status --short

# Añadir cambios
git add -A                  # Todo
git add 01-razor-fundamentos.md   # Un archivo concreto

# Commitear (los mensajes van en español)
git commit -m "feat: descripción del cambio"
git commit -m "fix: corrección"
git commit -m "docs: documentación"

# Subir y bajar
git push
git pull

# Historial
git log --oneline -10

# Diferencias
git diff
git diff --staged

# Ramas
git checkout -b feature/nueva-funcionalidad
git checkout main
git branch -d feature/nueva-funcionalidad
```

> 📝 **Nota:** Esta guía se irá ampliando con los comandos nuevos que aparezcan a medida que avancemos por la unidad.
