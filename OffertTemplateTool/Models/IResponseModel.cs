using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.Connectors;

namespace OffertTemplateTool.Models
{
    public abstract class IResponseModel<T> where T : class
    {
        public List<T> ReponseModel { get; set;}
    }
}

