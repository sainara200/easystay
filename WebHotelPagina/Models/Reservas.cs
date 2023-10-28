using Newtonsoft.Json;

namespace WebHotelPagina.Models
{
    public class Reservas
    {
        public int id_reserva { get; set; }
        public string usuario { get; set; }
        public string correo { get; set; }
        public string habitacion { get; set; }
         
        public DateTime fecha_reserva { get; set; }

        public DateTime fecha_entrada { get; set; }

        public DateTime fecha_salida { get; set; }
        public int dias_reserva { get; set; }
        public Decimal precio_total { get; set; }
    }
}
