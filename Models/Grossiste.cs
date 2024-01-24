using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webAPIBrasserie.Models
{
    public partial class Grossiste
    {
        public Grossiste()
        {
            Ventes = new HashSet<Vente>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Nom { get; set; }
        public int? Stock { get; set; }
        public double? Prix { get; set; }

        public virtual ICollection<Vente> Ventes { get; set; }
    }
}
