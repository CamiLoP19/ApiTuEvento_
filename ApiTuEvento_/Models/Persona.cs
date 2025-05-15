using System.ComponentModel.DataAnnotations;

namespace ApiTuEvento_.Models
{
    public class Persona
    {
        [Key] public int PersonaId { get; set; }
        public double Cedula { get; set; }
        public string Rol { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaNacimiento { get; set; }   

    }
}
