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
    public class EventosController : ControllerBase
    {
        private readonly ContextDB _context;

        public EventosController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Eventos
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventos()
        {
            var eventos = await _context.eventos
                .Select(e => new EventoResponseDto
                {
                    EventoId = e.EventoId,
                    NombreEvento = e.NombreEvento,
                    FechaEvento = e.FechaEvento,
                    LugarEvento = e.LugarEvento,
                    Aforo = e.Aforo,
                    CategoriaEvento = e.CategoriaEvento,
                    DescripcionEvento = e.DescripcionEvento,
                    ImagenUrl = e.ImagenUrl,
                    EstadoEventoActivo = e.EstadoEventoActivo
                })
                .ToListAsync();

            return Ok(eventos);
        }

        // GET: api/Eventos/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventoResponseDto>> GetEvento(int id)
        {
            var evento = await _context.eventos
                .Where(e => e.EventoId == id)
                .Select(e => new EventoResponseDto
                {
                    EventoId = e.EventoId,
                    NombreEvento = e.NombreEvento,
                    FechaEvento = e.FechaEvento,
                    LugarEvento = e.LugarEvento,
                    Aforo = e.Aforo,
                    CategoriaEvento = e.CategoriaEvento,
                    DescripcionEvento = e.DescripcionEvento,
                    ImagenUrl = e.ImagenUrl,
                    EstadoEventoActivo = e.EstadoEventoActivo
                })
                .FirstOrDefaultAsync();

            if (evento == null)
                return NotFound();

            return Ok(evento);
        }

        // POST: api/Eventos (solo admins)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<EventoResponseDto>> CrearEvento(EventoCreateDto dto)
        {
            var evento = new Evento
            {
                NombreEvento = dto.NombreEvento,
                FechaEvento = dto.FechaEvento,
                LugarEvento = dto.LugarEvento,
                Aforo = dto.Aforo,
                CategoriaEvento = dto.CategoriaEvento,
                DescripcionEvento = dto.DescripcionEvento,
                ImagenUrl = dto.ImagenUrl,
                EstadoEventoActivo = dto.EstadoEventoActivo
            };

            _context.eventos.Add(evento);
            await _context.SaveChangesAsync();

            // Mapeo a DTO de respuesta
            var response = new EventoResponseDto
            {
                EventoId = evento.EventoId,
                NombreEvento = evento.NombreEvento,
                FechaEvento = evento.FechaEvento,
                LugarEvento = evento.LugarEvento,
                Aforo = evento.Aforo,
                CategoriaEvento = evento.CategoriaEvento,
                DescripcionEvento = evento.DescripcionEvento,
                ImagenUrl = evento.ImagenUrl,
                EstadoEventoActivo = evento.EstadoEventoActivo
            };

            return CreatedAtAction(nameof(GetEvento), new { id = evento.EventoId }, response);
        }

        // PUT: api/Eventos/5 (solo admins)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarEvento(int id, EventoCreateDto dto)
        {
            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            evento.NombreEvento = dto.NombreEvento;
            evento.FechaEvento = dto.FechaEvento;
            evento.LugarEvento = dto.LugarEvento;
            evento.Aforo = dto.Aforo;
            evento.CategoriaEvento = dto.CategoriaEvento;
            evento.DescripcionEvento = dto.DescripcionEvento;
            evento.ImagenUrl = dto.ImagenUrl;
            evento.EstadoEventoActivo = dto.EstadoEventoActivo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Eventos/5 (solo admins)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarEvento(int id)
        {
            var evento = await _context.eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            _context.eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}