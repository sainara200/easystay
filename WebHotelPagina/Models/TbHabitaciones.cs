namespace WebHotelPagina.Models
{
    public class TbHabitaciones
    {
        public string cod_habitacion { get; set; }

        public int id_tipo { get; set; }
        public decimal precio { get; set; }
        public string nombre { get; set; }

        public string descripcion { get; set; }
         
        public IFormFile  archivo { get; set; }



    }
}
