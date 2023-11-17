using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebHotelPagina.Models;

namespace WebHotelPagina.Controllers
{
    public class TiposController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult nuevo()
        {
            return View();
        }

        public async Task<IActionResult> IndexListadoTipos()
        {
            var listado = new List<TipoHabitacion>();
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion/listadoGeneral");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<TipoHabitacion>>(apiresponse);

            }
            return View(listado);
        }




        [HttpPost]
        public async Task<IActionResult> NuevoTipo(TbTipoHabitacion obj)
        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                {
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.capacidad.ToString()), name: "capacidad");
                   // multipartForm.Add(new StringContent(obj.id_hotel.ToString()), name: "id_hotel"); 

                    

                    var response = await cliente.PostAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion", multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var result = resultado;
                    Console.WriteLine(response); // Imprime en la consola


                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoTipos");
                }
            }
        }
        public async Task<ActionResult> NuevoTipo()

        {  // Para el DropDownList 
            var tipo = new List<TbHoteles>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                tipo = JsonConvert.DeserializeObject<List<TbHoteles>>(respuestaAPI);
            }
            ViewBag.TIPO = new SelectList(tipo, "id", "nombre");
            // 

            return View(new TbTipoHabitacion());
        }





        public async Task<ActionResult> EditTipos(int id)

        {
            TbTipoHabitacion? obj = new TbTipoHabitacion();
            // permite realizar una solicitud al servicio web api 
            using (var cliente = new HttpClient())
            {
                // realizamos una solicitud Get 
                var respuesta = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion/" + id);
                // convertimos el contenido de la variable respuesta a una cadena 
                string respuestaPI = await respuesta.Content.ReadAsStringAsync();
                // para despues deserializarlo al formato Json de un objeto Medicos 
                obj = JsonConvert.DeserializeObject<TbTipoHabitacion>(respuestaPI);
            }


            // Para el DropDownList 
            var tipo = new List<TbHoteles>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                tipo = JsonConvert.DeserializeObject<List<TbHoteles>>(respuestaAPI);
            }
            ViewBag.TIPO = new SelectList(tipo, "id", "nombre");
            // 
            return View("NuevoTipo", obj);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTipos(int id, TbTipoHabitacion obj)

        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                {
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.capacidad.ToString()), name: "capacidad");
                  //  multipartForm.Add(new StringContent(obj.id_hotel.ToString()), name: "id_hotel");
 

                    var response = await cliente.PutAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion/ActualizarTipoHabitacion/" + id, multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(response); // Imprime en la consola

                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoTipos");
                }
            }
        }


         
        public async Task<ActionResult> ActivarTipo(int id)

        {
            using (var cliente = new HttpClient())
            {

                    var response = await cliente.PutAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion/ActivarTipo/" + id,null);
                    var resultado = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(response); // Imprime en la consola

                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoTipos");
               
            }
        }


         
        public async Task<ActionResult> DesactivarTipo(int id)

        {
            using (var cliente = new HttpClient())
            {

                var response = await cliente.DeleteAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion/EliminarTipo/" + id);
                var resultado = await response.Content.ReadAsStringAsync();
                Console.WriteLine(response); // Imprime en la consola

                // Realizar acciones adicionales con la respuesta si es necesario

                return RedirectToAction("IndexListadoTipos");

            }
        }

         



    }
}
