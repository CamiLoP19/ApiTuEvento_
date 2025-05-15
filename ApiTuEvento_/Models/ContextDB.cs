using Microsoft.EntityFrameworkCore;

namespace ApiTuEvento_.Models
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options) 
        { }


        public DbSet<ApiTuEvento_.Models.Persona> personas { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.Usuario> usuarios { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.Administrador> administradores { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.Evento> eventos { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.Carrito> carritos { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.CategoriaEvento> categoriaEventos { get; set; } = default!;
        public DbSet<ApiTuEvento_.Models.Boleto> boletos { get; set; } = default!;
    }
}
