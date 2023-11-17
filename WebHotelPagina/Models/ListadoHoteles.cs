namespace WebHotelPagina.Models
{
    public class ListadoHoteles
    { 
             public int id { get; set; }
        public string? nombre { get; set; }
        public string? ciudad { get; set; }
        public string? direccion { get; set; }

        public string? telefono { get; set; }
        public bool estado { get; set; }

        public string? imagen { get; set; } 
    }
}
