using MiPrimeraAPI2Cuatrimestre.BLL.Constantes;
using MiPrimeraAPI2Cuatrimestre.BLL.Dtos;
using MiPrimeraAPI2Cuatrimestre.DAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.BLL.Servicios
{
    public class PersonaServicio : IPersonaServicio
    {
        private readonly IPersonaRepositorio _repositorio;
        private readonly ICorreoServicio _correoServicio;

        public PersonaServicio(IPersonaRepositorio repositorio, ICorreoServicio correoServicio)
        {
            _repositorio = repositorio;
            _correoServicio = correoServicio;
        }

        public async Task<bool> AgregarPersonaAsync(PersonaDto persona)
        {
            // Validar reglas de negocio
            if (!ValidarEdad(persona.Edad))
            {
                return false;
            }

            var entidadPersona = new MiPrimeraAPI2Cuatrimestre.DAL.Entidades.Persona
            {
                Nombre = persona.Nombre,
                Apellido1 = persona.Apellido1,
                Edad = persona.Edad
            };

            return await _repositorio.AgregarPersonaAsync(entidadPersona);
        }

        public async Task<bool> ActualizarPersonaAsync(PersonaDto persona)
        {
            // Validar reglas de negocio
            if (!ValidarEdad(persona.Edad))
            {
                return false;
            }

            var entidadPersona = new MiPrimeraAPI2Cuatrimestre.DAL.Entidades.Persona
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellido1 = persona.Apellido1,
                Edad = persona.Edad
            };

            bool resultado = await _repositorio.ActualizarPersonaAsync(entidadPersona);

            // Si la actualización fue exitosa, enviar notificación por correo
            if (resultado)
            {
                await _correoServicio.EnviarNotificacionAsync(
                    "Persona Actualizada",
                    $"La persona {persona.Nombre} {persona.Apellido1} (ID: {persona.Id}) ha sido actualizada correctamente."
                );
            }

            return resultado;
        }

        public async Task<bool> EliminarPersonaAsync(int id)
        {
            return await _repositorio.EliminarPersonaAsync(id);
        }

        public async Task<PersonaDto?> ObtenerPersonaPorIdAsync(int id)
        {
            var entidadPersona = await _repositorio.ObtenerPersonaPorIdAsync(id);

            if (entidadPersona == null)
            {
                return null;
            }

            return new PersonaDto
            {
                Id = entidadPersona.Id,
                Nombre = entidadPersona.Nombre,
                Apellido1 = entidadPersona.Apellido1,
                Edad = entidadPersona.Edad
            };
        }

        public async Task<List<PersonaDto>> ObtenerTodasLasPersonasAsync()
        {
            var entidades = await _repositorio.ObtenerTodasLasPersonasAsync();

            return entidades.Select(e => new PersonaDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido1 = e.Apellido1,
                Edad = e.Edad
            }).ToList();
        }

        /// <summary>
        /// Valida que la edad cumpla con las reglas de negocio
        /// Regla 1: La edad debe ser mayor o igual a 18 años
        /// Regla 2: La edad no puede ser mayor a 120 años
        /// </summary>
        private bool ValidarEdad(int edad)
        {
            return edad >= ReglasDeNegocio.EDAD_MINIMA && edad <= ReglasDeNegocio.EDAD_MAXIMA;
        }
    }
}

//REGLAS DE NEGOCIO

// 1. La edad de la persona debe ser mayor o igual a 18 años.
// 2. La edad no puede ser mayor a 120 años.
// 3. Cuando se modifica una persona debe enviar un correo electronico de notificación al usuario. (Se puede simular con un log).


//Servicio Externo de Correo Electronico
//Crear una simulacion de servicio de correo electronico que reciba un mensaje y lo escriba en un log. (Se puede usar la clase Console para simular el log).


// Implementar la clase PersonaServicio para que cumpla con las reglas de negocio y utilice el servicio de correo electronico simulado.