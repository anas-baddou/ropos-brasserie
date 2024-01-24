using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIBrasserie.Models;

namespace webAPIBrasserie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BieresController : ControllerBase
    {
        private readonly BrasserieDBContext _context;

        public BieresController(BrasserieDBContext context)
        {
            _context = context;
        }

        // GET: api/Bieres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Biere>>> GetBieres()
        {
            try
            {
                var Bieres = await _context.Bieres
                    
                    .Include(v => v.Brasseur)
                    .ToListAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 256, // Set the maximum depth as needed
                                    // Other options as needed
                };

                var json = JsonSerializer.Serialize(Bieres, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur interne s'est produite : {ex.Message}");
            }
        }

        // GET: api/Bieres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Biere>> GetBiere(int id)
        {
            try
            {
                var biere = await _context.Bieres
                   
                    .Include(v => v.Brasseur)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (biere == null)
                    return NotFound($"biere avec l'ID {id} non trouvée");

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    // Other options as needed
                };

                var json = JsonSerializer.Serialize(biere, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur interne s'est produite : {ex.Message}");
            }
        }

        // PUT: api/Bieres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBiere(int id, Biere biere)
        {
            if (id != biere.Id)
            {
                return BadRequest();
            }

            _context.Entry(biere).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BiereExists(id))
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

        // POST: api/Bieres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Biere>> PostBiere(Biere biere)
        {
          if (_context.Bieres == null)
          {
              return Problem("Entity set 'BrasserieDBContext.Bieres'  is null.");
          }
            _context.Bieres.Add(biere);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBiere", new { id = biere.Id }, biere);
        }

        // DELETE: api/Bieres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBiere(int id)
        {
            if (_context.Bieres == null)
            {
                return NotFound();
            }
            var biere = await _context.Bieres.FindAsync(id);
            if (biere == null)
            {
                return NotFound();
            }

            _context.Bieres.Remove(biere);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BiereExists(int id)
        {
            return (_context.Bieres?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //Supprimer une bière d’un brasseur
        [HttpDelete("DeleteBiere/{brasserieId}/{biereId}")]
        public async Task<IActionResult> DeleteBeer(int brasserieId, int biereId)
        {
            var brasserie = await _context.Brasseurs.FindAsync(brasserieId);

            if (brasserie == null)
            {
                return NotFound("Brasserie non trouvée");
            }

            var biere = await _context.Bieres.FindAsync(biereId);

            if (biere == null)
            {
                return NotFound("Bière non trouvée");
            }

            // Assurez-vous que la bière appartient à la brasserie
            if (biere.BrasseurId!= brasserieId)
            {
                return BadRequest("La bière ne fait pas partie de cette brasserie");
            }

            _context.Bieres.Remove(biere);
            await _context.SaveChangesAsync();

            return Ok("Bière supprimée avec succès");
        }
       
    }

}
