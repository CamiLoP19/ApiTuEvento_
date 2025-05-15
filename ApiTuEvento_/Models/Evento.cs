using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class Evento
    {
        [Key] public int EventoId { get; set; }
        public string NombreEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public string LugarEvento { get; set; }
        public int Aforo { get; set; } //Personas maximas para el evento
        public string CategoriaEvento { get; set; }
        public string DescripcionEvento { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EstadoEventoActivo { get; set; }

        public ICollection<Boleto>? Boletos { get; set; }
    }

    public class EventoDTO
    {
        public int EventoId { get; set; }
        public string NombreEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public string LugarEvento { get; set; }
        public int Aforo { get; set; }
        public string CategoriaEvento { get; set; }
        public string DescripcionEvento { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EstadoEventoActivo { get; set; }
    }
}
