using System;
using System.Collections.Generic;
using OffertTemplateTool.DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OffertTemplateTool.Models
{
    public class OfferRenderViewModel
    {
        public FrontPageViewModel FrontPage { get; set; }
        public Page3ViewModel Page3 { get; set; }
        public ContentPages ContentPages { get; set; }
        public EstimateTablePage EstimateTablePage { get; set; }
        public IndexPage IndexPage { get; set; }
        public string FooterContent { get; set; }
    }

    public class Page3ViewModel
    {
        public string CustomerCompany { get; set; }
        public string CustomerName { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerEmail { get; set; }
        public int DocumentVersion { get; set; }
    }

    public class FrontPageViewModel
    {
        public string CustomerCompany { get; set; }
        public string CustomerName { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerCountry { get; set; }
        public string ProjectName { get; set; }
        public string LastUpdated { get; set; }
        public string DocumentCode { get; set; }
        public string CreatedBy { get; set; }
    }
    public class IndexPage
    {
        public List<Dictionary<string, int>> IndexItems { get; set; }
    }

    public class ContentPages
    {
        public List<Match> Alineas { get; set; }
    }
    public class EstimateTablePage
    {
        public ICollection<EstimateConnects> EstimateConnects { get; set; }

        public string ExclBtw { get; set; }
        public string BTW { get; set; }
        public string Totaal { get; set; }
    }

    public class Footer
    {
        public string KvK { get; set; }
        public string Bank { get; set; }
        public string BTW { get; set; }
    }
}
