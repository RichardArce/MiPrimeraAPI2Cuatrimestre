using MiPrimeraAPI2Cuatrimestre.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.BLL.Servicios
{
    public interface IPersonaServicio
    {
        Task<List<PersonaDto>> ObtenerTodasLasPersonasAsync();
        Task<PersonaDto?> ObtenerPersonaPorIdAsync(int id);
        Task<bool> AgregarPersonaAsync(PersonaDto persona);
        Task<bool> ActualizarPersonaAsync(PersonaDto persona);
        Task<bool> EliminarPersonaAsync(int id);
    }
}
