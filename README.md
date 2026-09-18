# GestionProductosMVC

Aplicación web ASP.NET Core MVC (.NET 10) para la gestión/visualización de un catálogo de productos, desarrollada como actividad académica. Como fuente de datos se utiliza la [PokeAPI](https://pokeapi.co/docs/v2) (API pública, de solo lectura): cada Pokémon actúa como "producto" del catálogo para efectos de esta actividad.

## Funcionalidad

- **Catálogo (`/Pokemon`)**: lista de productos con imagen, nombre y tipo(s), obtenidos desde `GET /pokemon`.
- **Detalle (`/Pokemon/Details/{idOrName}`)**: ficha de un producto con imagen, altura, peso, habilidades y estadísticas base, obtenidos desde `GET /pokemon/{idOrName}`.

## Arquitectura y patrones de diseño

- **MVC (Model-View-Controller)**: separación entre `Models/`, `Views/` y `Controllers/`.
- **Service Layer + Inversión de Dependencias**: `IPokemonService` / `PokemonService` encapsula el acceso a la API externa. El controlador depende de la interfaz, no de la implementación, y ASP.NET Core la inyecta por constructor (Dependency Injection nativa).
- **Typed HttpClient** (`AddHttpClient<IPokemonService, PokemonService>`): gestiona el ciclo de vida de `HttpClient` de forma segura (evita el agotamiento de sockets) y centraliza la configuración de base address y timeout.
- **Options Pattern** (`PokeApiSettings` + `IOptions<T>`): la URL base de la API y el tamaño de página del catálogo se configuran en `appsettings.json`, no están *hardcodeadas* en el código.
- **DTO / Anti-Corruption Layer**: las clases internas en `Services/PokeApiDtos.cs` (`internal sealed`) mapean el JSON crudo de la PokeAPI. El resto de la aplicación (controladores, vistas) solo conoce los modelos de dominio propios (`Models/Pokemon.cs`), evitando que el esquema de un tercero se filtre a todo el sistema.
- **Excepción de dominio propia** (`PokeApiException`): los errores de red/deserialización se traducen a una excepción propia con mensaje seguro para el usuario, sin exponer detalles internos de la API externa.

## Buenas prácticas de seguridad aplicadas

- **HTTPS y HSTS** habilitados (`UseHttpsRedirection`, `UseHsts` en producción).
- **Manejador de errores centralizado** (`UseExceptionHandler("/Home/Error")`) que evita filtrar *stack traces* en producción; los errores esperables de la API externa se capturan aparte y se muestran como mensaje amigable (sin detalles técnicos) en la propia vista.
- **Timeout explícito** en el `HttpClient` (10s) para evitar que una API externa lenta o caída deje solicitudes colgadas indefinidamente (protección básica ante denegación de servicio por dependencia externa).
- **Razor con auto-encoding**: todos los valores que vienen de la API externa (nombre, tipos, habilidades) se renderizan con `@Model...`, que Razor escapa automáticamente, mitigando XSS reflejado desde datos de terceros.
- **Sin secretos en el repositorio**: no se manejan credenciales (la PokeAPI es pública y no requiere API key); de todas formas se agregó `.gitignore` para excluir configuraciones locales (`appsettings.*.local.json`) por si se agregan en el futuro.
- **Validación de entrada básica**: el controlador valida que el parámetro `id` no sea nulo/vacío antes de consultarlo, y responde `404` si el producto no existe en lugar de propagar errores.
- **`.gitignore`** para no versionar binarios de compilación (`bin/`, `obj/`) ni configuración de IDE (`.vs/`).

## Estructura del proyecto

```
Controllers/
  HomeController.cs
  PokemonController.cs
Models/
  Pokemon.cs            (PokemonSummary, PokemonDetail, PokemonStat)
  PokeApiSettings.cs
  ErrorViewModel.cs
Services/
  IPokemonService.cs
  PokemonService.cs
  PokeApiDtos.cs         (DTOs internos de deserialización)
  PokeApiException.cs
Views/
  Pokemon/
    Index.cshtml
    Details.cshtml
```

## Cómo ejecutar

```bash
dotnet restore
dotnet run
```

La aplicación queda disponible en la URL indicada en la consola (por defecto un puerto local HTTPS/HTTP definido en `Properties/launchSettings.json`). Luego navegar a `/Pokemon` para ver el catálogo.
