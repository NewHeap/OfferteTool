using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace OffertTemplateTool.TemplateService
{
    public class TemplateServiceClass : ITemplateService
    {
        private IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceprovider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHostingEnvironment _env;
        public TemplateServiceClass(IRazorViewEngine viewEngine, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _serviceprovider = serviceProvider;
            _tempDataProvider = tempDataProvider;
        }
        public async Task<string> RenderTemplateAsync<T>(string filename, T viewmodel, bool isFullPathProvider = false)
        {
            var httpcontext = new DefaultHttpContext
            {
                RequestServices = _serviceprovider
            };

            var actionContext = new ActionContext(httpcontext, new RouteData(), new ActionDescriptor());
            using (var outputWriter = new StringWriter())
            {
                var ViewResult = _viewEngine.FindView(actionContext, filename, false);
                if (ViewResult.View == null)
                {
                    throw new ArgumentNullException($"{filename} does not match with available view");
                }
                var viewDictionary = new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewmodel
                };
                var tempDataDictionary = new TempDataDictionary(httpcontext, _tempDataProvider);

                var ViewContext = new ViewContext(
                    actionContext, 
                    ViewResult.View, 
                    viewDictionary, 
                    tempDataDictionary, 
                    outputWriter, new HtmlHelperOptions());

                await ViewResult.View.RenderAsync(ViewContext);
                return outputWriter.ToString();
            }

        }
    }
}
