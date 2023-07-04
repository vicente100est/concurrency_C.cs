using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/saludar")]
    [ApiController]
    public class SaludarController : ControllerBase
    {
        [HttpGet("{nombre}")]
        public ActionResult<string> ObtenerSaludo(string nombre)
        {
            return $"Hola mi estimado {nombre}! Espero que estes teniendo un gran dia";
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<string>> ObtenerSaludoConDelay(string name)
        {
            //Contexto de Sincronización
            Console.WriteLine($"Hilo antes del await: {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(500);
            Console.WriteLine($"Hilo despues del await: {Thread.CurrentThread.ManagedThreadId}");

            var esperar = RandomGen.NextDouble() * 10 + 1;
            await Task.Delay((int)esperar * 1000);
             
            return $"Hola mi estimado {name}! Espero que estes teniendo un gran dia";
        }
    }
}
