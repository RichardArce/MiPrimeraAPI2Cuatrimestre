using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.BLL.Servicios
{
    public interface ICorreoServicio
    {
        Task EnviarNotificacionAsync(string asunto, string mensaje);
    }
}
