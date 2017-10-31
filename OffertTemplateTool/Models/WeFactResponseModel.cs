using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.Connectors;

namespace OffertTemplateTool.Models
{
    public class WeFactResponseModel
    {
        public List<WeFactDebtorsModel> Debtors { get; set; }   
        public WeFactDebtorsModel Debtor { get; set; }
    }
}
