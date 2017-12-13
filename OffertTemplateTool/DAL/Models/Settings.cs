using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class Settings : IDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set;}
    }
}
