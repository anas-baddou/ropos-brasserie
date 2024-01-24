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
    public class BrasseursController : ControllerBase
    {
        private readonly BrasserieDBContext _context;

        public BrasseursController(BrasserieDBContext context)
        {
            _context = context;
        }

        // GET: api/Brasseurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brasseur>>> GetBrasseurs()
        {
          if (_context.Brasseurs == null)
          {
              return NotFound();
          }
            return await _context.Brasseurs.ToListAsync();
        }

        // GET: api/Brasseurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brasseur>> GetBrasseur(int id)
        {
          if (_context.Brasseurs == null)
          {
              return NotFound();
          }
            var brasseur = await _context.Brasseurs.FindAsync(id);

            if (brasseur == null)
            {
                return NotFound();
            }

            return brasseur;
        }

        // PUT: api/Brasseurs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrasseur(int id, Brasseur brasseur)
        {
            if (id != brasseur.Id)
            {
                return BadRequest();
            }

            _context.Entry(brasseur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrasseurExists(id))
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

        // POST: api/Brasseurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Brasseur>> PostBrasseur(Brasseur brasseur)
        {
          if (_context.Brasseurs == null)
          {
              return Problem("Entity set 'BrasserieDBContext.Brasseurs'  is null.");
          }
            _context.Brasseurs.Add(brasseur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBrasseur", new { id = brasseur.Id }, brasseur);
        }

        // DELETE: api/Brasseurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrasseur(int id)
        {
            if (_context.Brasseurs == null)
            {
                return NotFound();
            }
            var brasseur = await _context.Brasseurs.FindAsync(id);
            if (brasseur == null)
            {
                return NotFound();
            }

            _context.Brasseurs.Remove(brasseur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BrasseurExists(int id)
        {
            return (_context.Brasseurs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //Lister l’ensemble des bières par brasserie et les grossistes qui la vendent
        [HttpGet("GetBiereParBrasserie/{brasserieId}")]
        public IActionResult GetBieresByBrasseurs(int brasserieId)
        {
            var brasserie = _context.Brasseurs
                .Include(b => b.Bieres)
                    .ThenInclude(b => b.Ventes)
                        .ThenInclude(v => v.Grossiste)
                .FirstOrDefault(b => b.Id == brasserieId);

            if (brasserie == null)
            {
                return NotFound("Brasserie non trouvée");
            }

            var beersWithVentes = brasserie.Bieres.Select(b => new
            {
                BiereId = b.Id,
                BiereNom = b.Nom,
                DegréAlcool = b.DegresAlcool,
                Prix = b.Prix,
                Ventes = b.Ventes.Select(v => new
                {
                    VenteId = v.Id,
                    htva=v.MontantHtva,
                    DateVente = v.Date,
                    GrossisteId = v.Grossiste?.Id,
                    GrossisteNom = v.Grossiste?.Nom
                }).ToList()
            }).ToList();

            return Ok(beersWithVentes);
        }


    }
}
