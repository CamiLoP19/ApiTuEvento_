using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTuEvento_.Models;

namespace ApiTuEvento_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaEventoesController : ControllerBase
    {
        private readonly ContextDB _context;

        public CategoriaEventoesController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/CategoriaEventoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaEvento>>> GetcategoriaEventos()
        {
            return await _context.categoriaEventos.ToListAsync();
        }

        // GET: api/CategoriaEventoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaEvento>> GetCategoriaEvento(int id)
        {
            var categoriaEvento = await _context.categoriaEventos.FindAsync(id);

            if (categoriaEvento == null)
            {
                return NotFound();
            }

            return categoriaEvento;
        }

        // PUT: api/CategoriaEventoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaEvento(int id, CategoriaEvento categoriaEvento)
        {
            if (id != categoriaEvento.IdCategoriaEvento)
            {
                return BadRequest();
            }

            _context.Entry(categoriaEvento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaEventoExists(id))
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

        // POST: api/CategoriaEventoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoriaEvento>> PostCategoriaEvento(CategoriaEvento categoriaEvento)
        {
            _context.categoriaEventos.Add(categoriaEvento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaEvento", new { id = categoriaEvento.IdCategoriaEvento }, categoriaEvento);
        }

        // DELETE: api/CategoriaEventoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaEvento(int id)
        {
            var categoriaEvento = await _context.categoriaEventos.FindAsync(id);
            if (categoriaEvento == null)
            {
                return NotFound();
            }

            _context.categoriaEventos.Remove(categoriaEvento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaEventoExists(int id)
        {
            return _context.categoriaEventos.Any(e => e.IdCategoriaEvento == id);
        }
    }
}
