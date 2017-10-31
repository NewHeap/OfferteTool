using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models
{
    public class SettingsViewModel
    {
        public Guid Id { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Key { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Value { get; set; }
    }
}
