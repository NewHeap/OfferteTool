using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OffertTemplateTool.DAL.Models
{
    public class Offer : IDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public int DebtorNumber { get; set; } // voor WeFact API
        public int DocumentCode { get; set; } // ophalen vanuit WeFact
        public Users CreatedBy { get; set; } // Welke user maakt 't aan
        public Users UpdatedBy { get; set; } // Laatst geedit door?                         
        public DateTime? CreatedAt { get; set; } // Gemaakt op
        public DateTime? LastUpdatedAt { get; set; } // Elke x updaten
        public string IndexContent { get; set; } // Html text veld
        public string SecondContent { get; set; } // Html text veld
        public string ProjectRequirements { get; set; } // Html text veld
    }
}
