using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.Models;

namespace OffertTemplateTool.TemplateSevice
{
    public interface ITemplateService
    {
        Task<string> RenderTemplateAsync<T>(string filename, T model, bool isFullPathProvider = false);
    }
}
