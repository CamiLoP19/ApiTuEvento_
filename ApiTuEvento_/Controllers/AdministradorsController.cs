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
    public class AdministradorsController : ControllerBase
    {
        private readonly ContextDB _context;

        public AdministradorsController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Administradors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrador>>> Getadministradores()
        {
            return await _context.administradores.ToListAsync();
        }

        // GET: api/Administradors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Administrador>> GetAdministrador(int id)
        {
            var administrador = await _context.administradores.FindAsync(id);

            if (administrador == null)
            {
                return NotFound();
            }

            return administrador;
        }

        [HttpGet("ventas-por-evento")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VentasPorEvento()
        {
            var reporte = await _context.eventos
                .Select(evento => new
                {
                    EventoId = evento.EventoId,
                    Nombre = evento.NombreEvento,
                    BoletosVendidos = _context.boletos.Count(b => b.EventoId == evento.EventoId && b.EstadoVenta),
                    Ingresos = _context.boletos
                        .Where(b => b.EventoId == evento.EventoId && b.EstadoVenta)
                        .Sum(b => (decimal?)b.Precio) ?? 0
                })
                .ToListAsync();

            return Ok(reporte);
        
    }

        [HttpGet("usuarios-top-compras")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsuariosTopCompras()
        {
            var usuarios = await _context.usuarios
                .Select(u => new {
                    u.PersonaId,
                    u.NombreUsuario,
                    Compras = _context.boletos.Count(b => b.Usuario.PersonaId == u.PersonaId && b.EstadoVenta)
                })
                .OrderByDescending(u => u.Compras)
                .Take(10)
                .ToListAsync();

            return Ok(usuarios);
        }

        // PUT: api/Administradors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdministrador(int id, Administrador administrador)
        {
            if (id != administrador.PersonaId)
            {
                return BadRequest();
            }

            _context.Entry(administrador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministradorExists(id))
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

        // POST: api/Administradors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Administrador>> PostAdministrador(Administrador administrador)
        {
            _context.administradores.Add(administrador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdministrador", new { id = administrador.PersonaId }, administrador);
        }

        // DELETE: api/Administradors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdministrador(int id)
        {
            var administrador = await _context.administradores.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }

            _context.administradores.Remove(administrador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdministradorExists(int id)
        {
            return _context.administradores.Any(e => e.PersonaId == id);
        }
    }
}
