using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class EstimateConnects : IDb
    {
        public Guid Id { get; set; }
        public virtual Estimates Estimate { get; set; }
        public virtual EstimateLines EstimateLines { get; set; }
    }
}
