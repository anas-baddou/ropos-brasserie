using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIBrasserie.Models;

namespace webAPIBrasserie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrossistesController : ControllerBase
    {
        private readonly BrasserieDBContext _context;

        public GrossistesController(BrasserieDBContext context)
        {
            _context = context;
        }

        // GET: api/Grossistes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grossiste>>> GetGrossistes()
        {
          if (_context.Grossistes == null)
          {
              return NotFound();
          }
            return await _context.Grossistes.ToListAsync();
        }

        // GET: api/Grossistes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Grossiste>> GetGrossiste(int id)
        {
          if (_context.Grossistes == null)
          {
              return NotFound();
          }
            var grossiste = await _context.Grossistes.FindAsync(id);

            if (grossiste == null)
            {
                return NotFound();
            }

            return grossiste;
        }

        // PUT: api/Grossistes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrossiste(int id, Grossiste grossiste)
        {
            if (id != grossiste.Id)
            {
                return BadRequest();
            }

            _context.Entry(grossiste).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrossisteExists(id))
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

        // POST: api/Grossistes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Grossiste>> PostGrossiste(Grossiste grossiste)
        {
          if (_context.Grossistes == null)
          {
              return Problem("Entity set 'BrasserieDBContext.Grossistes'  is null.");
          }
            _context.Grossistes.Add(grossiste);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGrossiste", new { id = grossiste.Id }, grossiste);
        }

        // DELETE: api/Grossistes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrossiste(int id)
        {
            if (_context.Grossistes == null)
            {
                return NotFound();
            }
            var grossiste = await _context.Grossistes.FindAsync(id);
            if (grossiste == null)
            {
                return NotFound();
            }

            _context.Grossistes.Remove(grossiste);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrossisteExists(int id)
        {
            return (_context.Grossistes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

      

    }
}
