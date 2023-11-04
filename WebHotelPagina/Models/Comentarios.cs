namespace WebHotelPagina.Models
{
    public class Comentarios
    {
        /*c.codigo, c.comentario,c.email,c.fecha*/
        public int codigo { get; set; }
        public string comentario { get; set; }
        public int calificacion { get; set; }
        public string email { get; set; }
        public DateTime fecha { get; set; }
        public string habitacion { get; set; }
        public string usuario { get; set; }



        /////
        ///
        public int califi { get; set; }
        public int cantidad { get; set; }
    }
}
