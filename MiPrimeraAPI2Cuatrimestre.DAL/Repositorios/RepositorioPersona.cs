using MiPrimeraAPI2Cuatrimestre.DAL.Data;
using MiPrimeraAPI2Cuatrimestre.DAL.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.DAL.Repositorios
{
    public class RepositorioPersona : IPersonaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public RepositorioPersona(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ActualizarPersonaAsync(Persona persona)
        {

                _context.Personas.Update(persona);
                return await _context.SaveChangesAsync() >0;

        }

        public async Task<bool> AgregarPersonaAsync(Persona persona)
        {

                await _context.Personas.AddAsync(persona);
                return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> EliminarPersonaAsync(int id)
        {

                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                    return false;

                _context.Personas.Remove(persona);
                return await _context.SaveChangesAsync() > 0;

        }

        public async Task<Persona?> ObtenerPersonaPorIdAsync(int id)
        {
            return await _context.Personas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Persona>> ObtenerTodasLasPersonasAsync()
        {
            return await _context.Personas
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
