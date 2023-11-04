using System.Text.Json.Serialization;

namespace WebHotelPagina.Models
{
    public class Hotel2
    {
        public int id { get; set; }
        public string  nombre { get; set; } 
        public string  direccion { get; set; }

        public string  telefono { get; set; }
        public string imagen { get; set; }
        public string  Servicios { get; set; }
        public string  web { get; set; }
        public string  correo { get; set; }
        public string  descripcion { get; set; }
        public decimal  tarifamin { get; set; }
        public decimal  tarifamax { get; set; } 
        public int  id_ciudad { get; set; }
        public IFormFile archivo { get; set; }

    }
}
