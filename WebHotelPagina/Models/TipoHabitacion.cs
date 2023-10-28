namespace WebHotelPagina.Models
{
    public class TipoHabitacion
    {
        public int id { get; set; }
        public int id_hotel { get; set; }
        public string descripcion { get; set; }
        public int capacidad { get; set; }
        public string nombreHotel { get; set; } 
    }
}
