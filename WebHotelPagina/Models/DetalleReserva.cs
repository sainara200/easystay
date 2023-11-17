namespace WebHotelPagina.Models
{
    public class DetalleReserva
    {
        public DateTime? fecha_reserva { get; set; }
        public string? FormattedFechaReserva => fecha_reserva?.ToString("dd/MM/yyyy");

        public DateTime? fecha_entrada { get; set; }
        public string? FormattedFechaEntrada => fecha_entrada?.ToString("dd/MM/yyyy");

        public DateTime? fecha_salida { get; set; }
        public string? FormattedFechaSalida => fecha_salida?.ToString("dd/MM/yyyy");

        public Decimal precio_total { get; set; } 
        public string username { get; set; }
        public string correo { get; set; }
        public string transaccion { get; set; }

    }
}
