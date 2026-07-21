using System;
using System.Collections.Generic;
using System.Text;

namespace MiPrimeraAPI2Cuatrimestre.BLL.Servicios
{
    public class CorreoServicio : ICorreoServicio
    {
        public async Task EnviarNotificacionAsync(string asunto, string mensaje)
        {
            // Simular operación asincrónica (por ejemplo, escritura en log o envío de correo)
            await Task.Run(() =>
            {
                Console.WriteLine("=== NOTIFICACIÓN POR CORREO ELECTRÓNICO ===");
                Console.WriteLine($"Asunto: {asunto}");
                Console.WriteLine($"Mensaje: {mensaje}");
                Console.WriteLine($"Fecha/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine("==========================================");
            });
        }
    }
}

