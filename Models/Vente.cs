using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webAPIBrasserie.Models
{
    public partial class Vente
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? MontantHtva { get; set; }
        public int? BiereId { get; set; }
        public int? GrossisteId { get; set; }
        public int? Qtevendue { get; set; }

        public virtual Biere? Biere { get; set; }
        public virtual Grossiste? Grossiste { get; set; }
    }
}
