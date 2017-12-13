using OffertTemplateTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OffertTemplateTool.TemplateService
{
    public class PagesService
    {
        public List<PagesViewModel> GetPages(List<PagesViewModel> pages)
        {
            foreach (var item in pages)
            {
                RecursivePages(item);
                pages.Remove(item);
            }
           

            return pages;
        }

        public PagesViewModel RecursivePages(PagesViewModel page)
        {
            if (page.Type == TypeText.Text)
            {
                if (page.Text.Length >= 3700)
                {
                    var startpos = 0;
                    var substring = page.Text.Substring(3700);
                    page.Text = page.Text.Remove(3700);

                    var newpage = new PagesViewModel
                    {
                        Text = substring,
                         Footer = page.Footer,
                         Type = TypeText.Text
                    };
                    RecursivePages(newpage);

                    return page;
                }
                else
                {
                    return page;
                }
            }
            else
            {
                return page;
            }
           
        }
    }

}
