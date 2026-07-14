using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiPrimeraAPI2Cuatrimestre.Controllers
{
    /// <summary>
    /// Gestión de Personas
    /// 
    /// Controlador encargado de administrar el listado de personas.
    /// Permite obtener, agregar, actualizar y eliminar personas del sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Personas")]
    public class PersonaController : ControllerBase
    {
        //DEBERIA LLAMAR UN SERVICIO
        public static List<string> personas = new List<string> { "Juan", "María", "Pedro" };

        /// <summary>
        /// Obtener todas las personas
        /// </summary>
        /// <remarks>
        /// Devuelve el listado completo de todas las personas registradas en el sistema.
        /// 
        /// **Ejemplo de respuesta:**
        /// ```
        /// ["Juan", "María", "Pedro"]
        /// ```
        /// </remarks>
        /// <returns>Lista de todas las personas</returns>
        /// <response code="200">Operación exitosa - Retorna la lista de personas</response>
        [HttpGet(Name = "GetPersonas")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonas()
        {
              return Ok(personas);
        }

        /// <summary>
        /// Obtener una persona por índice
        /// </summary>
        /// <remarks>
        /// Recupera una persona específica usando su índice en la lista (basado en 0).
        /// 
        /// **Parámetro:**
        /// - `id` = 0 → Retorna "Juan"
        /// - `id` = 1 → Retorna "María"
        /// - `id` = 2 → Retorna "Pedro"
        /// </remarks>
        /// <param name="id">Índice de la persona en la lista (0-based)</param>
        /// <returns>La persona solicitada</returns>
        /// <response code="200">Persona encontrada</response>
        /// <response code="404">El índice está fuera de rango</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPersona(int id)
        {
            return Ok(personas[id]);
        }

        /// <summary>
        /// Crear nueva persona
        /// </summary>
        /// <remarks>
        /// Agrega una nueva persona al listado del sistema.
        /// 
        /// **Ejemplo de solicitud:**
        /// ```json
        /// "Carlos"
        /// ```
        /// 
        /// **Validaciones:**
        /// - El nombre no puede estar vacío
        /// - El nombre no puede contener solo espacios en blanco
        /// </remarks>
        /// <param name="nombre">Nombre de la persona a agregar (debe no estar vacío)</param>
        /// <returns>El nombre de la persona agregada</returns>
        /// <response code="200">Persona agregada exitosamente</response>
        /// <response code="400">El nombre está vacío o contiene solo espacios</response>
        [HttpPost(Name = "AgregarPersona")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AgregarPersona([FromBody] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }
            personas.Add(nombre);
            return Ok(nombre);
        }

        /// <summary>
        /// Eliminar una persona
        /// </summary>
        /// <remarks>
        /// Remueve una persona del listado usando su nombre.
        /// 
        /// **Ejemplo de solicitud:**
        /// ```json
        /// "Juan"
        /// ```
        /// 
        /// **Validaciones:**
        /// - El nombre no puede estar vacío
        /// - La persona debe existir en el listado
        /// </remarks>
        /// <param name="nombre">Nombre de la persona a eliminar</param>
        /// <returns>El nombre de la persona eliminada</returns>
        /// <response code="200">Persona eliminada exitosamente</response>
        /// <response code="400">El nombre está vacío o contiene solo espacios</response>
        /// <response code="404">La persona no existe en el listado</response>
        [HttpDelete(Name ="BorrarPersona")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BorrarPersona([FromBody] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }
            if (!personas.Contains(nombre))
            {
                return NotFound("La persona no existe.");
            }
            personas.Remove(nombre);
            return Ok(nombre);
        }

        /// <summary>
        /// Actualizar persona existente
        /// </summary>
        /// <remarks>
        /// Modifica el nombre de una persona en una posición específica del listado.
        /// 
        /// **Ejemplo:**
        /// - Posición: 0
        /// - Nuevo nombre: "Miguel"
        /// - Resultado: Reemplaza "Juan" por "Miguel"
        /// 
        /// **Parámetros de consulta:**
        /// - `posicion`: La posición de la persona a actualizar (0-based)
        /// </remarks>
        /// <param name="nombre">Nuevo nombre para la persona</param>
        /// <param name="posicion">Índice de la persona a actualizar</param>
        /// <returns>El nuevo nombre de la persona actualizada</returns>
        /// <response code="200">Persona actualizada exitosamente</response>
        /// <response code="400">El nombre está vacío o contiene solo espacios</response>
        /// <response code="404">La posición está fuera de rango</response>
        [HttpPut(Name ="ActualizarPersona")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActualizarPersona([FromBody]string nombre, int posicion)
        {
            personas[posicion] = nombre;
            return Ok(nombre);
        }
    }
}

//LAS RESPUESTAS QUE DE UN CONTROLADOR DEBEN SER CON STATUS CODES, NO CON STRING, PORQUE EL CLIENTE PUEDE INTERPRETAR EL STRING COMO UN 200 OK Y NO COMO UN ERROR.

//CREAR UNA BASE DE DATOS PERSONA
//CREAR LA MIGRACION
//CREAR LAS CAPAS
//INTEGRAR LAS CAPAS