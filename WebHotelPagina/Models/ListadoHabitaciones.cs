namespace WebHotelPagina.Models
{
    public class ListadoHabitaciones
    {
        public int id { get; set; }
        public string? cod_habitacion { get; set; }
        public string? tipo { get; set; }
        public decimal precio { get; set; }

        public string? descripcion { get; set; }

        public string? imagen { get; set; } 
        public bool estado { get; set; }
        public int capacidad { get; set; }
        public string? nombre { get; set; }
    }
}
