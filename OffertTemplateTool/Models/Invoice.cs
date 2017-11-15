using Newtonsoft.Json;
using System.Collections.Generic;

namespace OffertTemplateTool.Models
{
    public class Invoice
    {
        public string api_key { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string InvoiceCode { get; set; }
        public string Debtor { get; set; }
        [JsonProperty(PropertyName = "Concept")]
        public int Concept { get; set; }
        public List<Dictionary<string, string>> InvoiceLines { get; set; }
    }
}
