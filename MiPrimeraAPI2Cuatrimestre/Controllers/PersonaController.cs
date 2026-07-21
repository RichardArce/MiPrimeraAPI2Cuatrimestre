using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiPrimeraAPI2Cuatrimestre.BLL.Dtos;
using MiPrimeraAPI2Cuatrimestre.BLL.Servicios;

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
        private readonly IPersonaServicio _personaServicio;

        public PersonaController(IPersonaServicio personaServicio)
        {
            _personaServicio = personaServicio;
        }

        /// <summary>
        /// Obtener todas las personas
        /// </summary>
        /// <remarks>
        /// Devuelve el listado completo de todas las personas registradas en el sistema.
        /// 
        /// **Ejemplo de respuesta:**
        /// ```json
        /// [
        ///   { "id": 1, "nombre": "Juan", "apellido1": "García", "edad": 30 },
        ///   { "id": 2, "nombre": "María", "apellido1": "López", "edad": 28 }
        /// ]
        /// ```
        /// </remarks>
        /// <returns>Lista de todas las personas</returns>
        /// <response code="200">Operación exitosa - Retorna la lista de personas</response>
        [HttpGet(Name = "GetPersonas")]
        [ProducesResponseType(typeof(List<PersonaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PersonaDto>>> GetPersonas()
        {
            var personas = await _personaServicio.ObtenerTodasLasPersonasAsync();
            return Ok(personas);
        }

        /// <summary>
        /// Obtener una persona por ID
        /// </summary>
        /// <remarks>
        /// Recupera una persona específica usando su identificador único.
        /// </remarks>
        /// <param name="id">Identificador de la persona</param>
        /// <returns>La persona solicitada</returns>
        /// <response code="200">Persona encontrada</response>
        /// <response code="404">La persona no existe</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonaDto>> GetPersona(int id)
        {
            var persona = await _personaServicio.ObtenerPersonaPorIdAsync(id);
            
            if (persona == null)
            {
                return NotFound(new { codigo = "C02", mensaje = "La persona no existe." });
            }

            return Ok(persona);
        }

        /// <summary>
        /// Crear nueva persona
        /// </summary>
        /// <remarks>
        /// Agrega una nueva persona al sistema.
        /// 
        /// **Ejemplo de solicitud:**
        /// ```json
        /// {
        ///   "nombre": "Carlos",
        ///   "apellido1": "Martínez",
        ///   "edad": 25
        /// }
        /// ```
        /// 
        /// **Validaciones:**
        /// - El nombre no puede estar vacío
        /// - La edad debe ser mayor o igual a 18 años
        /// - La edad no puede ser mayor a 120 años
        /// </remarks>
        /// <param name="personaDto">Datos de la persona a crear</param>
        /// <returns>La persona creada</returns>
        /// <response code="201">Persona creada exitosamente</response>
        /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
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
                return BadRequest(new { codigo="C01", mensaje = "No se puede agregar la persona. Verifique que cumpla con las reglas de negocio (edad entre 18 y 120 años)." }); //Patron de diseño  
            }
               
            return CreatedAtAction(nameof(GetPersona), new { id = personaDto.Id }, personaDto); // revisar
        }

        /// <summary>
        /// Actualizar persona existente
        /// </summary>
        /// <remarks>
        /// Modifica los datos de una persona existente.
        /// 
        /// **Ejemplo:**
        /// ```json
        /// {
        ///   "id": 1,
        ///   "nombre": "Miguel",
        ///   "apellido1": "Rodríguez",
        ///   "edad": 32
        /// }
        /// ```
        /// 
        /// **Validaciones:**
        /// - La edad debe ser mayor o igual a 18 años
        /// - La edad no puede ser mayor a 120 años
        /// </remarks>
        /// <param name="personaDto">Datos actualizados de la persona</param>
        /// <returns>Mensaje de éxito o error</returns>
        /// <response code="200">Persona actualizada exitosamente (incluye notificación por correo)</response>
        /// <response code="400">Datos inválidos o reglas de negocio no cumplidas</response>
        [HttpPut(Name = "ActualizarPersona")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarPersona([FromBody] PersonaDto personaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _personaServicio.ActualizarPersonaAsync(personaDto);

            if (!resultado)
            {
                return BadRequest(new { codigo = "C03", mensaje = "No se puede actualizar la persona. Verifique que cumpla con las reglas de negocio (edad entre 18 y 120 años)." });
            }

            return Ok(new { mensaje = "Persona actualizada exitosamente. Se ha enviado una notificación de cambios." });
        }

        /// <summary>
        /// Eliminar una persona
        /// </summary>
        /// <remarks>
        /// Remueve una persona del sistema usando su ID.
        /// 
        /// **Parámetro:**
        /// - `id`: Identificador de la persona a eliminar
        /// </remarks>
        /// <param name="id">Identificador de la persona a eliminar</param>
        /// <returns>Mensaje de éxito o error</returns>
        /// <response code="200">Persona eliminada exitosamente</response>
        /// <response code="404">La persona no existe</response>
        [HttpDelete("{id}", Name = "EliminarPersona")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarPersona(int id)
        {
            var resultado = await _personaServicio.EliminarPersonaAsync(id);

            if (!resultado)
            {
                return NotFound(new { codigo = "C05", mensaje = "La persona no existe o no se pudo eliminar." });
            }

            return Ok(new { mensaje = "Persona eliminada exitosamente." });
        }
    }
}