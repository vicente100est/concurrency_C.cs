using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/tarjeta")]
    [ApiController]
    public class TarjetasController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> PorcesarTarjeta([FromBody] string tarjeta)
        {
            var valorAreatorio = RandomGen.NextDouble();
            var aprobada = valorAreatorio > 0.1;

            await Task.Delay(1000);
            Console.WriteLine($"Tarjeta {tarjeta} procesada");

            return Ok(new { Tarjeta = tarjeta, Aprobada = aprobada });
        }
    }
}
