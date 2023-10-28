using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebHotelPagina.Models;

namespace WebHotelPagina.Controllers
{
    public class ReservasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> IndexListadoReservas()
        {
            var listado = new List<Reservas>();
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Reserva\r\n");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<Reservas>>(apiresponse);

            }
            return View(listado);
        }




    }
}
