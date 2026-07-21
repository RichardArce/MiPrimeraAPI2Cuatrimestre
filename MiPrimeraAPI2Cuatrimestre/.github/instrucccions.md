# Instrucciones de Desarrollo - MiPrimeraAPI2Cuatrimestre

## Entity Framework Core
- Registrar el `DbContext` vía DI (Inyección de dependencias) (ya está en `Program.cs`).
- No hardcodear la cadena en `OnConfiguring`; permitir que DI la inyecte.
- Usar migraciones (`dotnet ef migrations add <Nombre>` / `dotnet ef database update`) para evolucionar el esquema. Para SQLite, revisar limitaciones de alteraciones.
- Mantener las entidades en la carpeta `Entidades` y el contexto en `Data`.
- Utilizar IQueryable para hacer las consultas a la base de datos
- Utilizar AsNoTracking cuando sea necesario en las consultas

## Código y estilo
- Habilitar `nullable` (ya está en el proyecto). Manejar referencias nulas explícitamente y usar tipos anulables cuando corresponda.
- Preferir métodos `async` para acceso a datos (`ToListAsync`, `SaveChangesAsync`).
- Seguir convenciones PascalCase para clases y propiedades.
- **No utilizar Excepciones en las capas de Repositorio ni Servicios** - esto se maneja en el middleware. Los servicios deben enfocarse únicamente en la lógica de negocio (validaciones, transformaciones, notificaciones) sin try-catch.
- Usar `SaveChangesAsync()` en lugar de `SaveChanges()` para persistir cambios en la base de datos.
- Confirmar si los cambios se aplicaron con `SaveChangesAsync() > 0`

## Patrón de Repositorio
- Los repositorios deben inyectar `ApplicationDbContext` a través del constructor.
- **Métodos que retornan `bool`**: Devolver el resultado de `SaveChangesAsync() > 0` (sin try-catch, las excepciones se manejan en el middleware).
- **Métodos de consulta**: Usar `AsNoTracking()` para optimizar (consultas de solo lectura).
- **Métodos de lectura única**: Retornar tipo nullable (`Entidad?`) para indicar que puede no encontrarse.

### Ejemplo de método de eliminación:
```csharp
public async Task<bool> EliminarPersonaAsync(int id)
{
    var persona = await _context.Personas.FindAsync(id);
    if (persona == null)
        return false;

    _context.Personas.Remove(persona);
    return await _context.SaveChangesAsync() > 0;
}
```

### Ejemplo de método de actualización:
```csharp
public async Task<bool> ActualizarPersonaAsync(Persona persona)
{
    _context.Personas.Update(persona);
    return await _context.SaveChangesAsync() > 0;
}
```

### Ejemplo de método de agregación:
```csharp
public async Task<bool> AgregarPersonaAsync(Persona persona)
{
    await _context.Personas.AddAsync(persona);
    return await _context.SaveChangesAsync() > 0;
}
```

### Ejemplo de método de consulta (una entidad):
```csharp
public async Task<Persona?> ObtenerPersonaPorIdAsync(int id)
{
    return await _context.Personas
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

### Ejemplo de método de consulta (lista):
```csharp
public async Task<List<Persona>> ObtenerTodasLasPersonasAsync()
{
    return await _context.Personas
        .AsNoTracking()
        .ToListAsync();
}
```

## Patrón de Servicios (BLL)
- Los servicios deben inyectar el repositorio correspondiente a través del constructor.
- **No usar try-catch en servicios**: El manejo de excepciones se realiza en el middleware de la API.
- **Los servicios deben enfocarse únicamente en la lógica de negocio**: validaciones, transformaciones de DTOs, notificaciones, etc.
- **Métodos que retornan `bool`**: Usar para indicar si la operación fue exitosa (validaciones fallidas retornan `false`).
- **Métodos de consulta**: Retornar DTOs en lugar de entidades, transformando los datos del repositorio.
- **Métodos asincrónico**: Todos los métodos que realicen operaciones I/O (base de datos, servicios externos) deben ser asincrónico (`async Task`).
- **Notificaciones externas**: Al llamar a servicios externos (correo, logging, etc.), usar `await` para garantizar que la operación se completa antes de continuar.

## Constantes de Negocio
- **Ubicación**: Las constantes de reglas de negocio deben estar centralizadas en la carpeta `BLL/Constantes`.
- **Archivo**: Crear un archivo estático público llamado `ReglasDenegocio.cs` que contenga todas las constantes aplicables a la lógica de negocio.
- **Documentación**: Cada constante debe estar documentada con un comentario XML que explique su propósito y la regla de negocio que representa.
- **Uso**: Los servicios deben importar y utilizar estas constantes en lugar de hardcodearlas.

### Ejemplo de archivo de constantes:
```csharp
namespace MiPrimeraAPI2Cuatrimestre.BLL.Constantes
{
    /// <summary>
    /// Constantes que definen las reglas de negocio de la aplicación
    /// </summary>
    public static class ReglasDenegocio
    {
        /// <summary>
        /// Edad mínima permitida para una persona (Regla 1)
        /// </summary>
        public const int EDAD_MINIMA = 18;

