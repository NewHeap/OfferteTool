using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models.wefactModels
{
    public class WefactDeleteInvoiceModel
    {
        public string api_key { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public int Identifier { get; set; }
    }
}
