using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class CompanyInfo : IDb
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string StreetName { get; set; }
        public int StreetNumber { get; set; }
        public string ZipCode { get; set; }
        public string CityName { get; set; }
        public string Email { get; set; }
        public string CompanyWebsite { get; set; }
        public int KVKNumber { get; set; }
        public string IBANNumber { get; set; }
        public string BTWCode { get; set; }

    }
}
