using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OffertTemplateTool.DAL.Models
{
    public class Offers : IDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string DebtorNumber { get; set; } // voor WeFact API
        public int DocumentCode { get; set; } // ophalen vanuit WeFact
        public virtual Users CreatedBy { get; set; } // Welke user maakt 't aan
        public virtual Users UpdatedBy { get; set; } // Laatst geedit door?                         
        public DateTime? CreatedAt { get; set; } // Gemaakt op
        public DateTime? LastUpdatedAt { get; set; } // Elke x updaten
        public string IndexContent { get; set; } // Html text veld
        public virtual Estimates Estimate { get; set; } // Html text veld

    }
}
