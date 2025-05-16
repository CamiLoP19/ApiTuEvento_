using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTuEvento_.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApiTuEvento_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoesController : ControllerBase
    {
        private readonly ContextDB _context;

        public CarritoesController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Carritoes
        // Obtener el contenido del carrito para el usuario autenticado
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CarritoDTO>> GetCarrito()
        {
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var carrito = await _context.carritos
                .Include(c => c.Boletos)
                .FirstOrDefaultAsync(c => c.IdUsuario== usuario.PersonaId);

            if (carrito == null)
            {
                return Ok(new CarritoDTO
                {
                    IdCarrito = 0,
                    IdUsuario = usuario.PersonaId,
                    Total = 0,
                    Boletos = new List<BoletoDTO>()
                });
            }

            // Calcula el total automáticamente
            carrito.Total = carrito.Boletos.Sum(b => (decimal)b.Precio);
            await _context.SaveChangesAsync();

            var dto = new CarritoDTO
            {
                IdCarrito = carrito.IdCarrito,
                IdUsuario = carrito.IdUsuario,
                Total = carrito.Total,
                Boletos = carrito.Boletos.Select(b => new BoletoDTO
                {
                    BoletoId = b.BoletoId,
                    TipoBoleto = b.TipoBoleto,
                    Descripcion = b.Descripcion,
                    Precio = b.Precio,
                    EventoId = b.EventoId
                }).ToList()
            };

            return Ok(dto);
        }
        // GET: api/Carritoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Carrito>> GetCarrito(int id)
        {
            var carrito = await _context.carritos.FindAsync(id);

            if (carrito == null)
            {
                return NotFound();
            }

            return carrito;
        }
        [HttpGet("mis-compras")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BoletoDTO>>> GetMisBoletosComprados()
        {
            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var boletos = await _context.boletos
                .Where(b => b.PersonaId == usuario.PersonaId && b.EstadoVenta)
                .Select(b => new BoletoDTO
                {
                    BoletoId = b.BoletoId,
                    TipoBoleto = b.TipoBoleto,
                    Descripcion = b.Descripcion,
                    Precio = b.Precio,
                    EventoId = b.EventoId,
                    CodigoQR = b.CodigoQR,
                    CodigoAN = b.CodigoAN
                    
                }).ToListAsync();

            return Ok(boletos);
        }
    

// PUT: api/Carritoes/5
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPut("{id}")]
        public async Task<IActionResult> PutCarrito(int id, Carrito carrito)
        {
            if (id != carrito.IdCarrito)
            {
                return BadRequest();
            }

            _context.Entry(carrito).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarritoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Carritoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Agregar un boleto al carrito
        [HttpPost("agregar")]
        [Authorize]
        public async Task<IActionResult> AgregarAlCarrito([FromBody] AgregarCarritoDTO dto)
        {
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var boleto = await _context.boletos
                .FirstOrDefaultAsync(b => b.BoletoId == dto.BoletoId && !b.EstadoVenta);
            if (boleto == null)
                return BadRequest("Boleto no disponible.");
            // Validar aforo
            var evento = await _context.eventos.FindAsync(boleto.EventoId);
            if (evento == null)
                return NotFound("Evento no encontrado.");

            var vendidos = await _context.boletos.CountAsync(b => b.EventoId == evento.EventoId && b.EstadoVenta);
            if (vendidos >= evento.Aforo)
                return BadRequest("No hay más boletos disponibles para este evento.");

            var carrito = await _context.carritos
                .Include(c => c.Boletos)
                .FirstOrDefaultAsync(c => c.IdUsuario == usuario.PersonaId);

            if (carrito == null)
            {
                carrito = new Carrito { IdUsuario = usuario.PersonaId, Boletos = new List<Boleto>() };
                _context.carritos.Add(carrito);
            }

            if (carrito.Boletos.Any(b => b.BoletoId == boleto.BoletoId))
                return BadRequest("Este boleto ya está en el carrito.");

            carrito.Boletos.Add(boleto);
            carrito.Total = carrito.Boletos.Sum(b => (decimal)b.Precio);
            await _context.SaveChangesAsync();

            return Ok("Boleto agregado al carrito.");
        }

        // Quitar un boleto del carrito
        [HttpDelete("quitar/{boletoId}")]
        [Authorize]
        public async Task<IActionResult> QuitarDelCarrito(int boletoId)
        {
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var carrito = await _context.carritos
                .Include(c => c.Boletos)
                .FirstOrDefaultAsync(c => c.IdUsuario == usuario.PersonaId);

            if (carrito == null)
                return NotFound("Carrito no encontrado.");

            var boleto = carrito.Boletos.FirstOrDefault(b => b.BoletoId == boletoId);
            if (boleto == null)
                return NotFound("Boleto no está en el carrito.");

            carrito.Boletos.Remove(boleto);
            carrito.Total = carrito.Boletos.Sum(b => (decimal)b.Precio);
            await _context.SaveChangesAsync();

            return Ok("Boleto quitado del carrito.");
        }

        // Vaciar el carrito
        [HttpDelete("vaciar")]
        [Authorize]
        public async Task<IActionResult> VaciarCarrito()
        {
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var carrito = await _context.carritos
                .Include(c => c.Boletos)
                .FirstOrDefaultAsync(c => c.IdUsuario == usuario.PersonaId);

            if (carrito == null)
                return NotFound("Carrito no encontrado.");

            carrito.Boletos.Clear();
            carrito.Total = 0;
            await _context.SaveChangesAsync();

            return Ok("Carrito vaciado.");
        }

        private bool CarritoExists(int id)
        {
            return _context.carritos.Any(e => e.IdCarrito == id);
        }
    }
}
