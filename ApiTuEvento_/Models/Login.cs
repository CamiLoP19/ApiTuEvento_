using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class Login
    {

    }

    public class LoginDto
    {
        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Contraseña { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public double Cedula { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contraseña { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        // Por defecto, "Usuario". Si tu registro permite admins, puedes exponerlo aquí.
        public string Rol { get; set; } = "Usuario";

        public class UsuarioResponseDto
        {
            public string NombreUsuario { get; set; }
            public string Correo { get; set; }
            public string Rol { get; set; }
            public double Cedula { get; set; }
            public DateTime FechaNacimiento { get; set; }
        }
    }
}