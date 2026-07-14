using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.DAL.Repositorios
{
    public interface IPersonaRepositorio
    {
        Task<List<Entidades.Persona>> ObtenerTodasLasPersonasAsync();
        Task<Entidades.Persona?> ObtenerPersonaPorIdAsync(int id);
        Task<bool> AgregarPersonaAsync(Entidades.Persona persona);
        Task<bool> ActualizarPersonaAsync(Entidades.Persona persona);
        Task<bool> EliminarPersonaAsync(int id);
    }
}
