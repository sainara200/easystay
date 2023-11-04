using Braintree;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using SlnWeb.Utilities.BrainTree;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
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
         

        public List<Comentarios> GetComentariosxHabitacion(string habitacion)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("cn1");
                var comentarios = new List<Comentarios>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_listarcomentarios", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@habitacion", habitacion);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var comentario = new Comentarios
                                {
                                    codigo = reader.GetInt32(0),
                                    comentario = reader.GetString(1),
                                    email = reader.GetString(2),
                                     fecha = reader.GetDateTime(3) ,
                                     calificacion=reader.GetInt32(4)
                                };
                                comentarios.Add(comentario);
                            }
                        }
                    }
                } 
                return comentarios;
            }
            catch (Exception ex)
            {
                // Manejo del error si es necesario
                // Aquí puedes registrar el error o tomar alguna otra acción
                throw; // También puedes lanzar la excepción para que sea manejada más arriba
            }
        }



        public List<Comentarios> GetCalificacionesxHabitacion(string habitacion)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("cn1");
                var comentarios = new List<Comentarios>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_ObtenerCalificaciones", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@habitacion", habitacion);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var comentario = new Comentarios
                                {
                                    califi = reader.GetInt32(0), 
                                    cantidad = reader.GetInt32(1)
                                };
                                comentarios.Add(comentario);
                            }
                        }
                    }
                }
                return comentarios;
            }
            catch (Exception ex)
            {
                // Manejo del error si es necesario
                // Aquí puedes registrar el error o tomar alguna otra acción
                throw; // También puedes lanzar la excepción para que sea manejada más arriba
            }
        }



        public PartialViewResult ObtenerCalificacionesPorHabitacion(string habitacion)
        {
            // Obtén las calificaciones actualizadas y devuelve la vista parcial o HTML
            var calificaciones = GetCalificacionesxHabitacion(habitacion);
            return PartialView("_ComentariosPartial", calificaciones); // Reemplaza "_CalificacionesPartial" con el nombre de tu vista parcial
        }

        public PartialViewResult ObtenerComentarios(string habitacion)
        {
            ViewBag.Habitacion = habitacion;
            var detallesVenta = GetComentariosxHabitacion(habitacion);
            return PartialView("_ComentariosPartial", detallesVenta);
        }


        //[HttpGet]
        //public IActionResult ObtenerComentarios(string habitacion)
        //{
        //    var comentarios = GetComentariosxHabitacion(habitacion);
        //    return Json(comentarios);
        //}



        [HttpPost]
        public async Task<IActionResult> AgregarComentario([FromForm] Comentarios obj)
        {
            var connectionString = _configuration.GetConnectionString("cn1");
            // Obtener el ID del usuario logueado
            obj.usuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("usp_ingresarcomentarios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Asignar los parámetros
                    command.Parameters.Add(new SqlParameter("@usu", obj.usuario));
                    command.Parameters.Add(new SqlParameter("@habitacion", obj.habitacion));
                    command.Parameters.Add(new SqlParameter("@comentario", obj.comentario));
                    command.Parameters.Add(new SqlParameter("@calificacion", obj.calificacion));

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();
                }
            }
            // Luego de agregar el comentario, carga los comentarios actualizados
            var comentarios = GetComentariosxHabitacion(obj.habitacion);
          //  var calificaciones = GetCalificacionesxHabitacion(obj.habitacion);


            return Json(comentarios  );

            // No hay declaración de retorno, ya que este método no devuelve un valor
        }











       [HttpGet]
