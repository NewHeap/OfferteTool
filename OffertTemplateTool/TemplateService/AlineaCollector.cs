using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OffertTemplateTool.TemplateService
{
    //public class Alinea
    //{
    //    public string _alinea { get; set; }
    //    public Alinea(string alinea)
    //    {
    //        _alinea = alinea;
    //    }

    //    public string GetAlinea()
    //    {
    //        return _alinea;
    //    }
    //}


    public class AlineaCollector
    {
        public List<Match> _alineas { get; set; }
        public AlineaCollector(List<Match> alineas) 
        {
            _alineas = alineas;
        }

        public List<Dictionary<string, int>> CreateTableOfContents()
        {
            var pagenumber = 4;
            List<Dictionary<string, int>> tableofcontent = new List<Dictionary<string, int>>();
            foreach (var item in _alineas)
            {
                pagenumber++;
                var Lenght = item.Length;

                var dictionary = new Dictionary<string, int>
                {
                    {item.ToString(), pagenumber}
                };
                if (Lenght >= 2000)
                {
                    pagenumber++;
                }
                tableofcontent.Add(dictionary);
            }
            return tableofcontent;
        }
    }
}
