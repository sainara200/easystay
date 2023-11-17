using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public async Task<IActionResult> IndexListadoReservas(string? usu, int? estado)
        {
            if (estado is null)
            {
                estado = 1;
            }

            var listado = new List<Reservas>();
            using (HttpClient cliente = new HttpClient())
            {
                var url = "http://testingtestteo-001-site1.ftempurl.com/api/Reserva";

                if (!string.IsNullOrEmpty(usu) || estado.HasValue)
                {
                    url += "?";

                    if (!string.IsNullOrEmpty(usu))
                    {
                        url += "usu=" + usu;

                        if (estado.HasValue)
                        {
                            url += "&";
                        }
                    }

                    if (estado.HasValue)
                    {
                        url += "estado=" + estado;
                    }
                }

                var response = await cliente.GetAsync(url);
                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<Reservas>>(apiresponse);
            }


            var usuarios = new List<USUARIO>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Authenticate/VerUsuarios");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                usuarios = JsonConvert.DeserializeObject<List<USUARIO>>(respuestaAPI);
            }
            ViewBag.USU = new SelectList(usuarios, "id", "username");
            ViewBag.SelectedUsu= usu;



            ViewBag.reserva = new[]
            {
        new { id = 1, valor = "RESERVAS VIGENTES" },
        new { id = 0, valor = "RESERVAS PASADAS" }
    };

            ViewBag.SelectedReserva = estado;

            return View(listado);
        }


       





    }
}