        /// <summary>
        /// Edad máxima permitida para una persona (Regla 2)
        /// </summary>
        public const int EDAD_MAXIMA = 120;
    }
}
```

### Ejemplo de uso en servicios:
```csharp
private bool ValidarEdad(int edad)
{
    return edad >= ReglasDenegocio.EDAD_MINIMA && edad <= ReglasDenegocio.EDAD_MAXIMA;
}
```

## Servicios Externos y Notificaciones
- **Asincronía**: Todos los servicios externos (correo, logging, APIs externas) deben implementar métodos asincrónico usando `async Task`.
- **Nomenclatura**: Métodos asincrónico deben terminar con el sufijo `Async` (ejemplo: `EnviarNotificacionAsync`).
- **Await obligatorio**: Cuando se invoca un servicio externo desde un servicio, se debe usar `await` para esperar la finalización.
- **Simulación de operaciones I/O**: Para servicios simulados (como correo en Console), usar `Task.Run()` para mantener la compatibilidad asincrónica.

### Ejemplo de servicio externo:
```csharp
public interface ICorreoServicio
{
    Task EnviarNotificacionAsync(string asunto, string mensaje);
}

public class CorreoServicio : ICorreoServicio
{
    public async Task EnviarNotificacionAsync(string asunto, string mensaje)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"Asunto: {asunto}");
            Console.WriteLine($"Mensaje: {mensaje}");
        });
    }
}
```

### Ejemplo de uso en servicios:
```csharp
if (resultado)
{
    await _correoServicio.EnviarNotificacionAsync(
        "Persona Actualizada",
        $"La persona {persona.Nombre} ha sido actualizada correctamente."
    );
}
```

## Controladores (API)
- **Inyección de dependencias**: Los controladores deben inyectar el servicio correspondiente a través del constructor, nunca usar variables estáticas.
- **Responsabilidad**: Los controladores son la capa de presentación. Solo deben:
  - Recibir y validar requests
  - Llamar al servicio con los datos validados
  - Transformar respuestas del servicio en respuestas HTTP apropiadas
- **Respuestas HTTP**: Siempre retornar status codes apropiados (no solo strings):
  - `200 OK` para operaciones exitosas
  - `201 Created` para creación de recursos
  - `400 Bad Request` para datos inválidos
  - `404 Not Found` para recursos no encontrados
- **DTOs**: Usar DTOs para recibir y retornar datos, nunca entidades de base de datos.
- **Validación**: Validar `ModelState` antes de llamar al servicio.
- **Documentación**: Usar XML comments y decoradores `ProducesResponseType` para documentar los endpoints.

### Ejemplo de controlador bien estructurado:
```csharp
[ApiController]
[Route("api/[controller]")]
public class PersonaController : ControllerBase
{
    private readonly IPersonaServicio _personaServicio;

    public PersonaController(IPersonaServicio personaServicio)
    {
        _personaServicio = personaServicio;
    }

    /// <summary>
    /// Obtener todas las personas
    /// </summary>
    [HttpGet(Name = "GetPersonas")]
    [ProducesResponseType(typeof(List<PersonaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonaDto>>> GetPersonas()
    {
        var personas = await _personaServicio.ObtenerTodasLasPersonasAsync();
        return Ok(personas);
    }

    /// <summary>
    /// Crear nueva persona
    /// </summary>
    [HttpPost(Name = "AgregarPersona")]
    [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonaDto>> AgregarPersona([FromBody] PersonaDto personaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _personaServicio.AgregarPersonaAsync(personaDto);

        if (!resultado)
        {
            return BadRequest(new { mensaje = "No se puede agregar la persona." });
        }

        return CreatedAtAction(nameof(GetPersona), new { id = personaDto.Id }, personaDto);
    }

    /// <summary>
    /// Eliminar una persona
    /// </summary>
    [HttpDelete("{id}", Name = "EliminarPersona")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarPersona(int id)
    {
        var resultado = await _personaServicio.EliminarPersonaAsync(id);

        if (!resultado)
        {
            return NotFound(new { mensaje = "La persona no existe." });
        }

        return Ok(new { mensaje = "Persona eliminada exitosamente." });
    }
}
### Paquetes EF Core:
- `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
- `dotnet add package Microsoft.EntityFrameworkCore.Design`

### Herramienta CLI:
- `dotnet tool install --global dotnet-ef`

### Scaffold desde SQLite (genera entidades y contexto):
- `dotnet ef dbcontext scaffold "Data Source=C:\ruta\a\tu.db" Microsoft.EntityFrameworkCore.Sqlite --output-dir Entidades --context ApplicationDbContext --context-dir Data --force`

## Migraciones:
- `dotnet ef migrations add InitialCreate`
- `dotnet ef database update`

## Otras recomendaciones
- Documentar cómo ejecutar el proyecto en desarrollo (`dotnet restore`, `dotnet build`, `dotnet run`).
- Mantener dependencias actualizadas y planear actualizaciones mayores con pruebas.
