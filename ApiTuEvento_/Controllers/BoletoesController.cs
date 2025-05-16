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
        [Authorize(Roles = "Administrador")]
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

            // 3. Generar código alfanumérico único
            boleto.CodigoAN = Guid.NewGuid().ToString("N").Substring(0, 10);

            // 4. Generar código QR (base64) usando el helper
            boleto.CodigoQR = QRCodeHelper.GenerarCodigoQR(boleto.CodigoAN);

            // 5. Asignar usuario y marcar como vendido
            boleto.PersonaId = usuario.PersonaId;
            boleto.EstadoVenta = true;

            // 6. Guardar cambios
            await _context.SaveChangesAsync();

            // 7. Devolver la info del boleto comprado (puedes ajustar el DTO si quieres mostrar más info)
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
