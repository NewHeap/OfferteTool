using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models
{
    public class HtmlToPdfModel<T> where T : class
    {
        public string Name { get; set; }
        public IList<HtmlToPdfPage> Pages { get; set; }
        public HtmlToPdfFooter Footer { get; set; }
        public string FooterContent { get; set; }
        public T ViewModel { get; set; }

        public IEnumerable<HtmlToPdfPage> GetPages(HtmlToPdfPageType type)
        {
            return Pages.Where(x => x.Type == type).OrderBy(x => x.Index);
        }

    }

    public class HtmlToPdfPage
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public HtmlToPdfPageType Type { get; set; }
        public int Index { get; set; }
    }

    public class HtmlToPdfFooter
    {
        public string Bank { get; set; }
        public string Kvk { get; set; }
        public string Btw { get; set; }
    }

    public enum HtmlToPdfPageType
    {
        Text = 0x001,
        Index = 0x002,
        Preface = 0x003,
        FrontPage = 0x004,
        PrefaceInfo = 0x005,
        Estimate = 0x006,
        Agree = 0x007  
    }
}
