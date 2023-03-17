using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
