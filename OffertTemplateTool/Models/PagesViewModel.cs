using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models
{
    public class PagesViewModel
    {
        public string Text { get; set; }
        public string Footer { get; set; }
        public TypeText Type { get;set; }
    }

    public enum TypeText
    {
        Header = 0x001,
        Text = 0x002,
        Footer = 0x003,
        prepace = 0x004
    }
}
