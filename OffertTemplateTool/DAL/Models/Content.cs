using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class Content : IDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string DocumentCode { get; set; }
    }
}