public async Task<IActionResult> GetHabitacionesxTipo(int? id)
{
    var connectionString = _configuration.GetConnectionString("cn1");
    var habitaciones = new List<ListadoHabitaciones>();

    using (var connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();

        using (var command = new SqlCommand("usp_HABITACIONEXTIPO", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TIPO", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var habitacion = new ListadoHabitaciones
                    {
                        id = reader.GetInt32(0),
                        cod_habitacion = reader.GetString(1),
                        nombre = reader.GetString(2),
                        tipo = reader.GetString(3),
                        precio = reader.GetDecimal(4),
                        descripcion = reader.GetString(5),
                        imagen = reader.GetString(6),
                        capacidad = reader.GetInt32(7)
                    };
                    habitaciones.Add(habitacion);
                }
            }
        }
    }

    return Ok(habitaciones);
}






        public async Task<IActionResult> IndexHabitaciones(int? id, int? hotel)
        {
            var listado = new List<ListadoHabitaciones>();

            using (HttpClient cliente = new HttpClient())
            {
                 
                var url = "http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/listadoxTipo";

                if (id.HasValue || hotel.HasValue)
                {
                    url += "?";

                    if (id.HasValue)
                    {
                        url += "id=" + id;
                    }

                    if (hotel.HasValue)
                    {
                        if (id.HasValue)
                        {
                            url += "&";
                        }
                        url += "hotel=" + hotel;
                    }
                }
                var response = await cliente.GetAsync( url);
                string apiresponse = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<ListadoHabitaciones>>(apiresponse);
            }


      ///  http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ListadoGeneral


            var tipo = new List<TbTipoHabitacion>();

            using (HttpClient cliente = new HttpClient())
            {
                var respuesta = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/TipoHabitacion");
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                tipo = JsonConvert.DeserializeObject<List<TbTipoHabitacion>>(respuestaAPI);
            }

            ViewBag.TIPO = new SelectList(tipo, "id", "descripcion");
            ViewBag.SelectedCategory = id;



            var hoteles = new List<TbHoteles>();
            using (HttpClient cliente = new HttpClient())
            {
                // realizar la solicitud GET 
                var respuesta =
                  await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Hoteles/ListadoGeneral");
                // convertir el contenido a una cadena 
                string respuestaAPI = await respuesta.Content.ReadAsStringAsync();
                // deserializar la cadena (Json) a Lista Genérica de Medicos 
                hoteles = JsonConvert.DeserializeObject<List<TbHoteles>>(respuestaAPI);
            }
            ViewBag.HOTEL = new SelectList(hoteles, "id", "nombre");
            ViewBag.SelectedHotel = hotel;



            return View(listado);
        }

        //public async Task<IActionResult> DetailsHabitacion(string id)
        //{
        //    using (HttpClient cliente = new HttpClient())
        //    {
        //        var response = await cliente.GetAsync("http://testingtestteo-001-site1.ftempurl.com/api/Habitaciones/" + id);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string apiresponse = await response.Content.ReadAsStringAsync();
        //            var habitacion = JsonConvert.DeserializeObject<ListadoHabitaciones>(apiresponse);
        //            var gateway = _brain.GetGateWay();
        //            var clienToken = gateway.ClientToken.Generate();
        //            ViewBag.ClientToken = clienToken;

        //            return View(habitacion);
        //        }
        //        else
        //        {
        //            // Manejar el caso en el que no se pueda obtener la habitación
        //            return View("Error"); // Puedes crear una vista de error personalizada
        //        }
        //    }
        //}

        public async Task<IActionResult> DetailsHabitacion(string id)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("cn1");
                ListadoHabitaciones habitacion = null;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("USP_ObtenerHabitacionxId2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@cod_habitacion", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                habitacion = new ListadoHabitaciones
                                {
                                    id = reader.GetInt32(0),
                                    cod_habitacion = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    IdTipo = reader.GetInt32(3),
                                    tipo = reader.GetString(4),
                                    capacidad = reader.GetInt32(5),
                                    precio = reader.GetDecimal(6),
                                    descripcion = reader.GetString(7),
                                    imagen = reader.GetString(8)
                                };
                            }
                        }
                    }
                }

                if (habitacion == null)
                {
                    // Puedes manejar el caso de habitación no encontrada mostrando una vista de error personalizada
                    // o redirigiendo a otra acción que muestre la vista de error.
                    return View("Error"); // Debes crear una vista de error llamada "Error".
                }

                // Obten los comentarios después de encontrar la habitación
                var comentarios = GetComentariosxHabitacion(habitacion.cod_habitacion);
                var calificaciones = GetCalificacionesxHabitacion(habitacion.cod_habitacion);
                habitacion.comentarios = comentarios;
                habitacion.calificacion = calificaciones;
                var gateway = _brain.GetGateWay();
                       var clienToken = gateway.ClientToken.Generate();
                         ViewBag.ClientToken = clienToken;

                // Carga la vista con el modelo de habitación
                return View(habitacion);
            }
            catch (Exception ex)
            {
                // Manejar los errores internos del servidor
                return View("Error"); // Debes crear una vista de error llamada "Error".
            }
        }



        [HttpPost]
        public async Task<IActionResult> RegistroReserva(IFormCollection collection, [FromForm] Reservas obj)
        { 
            obj.usuario = User.FindFirstValue(ClaimTypes.NameIdentifier);


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

                       //eturn Json(reserva);

                    }
                    else
                    {
                        return StatusCode(500, "Error al crear la reserva");
                    }


                }
            }
            return RedirectToAction("IndexReserva", reserva);

        }



        public async Task<IActionResult> IndexReserva(Reservas reservas)
        {
            return View(reservas);
        }










    }
}

