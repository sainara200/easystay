using Braintree;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using SlnWeb.Utilities.BrainTree;
using System.Data;
using System.Data.SqlClient;
using WebHotelPagina.Models;

namespace WebHotelPagina.Controllers
{
    public class IndexController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _email;
        private readonly IBrainTreeGate _brain;
        private readonly IWebHostEnvironment _webHost;

        public IndexController(IConfiguration configuration, IWebHostEnvironment webHost, IEmailSender email, IBrainTreeGate brain)
        {
            _webHost = webHost;
            _email = email;
            _brain = brain;
            _configuration =  configuration; 
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> IndexHabitaciones()
        {
            var listado = new List<ListadoHabitaciones>();
            using (HttpClient cliente = new HttpClient())
            {

                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/listadoGeneral2");

                string apiresponse = await response.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoHabitaciones>>(apiresponse);

            }
            return View(listado);
        }

        public async Task<IActionResult> DetailsHabitacion(string id)
        {
            using (HttpClient cliente = new HttpClient())
            {
                var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    var habitacion = JsonConvert.DeserializeObject<ListadoHabitaciones>(apiresponse);
                    var gateway = _brain.GetGateWay();
                    var clienToken = gateway.ClientToken.Generate();
                    ViewBag.ClientToken = clienToken;

                    return View(habitacion);
                }
                else
                {
                    // Manejar el caso en el que no se pueda obtener la habitación
                    return View("Error"); // Puedes crear una vista de error personalizada
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> RegistroReserva(IFormCollection collection, [FromForm] Reservas obj)
        { 
            obj.usuario = "12339dea-1dab-45de-a837-5b66dba8aea9";
            

            var connectionString = _configuration.GetConnectionString("cn1");
            Reservas reserva = null;

            var rutaEmail = _webHost.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "templates" + Path.DirectorySeparatorChar.ToString() + "PlantillaOrden.html";
            var subject = "Nueva Orden";
            string HtmlBody = "";
            string usu = "";
            int dias = 0;
            decimal monto = 0;
            string habitacion = "";

            using (StreamReader sr = System.IO.File.OpenText(rutaEmail))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //////
            ///




            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_InsertarReservaYTransaccion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Asignar los parámetros
                    command.Parameters.Add(new SqlParameter("@id_habitacion", obj.habitacion));
                    command.Parameters.Add(new SqlParameter("@id_usuario", obj.usuario));
                    command.Parameters.Add(new SqlParameter("@fecha_entrada", obj.fecha_entrada));
                    command.Parameters.Add(new SqlParameter("@fecha_salida", obj.fecha_salida));
                    command.Parameters.Add(new SqlParameter("@correo", obj.correo));

                    // Agregar el parámetro de salida para obtener el ID de la reserva
                    SqlParameter idReservaParam = new SqlParameter("@id_reserva", SqlDbType.Int);
                    idReservaParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(idReservaParam);

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();

                    
                   



                     
                    // Obtener el valor de id_reserva del parámetro de salida
                    int idReserva = (int)idReservaParam.Value;

                    if (idReserva > 0)
                    {
                        using (var command2 = new SqlCommand("USP_ObtenerReservaPorId", connection))
                        {
                            command2.CommandType = CommandType.StoredProcedure;
                            command2.Parameters.AddWithValue("@id_reserva", idReserva);

                            using (var reader = await command2.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    reserva = new Reservas
                                    {
                                        id_reserva = reader.GetInt32(0),
                                        usuario = reader.GetString(1),
                                        correo = reader.GetString(2),
                                        habitacion = reader.GetString(3),
                                        fecha_reserva = reader.GetDateTime(4),
                                        fecha_entrada = reader.GetDateTime(5),
                                        fecha_salida = reader.GetDateTime(6),
                                        dias_reserva = reader.GetInt32(7),
                                        precio_total = reader.GetDecimal(8)

                                    };
                                    usu = reserva.usuario;
                                    dias = reserva.dias_reserva;
                                    monto = reserva.precio_total;
                                    habitacion = reserva.habitacion;
                                }
                            }
                        }

                          


                        string transaccion = "";
                        string nonceFromTheClient = collection["payment_method_nonce"];

                        var request = new TransactionRequest
                        {
                            Amount = reserva.precio_total, // Especifica el monto de la transacción
                            PaymentMethodNonce = nonceFromTheClient,
                            OrderId = reserva.id_reserva.ToString(),
                            Options = new TransactionOptionsRequest
                            {
                                SubmitForSettlement = true,
                            }
                        };
                        var gateway = _brain.GetGateWay();

                        Result<Transaction> result = gateway.Transaction.Sale(request);

                        if (result.Target.ProcessorResponseText == "Approved")
                        {
                            transaccion = result.Target.Id;
                             
                        }  
                        else
                        {
                            transaccion = result.Message;
                        }

                        using (var command2 = new SqlCommand("sp_ReInsertarTransaccion", connection))
                        {
                            command2.CommandType = CommandType.StoredProcedure;
                            command2.Parameters.AddWithValue("@p_id_reserva", idReserva);
                            command2.Parameters.AddWithValue("@p_transaccion", transaccion);

                            command2.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado
                        }
                    }

                    if (reserva != null)
                    {
                        string mensaje = string.Format(HtmlBody, usu,
                                            obj.correo,
                                            obj.fecha_entrada, obj.fecha_salida, dias, monto, habitacion);

                        await _email.SendEmailAsync(obj.correo, subject, mensaje); 



                    }
                    else
                    {
                        return StatusCode(500, "Error al crear la reserva");
                    }


                }
            }
            return RedirectToAction("IndexHabitaciones"); // Redirigir a otra vista después del procesamiento

        }
    }
}

