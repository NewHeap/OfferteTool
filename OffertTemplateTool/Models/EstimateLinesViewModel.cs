using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models
{
    public class EstimateLinesViewModel
    {
        public Guid Id { get; set; }
        public string Specification { get; set; }
        public decimal HourCost { get; set; }
        public double Hours { get; set; }
        public decimal TotalCost { get; set; }
    }
}
