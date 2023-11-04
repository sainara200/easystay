using Microsoft.AspNetCore.Mvc;
using WebHotelPagina.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; 
using System.Text;
using System.Net.Http.Headers; 

namespace WebHotelPagina.Controllers
{
    public class HabitacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexListadoHabitaciones()
        {
            var listado = new List<ListadoHabitaciones>();
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/listadoGeneral");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoHabitaciones>>(apiresponse);

            }
            return View(listado);
        }




        [HttpPost]
        public async Task<IActionResult> NuevaHabitacion(TbHabitaciones obj)
        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                { 
                    multipartForm.Add(new StringContent(obj.precio.ToString()), name: "precio");
                    multipartForm.Add(new StringContent(obj.id_tipo.ToString()), name: "id_tipo");
                    multipartForm.Add(new StringContent(obj.nombre), name: "nombre");
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.id_hotel.ToString()), name: "hotel_id");

                    var filestream = new StreamContent(obj.archivo.OpenReadStream());
                    filestream.Headers.ContentType = new MediaTypeHeaderValue(obj.archivo.ContentType);

                    multipartForm.Add(filestream, name: "archivo", fileName: obj.archivo.FileName);

                    var response = await cliente.PostAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones", multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var result = resultado;

                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoHabitaciones");
                }
            }
        }
        public async Task<ActionResult> NuevaHabitacion()

        {

            // Para el DropDownList 
            var tipo = new List<TbTipoHabitacion>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                tipo = JsonConvert.DeserializeObject<List<TbTipoHabitacion>>(respuestaAPI);
            }
            ViewBag.TIPO = new SelectList(tipo, "id", "descripcion");

            var hotel = new List<TbHoteles>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ListadoGeneral");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                hotel = JsonConvert.DeserializeObject<List<TbHoteles>>(respuestaAPI);
            }
            ViewBag.HOTEL = new SelectList(hotel, "id", "nombre");

            return View(new TbHabitaciones());
        }





        public async Task<ActionResult> EditHabitaciones(string id)

        {
            TbHabitaciones? obj = new TbHabitaciones();
            // permite realizar una solicitud al servicio web api 
            using (var cliente = new HttpClient())
            {
                // realizamos una solicitud Get 
                var respuesta = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/" + id);
                // convertimos el contenido de la variable respuesta a una cadena 
                string respuestaPI = await respuesta.Content.ReadAsStringAsync();
                // para despues deserializarlo al formato Json de un objeto Medicos 
                obj = JsonConvert.DeserializeObject<TbHabitaciones>(respuestaPI);
            }


            // Para el DropDownList 
            var tipo = new List<TbTipoHabitacion>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                tipo = JsonConvert.DeserializeObject<List<TbTipoHabitacion>>(respuestaAPI);
            }
            ViewBag.TIPO = new SelectList(tipo, "id", "descripcion");


            var hotel = new List<TbHoteles>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ListadoGeneral");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                hotel = JsonConvert.DeserializeObject<List<TbHoteles>>(respuestaAPI);
            }
            ViewBag.HOTEL = new SelectList(hotel, "id", "nombre");
            // 
            return View("NuevaHabitacion", obj);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditHabitaciones(string id, TbHabitaciones obj)

        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                {
                    multipartForm.Add(new StringContent(obj.precio.ToString()), name: "precio");
                    multipartForm.Add(new StringContent(obj.id_tipo.ToString()), name: "id_tipo");
                    multipartForm.Add(new StringContent(obj.nombre), name: "nombre");
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.id_hotel.ToString()), name: "hotel_id");

                    if (obj.archivo != null)
                    {
                        var filestream = new StreamContent(obj.archivo.OpenReadStream());
                        filestream.Headers.ContentType = new MediaTypeHeaderValue(obj.archivo.ContentType);
                        multipartForm.Add(filestream, name: "archivo", fileName: obj.archivo.FileName);
                    }

                    var response = await cliente.PutAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/ActualizarHabitacion/" + id, multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();

                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoHabitaciones");
                }
            }
        }










    }
}
