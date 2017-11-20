using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class EstimateLines : IDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Specification { get; set; }
        [Required]
        public float HourCost { get; set; }
        [Required]
        public double Hours { get; set; }
        [Required]
        public  float TotalCost { get; set; }
        public int WefactIdentifier { get; set; }
    }
}
