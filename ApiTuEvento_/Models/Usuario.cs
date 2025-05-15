namespace ApiTuEvento_.Models
{
    public class Usuario : Persona
    {
        public ICollection<Boleto>? boletos { get; set; }
        public virtual Carrito? Carrito { get; set; }
    }

    public class UsuarioDTO
    {
        public string NombreUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public double Cedula { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
    }
}
