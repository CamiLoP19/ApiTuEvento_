using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTuEvento_.Models;
using ApiTuEvento_.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ApiTuEvento_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoletoesController : ControllerBase
    {
        private readonly ContextDB _context;

        public BoletoesController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Boletoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Boleto>>> Getboletos()
        {
            return await _context.boletos.ToListAsync();
        }

        // GET: api/Boletoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Boleto>> GetBoleto(int id)
        {
            var boleto = await _context.boletos.FindAsync(id);

            if (boleto == null)
            {
                return NotFound();
            }

            return boleto;
        }
        // GET: api/Boletoes/entradas-disponibles/{eventoId}
        [HttpGet("vendidas-por-evento/{eventoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EntradasVendidasPorEvento(int eventoId)
        {
            var totalVendidas = await _context.boletos
                .CountAsync(b => b.EventoId == eventoId && b.EstadoVenta);

            return Ok(new { EventoId = eventoId, EntradasVendidas = totalVendidas });
        }
        // PUT: api/Boletoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoleto(int id, Boleto boleto)
        {
            if (id != boleto.BoletoId)
            {
                return BadRequest();
            }

            _context.Entry(boleto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoletoExists(id))
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

        // POST: api/Boletoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //  Validar y marcar boleto como usado
        [HttpPost("validar-entrada")]
        [Authorize(Roles = "Admin")] // o el rol que valida entradas
        public async Task<IActionResult> ValidarEntrada([FromBody] string codigoAN)
        {
            var boleto = await _context.boletos.FirstOrDefaultAsync(b => b.CodigoAN == codigoAN);

            if (boleto == null)
                return NotFound("Boleto no encontrado.");

            if (!boleto.EstadoVenta)
                return BadRequest("Este boleto no ha sido vendido.");

            if (boleto.Usado)
                return BadRequest("Este boleto ya fue utilizado para ingresar.");

            // Marcar como usado
            boleto.Usado = true;
            await _context.SaveChangesAsync();

            return Ok("Entrada validada correctamente. ¡Disfrute el evento!");
        }
        [HttpPost("comprar")]
        [Authorize]
        public async Task<IActionResult> ComprarBoleto([FromBody] ComprarBoletoDto dto)
        {
            // 1. Obtener el usuario autenticado
            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null) return Unauthorized();

            // 2. Buscar el boleto disponible (no vendido) por ID
            var boleto = await _context.boletos
                .FirstOrDefaultAsync(b => b.BoletoId == dto.BoletoId && !b.EstadoVenta);
            if (boleto == null)
                return BadRequest("Boleto no disponible o ya vendido.");
            // 3. VALIDACIÓN DE AFORO
            var evento = await _context.eventos.FirstOrDefaultAsync(e => e.EventoId == boleto.EventoId);
            if (evento == null)
                return BadRequest("Evento no encontrado.");

            var vendidos = await _context.boletos.CountAsync(b => b.EventoId == evento.EventoId && b.EstadoVenta);
            if (vendidos >= evento.Aforo)
                return BadRequest("¡Aforo completo! No hay más boletos disponibles para este evento.");

            // 4. Generar código alfanumérico único
            boleto.CodigoAN = Guid.NewGuid().ToString("N").Substring(0, 10);

            // 5. Generar código QR (base64) usando el helper
            boleto.CodigoQR = QRCodeHelper.GenerarCodigoQR(boleto.CodigoAN);

            // 6. Asignar usuario y marcar como vendido
            boleto.PersonaId = usuario.PersonaId;
            boleto.EstadoVenta = true;

            // 7. Guardar cambios
            await _context.SaveChangesAsync();

            // 8. Devolver la info del boleto comprado (puedes ajustar el DTO si quieres mostrar más info)
            var response = new BoletoDTO
            {
                BoletoId = boleto.BoletoId,
                TipoBoleto = boleto.TipoBoleto,
                Descripcion = boleto.Descripcion,
                Precio = boleto.Precio,
                EstadoVenta = boleto.EstadoVenta,
                CodigoQR = boleto.CodigoQR,
                CodigoAN = boleto.CodigoAN,
                EventoId = boleto.EventoId,
                PersonaId = boleto.PersonaId
            };

            return Ok(response);
        }

        [HttpPost("cancelar-boleto/{boletoId}")]
        [Authorize]
        public async Task<IActionResult> CancelarBoleto(int boletoId)
        {
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
            if (usuario == null)
                return Unauthorized();

            var boleto = await _context.boletos
                .FirstOrDefaultAsync(b => b.BoletoId == boletoId && b.Usuario.PersonaId == usuario.PersonaId);

            if (boleto == null)
                return NotFound("Boleto no encontrado o no pertenece a usted.");

            // Opcional: No permitir cancelar si ya fue usado
            if (boleto.Usado)
                return BadRequest("No se puede cancelar un boleto que ya ha sido utilizado.");

            // Opcional: No permitir cancelar después de la fecha del evento
            var evento = await _context.eventos.FindAsync(boleto.EventoId);
            if (evento != null && evento.FechaEvento <= DateTime.Now)
                return BadRequest("No se puede cancelar un boleto después de la fecha del evento.");

            // Eliminar el boleto de la base de datos
            _context.boletos.Remove(boleto);
            await _context.SaveChangesAsync();

            return Ok("Boleto cancelado y cupo liberado.");
        }

        [HttpPost("validar-boleto")]
        [Authorize(Roles = "Admin")] //  el rol que valida en la entrada
        public async Task<IActionResult> ValidarBoleto([FromBody] ValidarBoletoDTO dto)
        {
            var boleto = await _context.boletos
                .FirstOrDefaultAsync(b => b.BoletoId == dto.BoletoId); // O por dto.CodigoQR si usas QR

            if (boleto == null)
                return NotFound("Boleto no encontrado.");
            if (boleto.Usado)
                return BadRequest("Este boleto ya fue utilizado.");
            if (!boleto.EstadoVenta)
                return BadRequest("El boleto no está pagado o es inválido.");


            boleto.Usado = true;
            await _context.SaveChangesAsync();

            return Ok("Boleto válido. ¡Acceso permitido!");
        }

        

        // DELETE: api/Boletoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoleto(int id)
        {
            var boleto = await _context.boletos.FindAsync(id);
            if (boleto == null)
            {
                return NotFound();
            }

            _context.boletos.Remove(boleto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoletoExists(int id)
        {
            return _context.boletos.Any(e => e.BoletoId == id);
        }
    }
}
