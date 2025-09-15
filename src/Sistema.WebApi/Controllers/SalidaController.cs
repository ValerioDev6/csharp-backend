
using Microsoft.AspNetCore.Mvc;
using Sistema.WebApi.Datos.DBContexAplication;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalidaController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public SalidaController(DBContexAplication context)
        {
            _context = context;

        }


    }

}
