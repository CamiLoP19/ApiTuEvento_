using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class Carrito
    {
        [Key] public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public decimal Total { get; set; }
        public ICollection<Boleto> boletos { get; set; }
    }

    public class CarritoDTO
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public decimal Total { get; set; }
    
    }
}
