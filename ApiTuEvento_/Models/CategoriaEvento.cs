using ApiTuEvento_.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class CategoriaEvento
    {
        [Key] public int IdCategoriaEvento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public ICollection<Evento> Eventos { get; set; }
    }

    public class CategoriaEventoDTO
    {
        public int IdCategoriaEvento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

}

