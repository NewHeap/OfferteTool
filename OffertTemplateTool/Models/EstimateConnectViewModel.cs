using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Models;

namespace OffertTemplateTool.Models
{
    public class EstimateConnectViewModel
    {
        public Guid Id { get; set; }
        public virtual Estimates Estimate { get; set; }
        public virtual EstimateLines EstimateLines { get; set; }
    }
}
