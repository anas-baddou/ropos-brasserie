using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webAPIBrasserie.Models;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace webAPIBrasserie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentesController : ControllerBase
    {
        private readonly BrasserieDBContext _context;

        public VentesController(BrasserieDBContext context)
        {
            _context = context;
        }

        // GET: api/Ventes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vente>>> GetVentes()
        {
            try
            {
                var ventes = await _context.Ventes
                    .Include(v => v.Biere)
                    .Include(v => v.Grossiste)
                    .ToListAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 256, // Set the maximum depth as needed
                                    // Other options as needed
                };

                var json = JsonSerializer.Serialize(ventes, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur interne s'est produite : {ex.Message}");
            }
        }

        // GET: api/Ventes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vente>> GetVente(int id)
        {
            try
            {
                var vente = await _context.Ventes
                    .Include(v => v.Biere)
                    .Include(v => v.Grossiste)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vente == null)
                    return NotFound($"Vente avec l'ID {id} non trouvée");

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    // Other options as needed
                };

                var json = JsonSerializer.Serialize(vente, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur interne s'est produite : {ex.Message}");
            }
        }



        // PUT: api/Ventes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVente(int id, Vente vente)
        {
            if (id != vente.Id)
            {
                return BadRequest();
            }

            _context.Entry(vente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenteExists(id))
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

        // POST: api/Ventes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vente>> PostVente(Vente vente)
        {
            if (_context.Ventes == null)
            {
                return Problem("Entity set 'BrasserieDBContext.Ventes'  is null.");
            }
            _context.Ventes.Add(vente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVente", new { id = vente.Id }, vente);
        }

        // DELETE: api/Ventes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVente(int id)
        {
            if (_context.Ventes == null)
            {
                return NotFound();
            }
            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }

            _context.Ventes.Remove(vente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VenteExists(int id)
        {
            return (_context.Ventes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //joute la vente d’une bière existante, à un grossiste existant
        [HttpPost("AjouterVente")]
        public IActionResult AjouterVente([FromBody] Vente venteModel)
        {
            try
            {
                // Obtenir la bière
                var biere = _context.Bieres.FirstOrDefault(b => b.Id == venteModel.BiereId);

                // Obtenir le grossiste
                var grossiste = _context.Grossistes.FirstOrDefault(g => g.Id == venteModel.GrossisteId);

                if (biere == null || grossiste == null)
                {
                    return NotFound("Bière ou grossiste non trouvé");
                }

                // Vérifier si la bière est vendue par le grossiste
                var isBiereVendueParGrossiste = _context.Ventes
                    .Any(v => v.BiereId == venteModel.BiereId && v.GrossisteId == venteModel.GrossisteId);

                if (!isBiereVendueParGrossiste)
                {
                    return BadRequest("La bière n'est pas vendue par le grossiste");
                }

                // Vérifier si la quantité vendue est valide
                if (venteModel.Qtevendue <= 0)
                {
                    return BadRequest("La quantité vendue doit être supérieure à zéro");
                }

                // Vérifiee si la quantité vendue ne dépasse pas le stock du grossiste
                if (venteModel.Qtevendue > grossiste.Stock)
                {
                    return BadRequest("La quantité vendue dépasse le stock du grossiste");
                }

                // Créer une nouvelle vente
                var nouvelleVente = new Vente
                {
                    BiereId = venteModel.BiereId,
                    GrossisteId = venteModel.GrossisteId,
                    Qtevendue = venteModel.Qtevendue,
                    Date = DateTime.UtcNow
                };

                // Ajouter la vente à la base de données
                _context.Ventes.Add(nouvelleVente);

                // Mettre à jour le stock du grossiste
                grossiste.Stock -= venteModel.Qtevendue;

                // save les modifications dans la base de données
                _context.SaveChanges();

                return Ok("Vente ajoutée avec succès");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
            //Mise à jour de la quantité restante d’une bière chez un grossiste
            [HttpPost("MiseAJourQuantite")]
        public IActionResult MiseAJourQuantite( MiseAJourQuantiteModel model)
        {
            try
            {
                Console.WriteLine($"Biere ID: {model.BiereId}, Grossiste ID: {model.GrossisteId}, Nouvelle Quantite: {model.NouvelleQuantite}");

                // Obtenir le grossiste
                var grossiste = _context.Grossistes.FirstOrDefault(g => g.Id == model.GrossisteId);

                // Obtenir la bière
                var biere = _context.Bieres.FirstOrDefault(b => b.Id == model.BiereId);

                if (grossiste == null || biere == null)
                {
                    return NotFound("Grossiste ou bière non trouvé");
                }

                // tester que la quantité mise à jour est valide
                if (model.NouvelleQuantite < 0)
                {
                    return BadRequest("La nouvelle quantité doit être positive");
                }

                // update la quantité restante de la bière chez le grossiste
                biere.Quantite = model.NouvelleQuantite;

                // save les modifications dans la base de données
                _context.SaveChanges();

                return Ok("Quantité mise à jour avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
    }

}


