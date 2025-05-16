using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class Carrito
    {

        [Key] public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public decimal Total { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<Boleto> Boletos { get; set; } = new List<Boleto>();
    } 

        public class CarritoDTO
    {
            public int IdCarrito { get; set; }
            public int IdUsuario { get; set; }
            public decimal Total { get; set; }
            public List<BoletoDTO> Boletos { get; set; }
        
    }

    public class AgregarCarritoDTO
    {
        public int BoletoId { get; set; }
    }
}
