using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SlnWeb.Utilities.BrainTree;
using WebHotelPagina.Models; 
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http.Headers;

namespace WebHotelPagina.Controllers
{
    public class HotelesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _email;
        private readonly IBrainTreeGate _brain;
        private readonly IWebHostEnvironment _webHost;

        public HotelesController(IConfiguration configuration, IWebHostEnvironment webHost, IEmailSender email, IBrainTreeGate brain)
        {
            _webHost = webHost;
            _email = email;
            _brain = brain;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

       
        public async Task<IActionResult> IndexListadoHoteles()
        {
            var listado = new List<ListadoHoteles>();
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ListadoGeneral");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoHoteles>>(apiresponse);

            }
            return View(listado);
        }




        [HttpPost]
        public async Task<IActionResult> NuevoHotel(Hotel2 obj)
        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                {
                     
                    multipartForm.Add(new StringContent(obj.nombre.ToString()), name: "Nombre");
                    multipartForm.Add(new StringContent(obj.direccion.ToString()), name: "Direccion");
                    multipartForm.Add(new StringContent(obj.id_ciudad.ToString()), name: "IdCiudad");
                    multipartForm.Add(new StringContent(obj.telefono.ToString()), name: "Telefono");
                    multipartForm.Add(new StringContent(obj.Servicios.ToString()), name: "Servicios");
                    multipartForm.Add(new StringContent(obj.web.ToString()), name: "web");
                    multipartForm.Add(new StringContent(obj.correo.ToString()), name: "correo");
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.tarifamin.ToString()), name: "tarifamin");
                    multipartForm.Add(new StringContent(obj.tarifamax.ToString()), name: "tarifamax");

                    var filestream = new StreamContent(obj.archivo.OpenReadStream());
                    filestream.Headers.ContentType = new MediaTypeHeaderValue(obj.archivo.ContentType);

                    multipartForm.Add(filestream, name: "archivo", fileName: obj.archivo.FileName);



                    var response = await cliente.PostAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles", multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var result = resultado;
                    Console.WriteLine(response); // Imprime en la consola


                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoHoteles");
                }
            }
        }
        public async Task<ActionResult> NuevoHotel()

        {  // Para el DropDownList 
            var departamento = new List<Departamento>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Departamentos");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                departamento = JsonConvert.DeserializeObject<List<Departamento>>(respuestaAPI);
            }
            ViewBag.DEP = new SelectList(departamento, "id", "nombreDep");
            // 

            return View(new Hotel2());
        }





        public async Task<ActionResult> EditHotel(int id)

        {
            Hotel2? obj = new Hotel2();
            // permite realizar una solicitud al servicio web api 
            using (var cliente = new HttpClient())
            {
                // realizamos una solicitud Get 
                var respuesta = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/" + id);
                // convertimos el contenido de la variable respuesta a una cadena 
                string respuestaPI = await respuesta.Content.ReadAsStringAsync();
                // para despues deserializarlo al formato Json de un objeto Medicos 
                obj = JsonConvert.DeserializeObject<Hotel2>(respuestaPI);
            }


            var departamento = new List<Departamento>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Departamentos");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                departamento = JsonConvert.DeserializeObject<List<Departamento>>(respuestaAPI);
            }
            ViewBag.DEP = new SelectList(departamento, "id", "nombreDep");
            // 
            // 
            return View("NuevoHotel", obj);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditHotel(int id, Hotel2 obj)

        {
            using (var cliente = new HttpClient())
            {
                using (var multipartForm = new MultipartFormDataContent())
                {
                    multipartForm.Add(new StringContent(obj.nombre.ToString()), name: "Nombre");
                    multipartForm.Add(new StringContent(obj.direccion.ToString()), name: "Direccion");
                    multipartForm.Add(new StringContent(obj.id_ciudad.ToString()), name: "IdCiudad");
                    multipartForm.Add(new StringContent(obj.telefono.ToString()), name: "Telefono");
                    multipartForm.Add(new StringContent(obj.Servicios.ToString()), name: "Servicios");
                    multipartForm.Add(new StringContent(obj.web.ToString()), name: "web");
                    multipartForm.Add(new StringContent(obj.correo.ToString()), name: "correo");
                    multipartForm.Add(new StringContent(obj.descripcion.ToString()), name: "descripcion");
                    multipartForm.Add(new StringContent(obj.tarifamin.ToString()), name: "tarifamin");
                    multipartForm.Add(new StringContent(obj.tarifamax.ToString()), name: "tarifamax");
                    //  multipartForm.Add(new StringContent(obj.id_hotel.ToString()), name: "id_hotel");


                    if (obj.archivo != null)
                    {
                        var filestream = new StreamContent(obj.archivo.OpenReadStream());
                        filestream.Headers.ContentType = new MediaTypeHeaderValue(obj.archivo.ContentType);
                        multipartForm.Add(filestream, name: "archivo", fileName: obj.archivo.FileName);
                    }


                    var response = await cliente.PutAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ActualizarHotel/" + id, multipartForm);
                    var resultado = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(response); // Imprime en la consola

                    // Realizar acciones adicionales con la respuesta si es necesario

                    return RedirectToAction("IndexListadoHoteles");
                }
            }
        }



    }
}
