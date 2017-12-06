using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.TemplateService;
using OffertTemplateTool.Models;
using DinkToPdf;
using Microsoft.AspNetCore.Http;

namespace OffertTemplateTool.Controllers
{
    public class RenderTestController : Controller
    {
        protected readonly ITemplateService TemplateService;

        public RenderTestController(ITemplateService templateService)
        {
            TemplateService = templateService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new OfferRenderViewModel {
                ContentPages = new ContentPages
                {

                },
                EstimateTablePage = new EstimateTablePage
                {
                    
                },
                FrontPage = new FrontPageViewModel
                {
                    CreatedBy = "Thomas",
                    CustomerCompany = "Aap B.V.",
                    CustomerCountry = "Turkmenistan",
                    CustomerName = "AAAA",
                    LastUpdated = DateTime.Now.ToShortDateString(),
                    ProjectName = "Test"
                }
            };
            var html = await TemplateService.RenderTemplateAsync<OfferRenderViewModel>("Template/NewHeapTemplate", vm);
            var doc = new HtmlToPdfDocument()
            {
                 GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };
            var converter = new SynchronizedConverter(new PdfTools());
            byte[] pdf = converter.Convert(doc);

            Response.Clear();
            Response.ContentType = "Application/pdf";
            Response.Headers.Add("Content-Disposition", string.Format("Attachment;FileName=OfferTEst.pdf;"));
            Response.Headers.Add("Content-Length", pdf.Length.ToString());
            await Response.Body.WriteAsync(pdf, 0, pdf.Length);
            Response.Clear();

            return Ok();
        }
    }
}