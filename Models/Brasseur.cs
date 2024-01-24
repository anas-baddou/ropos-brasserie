using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webAPIBrasserie.Models
{
    public partial class Brasseur
    {
        public Brasseur()
        {
            Bieres = new HashSet<Biere>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Adresse { get; set; }

        public virtual ICollection<Biere> Bieres { get; set; }
    }
}
