using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace OffertTemplateTool.Models
{
    public class OfferViewModel
    {
        public Guid Id { get; set; }
        public string DebtorNumber { get; set; } // voor WeFact API
        public string DocumentCode { get; set; } // ophalen vanuit WeFact
        [Required]
        public string ProjectName { get; set; }
        public Users CreatedBy { get; set; } // Welke user maakt 't aan
        public Users UpdatedBy { get; set; } // Laatst geedit door?                         
        public DateTime? CreatedAt { get; set; } // Gemaakt op
        public DateTime? LastUpdatedAt { get; set; } // Elke x updaten
        public string IndexContent { get; set; } // Html text veld
        public Guid Estimate { get; set; } // Html text veld
        public int DocumentVersion { get; set; }
       
        public ICollection<EstimateLinesViewModel> EstimateLines { get; set; }
       
    }
}
