namespace ApiTuEvento_.Models
{
    public class Administrador : Persona
    {
    }

    public class AdministradorDTO
    {
        public string NombreUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public double Cedula { get; set; }
        public string Rol { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}
