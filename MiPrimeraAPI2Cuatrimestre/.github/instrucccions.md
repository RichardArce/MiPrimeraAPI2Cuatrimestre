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
- No utilizar Excepciones en las capas, esto se maneja en el middleware.
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

## Comandos útiles
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
