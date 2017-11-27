using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.TemplateSevice
{
    public class TemplateService : ITemplateService
    {
        private IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceprovider;
        private readonly ITempDataProvider _tempDataProvoder;
        public TemplateService(IRazorViewEngine viewEngine, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _serviceprovider = serviceProvider;
            _tempDataProvoder = tempDataProvider;
        }
        public Task<string> RenderTemplateAsync<TViewModel>(string filename, TViewModel model)
        {
            return null;
        }
    }
}
