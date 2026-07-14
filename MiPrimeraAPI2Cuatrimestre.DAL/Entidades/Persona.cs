using System;
using System.Collections.Generic;

namespace MiPrimeraAPI2Cuatrimestre.DAL.Entidades;

public partial class Persona
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public int Edad { get; set; }
}
