using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlnWeb.Utilities.BrainTree;
using System.Diagnostics;
using WebHotelPagina.Models;
using Braintree; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing; 
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims; 

namespace WebHotelPagina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly IConfiguration _configuration;
        private readonly IEmailSender _email;
        private readonly IBrainTreeGate _brain;
        private readonly IWebHostEnvironment _webHost; 




        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, 
            IWebHostEnvironment webHost, IEmailSender email, IBrainTreeGate brain)
        {
            _webHost = webHost;
            _email = email;
            _brain = brain;
            _configuration = configuration; 
        _logger = logger;
        }

        public async Task<IActionResult> Index ()
        {
            var listado = new List<ListadoHoteles>();
            List<ListadoDepa> departamentos =GetDepartamentos();
            ViewBag.Departamentos = 
            new SelectList(departamentos, "id", "departamento");
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoHoteles>>(apiresponse);

            }
            return View(listado);
        }



        public async Task<IActionResult> IndexHotel(int hotel)
        {
            Hotel2? obj = new Hotel2();
            // permite realizar una solicitud al servicio web api 
            using (var cliente = new HttpClient())
            {
                // realizamos una solicitud Get 
                var respuesta = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/" + hotel);
                // convertimos el contenido de la variable respuesta a una cadena 
                string respuestaPI = await respuesta.Content.ReadAsStringAsync();
                // para despues deserializarlo al formato Json de un objeto Medicos 
                obj = JsonConvert.DeserializeObject<Hotel2>(respuestaPI);
            }
            return View(obj);
        }


        public List<ListadoDepa> GetDepartamentos()
        {
             
                var connectionString = _configuration.GetConnectionString("cn1");
                var departamentos = new List<ListadoDepa>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_listarDEPARTAMENTOS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; 

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var departamento = new ListadoDepa
                                {
                                    id = reader.GetInt32(0),
                                    departamento = reader.GetString(1)
                                };
                                departamentos.Add(departamento);
                            }
                        }
                    }
                }
                return departamentos; 
        }




        [HttpPost]
        public async Task<IActionResult> GetHotelesxDepartamento(int departamento)
        { 
            var hotel = GetHotelesxDepartamentos(departamento); 
                return Json(hotel); 
        
        }

         


         

        public List<ListadoHoteles> GetHotelesxDepartamentos(int departamento)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("cn1");
                var hoteles = new List<ListadoHoteles>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_BUSCARHOTELES", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DEPARTAMENTO", departamento);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var hotel = new ListadoHoteles
                                {  // ----id,nombre,ciudad,direccion,telefono,imagen

                                    id = reader.GetInt32(0),
                                    nombre = reader.GetString(1),
                                    ciudad = reader.GetString(2),
                                    direccion = reader.GetString(3),
                                    telefono = reader.GetString(4),
                                    imagen = reader.GetString(5)
                                };
                                hoteles.Add(hotel);
                            }
                        }
                    }
                }
                return hoteles;
            }
            catch (Exception ex)
            {
                // Manejo del error si es necesario
                // Aquí puedes registrar el error o tomar alguna otra acción
                throw; // También puedes lanzar la excepción para que sea manejada más arriba
            }
        }


        /// http://testingtestteo-001-site1.ftempurl.com/api/Reserva/ListadoxUsuario/

        public async Task<IActionResult> ReservasxUsu()
        {
            var listado = new List<Reservas>();
            string usu=   User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Reserva/ListadoxUsuario/"+usu);

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<Reservas>>(apiresponse);

            }
            return View(listado);
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


       



    }
}