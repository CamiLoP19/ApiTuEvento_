namespace ApiTuEvento_.Models
{
    public class Login
    {

    }

    public class LoginDto
    {
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
    }

    public class RegisterDto
    {
        public double Cedula { get; set; }
        public string Rol { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}