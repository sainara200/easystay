namespace WebHotelPagina.Models
{
    public class ListadoHabitaciones
    {
        public int id { get; set; }
        public string? cod_habitacion { get; set; }
        public string? tipo { get; set; }
        public string hotel { get; set; }
        public decimal precio { get; set; }

        public string? descripcion { get; set; }

        public string? imagen { get; set; } 
        public bool estado { get; set; }
        public int capacidad { get; set; }
        public string? nombre { get; set; }
        public int IdTipo { get; set; }
        public int IdHotel { get; set; } 
        public DateTime? fechaSalida { get; set; }

        public string? FormattedFechaSalida => fechaSalida?.ToString("dd/MM/yyyy");


        public IEnumerable<Comentarios> comentarios { get; set; }
        public IEnumerable<Comentarios> calificacion { get; set; }


    }
}
