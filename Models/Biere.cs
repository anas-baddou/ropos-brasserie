using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webAPIBrasserie.Models
{
    public partial class Biere
    {
        public Biere()
        {
            Ventes = new HashSet<Vente>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Nom { get; set; }
        public double? Quantite { get; set; }
        public double? Prix { get; set; }
        public int? BrasseurId { get; set; }
        public double? DegresAlcool { get; set; }

        public virtual Brasseur? Brasseur { get; set; }
        public virtual ICollection<Vente> Ventes { get; set; }
    }
}
