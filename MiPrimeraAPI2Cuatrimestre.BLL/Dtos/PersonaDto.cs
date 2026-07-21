using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.BLL.Dtos
{
    public record PersonaDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellido1 { get; set; } = null!;

        public int Edad { get; set; }
    }
}
