using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models.wefactModels
{
    public class WefactInvoiceLine
    {
        public int Identifier { get; set; }
    }

    public class WefactInvoice
    {
        public string api_key { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string InvoiceCode { get; set; }
        public string Debtor { get; set; }
        [JsonProperty(PropertyName = "Concept")]
        public int Concept { get; set; }
        public int Identifier { get; set; }
        public List<WefactInvoiceLine> InvoiceLines { get; set; }
    }
}
