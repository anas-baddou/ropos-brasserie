﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIBrasserie.Controllers;
using webAPIBrasserie.Models;

namespace webAPIBrasserie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevisController : ControllerBase
    {
        private readonly BrasserieDBContext _context;

        public DevisController(BrasserieDBContext context)
        {
            _context = context;
        }
        //Demander un devis à un grossiste, en cas de succès,
        //la méthode renvoie un prix et un récapitulatif, en cas d’erreur,
        //elle renvoie une exception et un message pour expliquer la raison
        public class DemandeDevisModel
        {
            public int GrossisteId { get; set; }
            public List<Biere> Bieres { get; set; }
        }

      

        [HttpPost("DemanderDevis")]
        public IActionResult DemanderDevis([FromBody] DemandeDevisModel demandeDevis)
        {
            

            try
            {
                // Valider que la demande n'est pas vide
                if (demandeDevis == null || demandeDevis.GrossisteId == 0 || demandeDevis.Bieres == null || demandeDevis.Bieres.Count == 0)
                    throw new Exception("La demande ne peut pas être vide");

                var grossiste = _context.Grossistes.FirstOrDefault(g => g.Id == demandeDevis.GrossisteId);
                

                if (grossiste == null)
                    throw new Exception("Le grossiste n'existe pas");

                // Valider les bières
                ValidateBieres(demandeDevis.Bieres);

                // Calculer le prix total avec les éventuelles réductions
                decimal prixTotal = CalculateTotalPrice(demandeDevis.Bieres);

                // Créer un récapitulatif ou toute information supplémentaire
                var recapitulatif = $"Demande réussie chez le grossiste {grossiste.Nom}. " +
                                    $"Prix total après réduction : {prixTotal:C}";

                return Ok(new { PrixTotal = prixTotal, Recapitulatif = recapitulatif });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        private void ValidateBieres(List<Biere> bieres)
        {
            // Valider chaque bière
            foreach (var biereDemande in bieres)
            {
                //  ajouter des validations spécifiques pour chaque bière
                if (biereDemande.Id == 0 || string.IsNullOrEmpty(biereDemande.Nom) || biereDemande.Quantite <= 0 || biereDemande.Prix <= 0)
                {
                    throw new Exception("La liste de bières est invalide");
                }
            }

            // Vérifier s'il y a des doublons
            var distinctBieres = bieres.Select(b => b.Id).Distinct().Count();
            if (distinctBieres < bieres.Count)
                throw new Exception("Il ne peut pas y avoir de doublon dans la demande");
        }
        //methode pour calculer le prix total
        private decimal CalculateTotalPrice(List<Biere> bieres)
        {
            decimal prixTotal = 0;

            // Appliquer une réduction si le nombre total de bières est supérieur à 20
            if (bieres.Sum(b => b.Quantite) > 20)
            {
                decimal prixSum = bieres.Sum(b => (decimal)b.Prix);
                decimal quantiteSum = bieres.Sum(b => (decimal)b.Quantite);

                prixTotal = prixSum * quantiteSum * 0.8m; // 20% de réduction
                Console.WriteLine($"Biere ID: {prixTotal}, Grossiste ID: {prixSum}, Nouvelle Quantite: {quantiteSum}");
            }
            // Appliquer une réduction si le nombre total de bières est supérieur à 10
            else if (bieres.Sum(b => b.Quantite) > 10)
            {
                decimal prixSum = bieres.Sum(b => (decimal)b.Prix);
                decimal quantiteSum = bieres.Sum(b => (decimal)b.Quantite);

                prixTotal = prixSum * quantiteSum * 0.9m; // 10% de réduction
                Console.WriteLine($"Biere ID: {prixTotal}, Grossiste ID: {prixSum}, Nouvelle Quantite: {quantiteSum}");
            }
            else
            {
                decimal prixSum = bieres.Sum(b => (decimal)b.Prix);
                decimal quantiteSum = bieres.Sum(b => (decimal)b.Quantite);

                prixTotal = prixSum * quantiteSum;
                Console.WriteLine($"prixtotal: {prixTotal}, totalbiere: {prixSum}, Quantite: {quantiteSum}");
            }

            return prixTotal;
        }
    }
}
