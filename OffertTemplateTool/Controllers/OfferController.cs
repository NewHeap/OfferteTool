using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using OffertTemplateTool.DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using OffertTemplateTool.Connectors;
using Microsoft.AspNetCore.Authorization;
using DinkToPdf;
using OffertTemplateTool.TemplateService;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        protected readonly OfferRepository OfferRepository;
        protected readonly UsersRepository UserRepository;
        protected readonly EstimateRepository EstimateRepository;
        protected readonly EstimateLinesRepository EstimateLinesRepository;
        protected readonly EstimateConnectsRepository EstimateConnectsRepository;
        protected readonly SettingsRepository SettingsRepository;
        protected readonly WeFactConnector Wefactconnector;
        protected readonly ITemplateService TemplateService;
        protected readonly IHostingEnvironment Environment;
        protected readonly IConverter ConverterService;

        public OfferController(IRepository<Offers> offerrepository,
            IRepository<Users> userrepository,
            IRepository<Offers> offerrepoitory,
            IRepository<Estimates> estimaterepository,
            IRepository<EstimateLines> estimatlinesrepository,
            IRepository<EstimateConnects> estimateconnectsrepository,
            IRepository<Settings> settingsrepository,
            IConnector wefactConnector,
            IHostingEnvironment env,
            ITemplateService templateservice,
            IConverter converterService)
        {
            OfferRepository = (OfferRepository)offerrepository;
            UserRepository = (UsersRepository)userrepository;
            EstimateRepository = (EstimateRepository)estimaterepository;
            EstimateLinesRepository = (EstimateLinesRepository)estimatlinesrepository;
            EstimateConnectsRepository = (EstimateConnectsRepository)estimateconnectsrepository;
            SettingsRepository = (SettingsRepository)settingsrepository;
            Wefactconnector = (WeFactConnector)wefactConnector;
            TemplateService = templateservice;
            Environment = env;
            ConverterService = converterService;
        }

        public async Task<IActionResult> Index()
        {
            ;
            if (User.Identity.Name != null)
            {
                var offertes = await OfferRepository.GetAllAsync();
                ViewData["offertes"] = offertes.Select(x => new OfferViewModel
                {
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    LastUpdatedAt = x.LastUpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    ProjectName = x.ProjectName,
                    Id = x.Id
                }).ToList();

                List<string> templates = new List<string>();
                var files = Directory.GetFiles(@"Views/Template/")
                    .Select(Path.GetFileName)
                    .ToArray();
                foreach (var item in files)
                {
                    var file = item.Replace(".cshtml", "");
                    if (file[0].ToString() != "~" && file[1].ToString() != "$")
                    {
                        templates.Add(file);
                    }
                }

                ViewData["templates"] = templates;

                return View();
            }
            else
            {
                return Redirect("~/Account/AccessDenied");
            }
        }

        [HttpPost]
        public async Task<IActionResult> OfferUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View("File not selected");
            }
            else
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Views/Template/",
                    file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return View();
            }
        }

        [HttpGet]
        public IActionResult OfferUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewOffer(OfferViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = UserRepository.FindUserByEmail(User.Identity.Name);
                //var customer = wefactconnector.GetCustomerInfo(model.DebtorNumber);
                var settingdocument = SettingsRepository.getSpecificSetting("DocumentCode");
                int documentcode = int.Parse(settingdocument.Value);
                var code = documentcode.ToString("000");

                var offerte = new Offers
                {
                    IndexContent = model.IndexContent,
                    ProjectName = model.ProjectName,
                    //DebtorNumber = model.DebtorNumber,
                    CreatedBy = user,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedBy = user,
                    DocumentCode = "PV" + code,
                    DocumentVersion = 1
                };

                await OfferRepository.AddAsync(offerte);

                documentcode++;
                settingdocument.Value = documentcode.ToString();
                await SettingsRepository.SaveChangesAsync();

                var est = new Estimates
                {

                };

                await EstimateRepository.AddAsync(est);

                foreach (var line in model.EstimateLines)
                {
                    var lin = new EstimateLines
                    {
                        HourCost = line.HourCost,
                        Hours = line.Hours,
                        Specification = line.Specification,
                        TotalCost = line.TotalCost
                    };

                    var connect = new EstimateConnects
                    {
                        Estimate = est,
                        EstimateLines = lin
                    };

                    await EstimateLinesRepository.AddAsync(lin);
                    await EstimateConnectsRepository.AddAsync(connect);
                }

                offerte.Estimate = est;
                await OfferRepository.UpdateAsync(offerte);

                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IActionResult NewOffer()
        {
            OfferViewModel model = new OfferViewModel
            {

            };
            //var debtorinfo = wefactconnector.GetCustomers();
            //ViewData["debtors"] = debtorinfo;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOffer(OfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("offermodel", "Form is not valid!");
                return View(model);
            }
            try
            {
                ICollection<EstimateConnects> Connect;
                ICollection<Offers> offers;


                using (var context = new DataBaseContext())
                {
                    offers = context.Offer
                   .Include(offermodel => offermodel.Estimate)
                   .Include(users => users.CreatedBy)
                   .Include(users => users.UpdatedBy)
                    .ToList();

                    Connect = context.EstimateConnects
                    .Include(line => line.EstimateLines)
                    .ToList();
                }

                Offers offer = await OfferRepository.FindAsync(model.Id);
                var dboffer = offers.FirstOrDefault(x => x.Id.Equals(model.Id));
                Users user = UserRepository.FindUserByEmail(User.Identity.Name);
                var estimate = await EstimateRepository.FindAsync(dboffer.Estimate.Id);
                var connectlines = Connect.Where(x => x.Estimate.Id.Equals(dboffer.Estimate.Id)).ToList();
                if (model.EstimateLines != null)
                {
                    foreach (var item in model.EstimateLines)
                    {
                        if (EstimateLinesRepository.AnyLineExist(item.Id) == true)
                        {
                            var line = await EstimateLinesRepository.FindAsync(item.Id);
                            line.Specification = item.Specification;
                            line.Hours = item.Hours;
                            line.HourCost = item.HourCost;
                            line.TotalCost = item.TotalCost;
                            await EstimateLinesRepository.SaveChangesAsync();
                        }
                        else
                        {
                            var newline = new EstimateLines
                            {
                                Specification = item.Specification,
                                Hours = item.Hours,
                                HourCost = item.HourCost,
                                TotalCost = item.TotalCost
                            };
                            await EstimateLinesRepository.AddAsync(newline);

                            var newconnect = new EstimateConnects
                            {
                                Estimate = estimate,
                                EstimateLines = newline
                            };
                            await EstimateConnectsRepository.AddAsync(newconnect);
                        }
                    }
                    foreach (var connectline in connectlines)
                    {
                        if (!model.EstimateLines.Any(x => x.Id == connectline.EstimateLines.Id))
                        {
                            await EstimateConnectsRepository.RemoveAsync(connectline.Id);
                        }
                    }
                }

                offer.IndexContent = model.IndexContent;
                offer.LastUpdatedAt = DateTime.Now;
                offer.UpdatedBy = user;
                offer.ProjectName = model.ProjectName;
                offer.DocumentVersion = offer.DocumentVersion + 1;
                //offer.DebtorNumber = model.DebtorNumber;

                await OfferRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ICollection<Offers> offers;
                ICollection<EstimateConnects> Connect;
                using (var context = new DataBaseContext())
                {
                    offers = context.Offer
                       .Include(offermodel => offermodel.Estimate)
                       .Include(user => user.CreatedBy)
                       .Include(user => user.UpdatedBy)
                        .ToList();

                    Connect = context.EstimateConnects
                        .Include(line => line.EstimateLines)
                        .ToList();
                }

                var offer = offers.FirstOrDefault(x => x.Id.Equals(model.Id));
                var estimate = await EstimateRepository.FindAsync(offer.Estimate.Id.ToString());
                var lines = Connect.Where(x => x.Estimate.Id == estimate.Id).ToList();

                ViewData["estimatelines"] = lines.Select(x => new EstimateConnectViewModel
                {
                    EstimateLines = x.EstimateLines
                }).ToList();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditOffer(Guid Id)
        {
            ICollection<Offers> offers;
            ICollection<EstimateConnects> Connect;
            using (var context = new DataBaseContext())
            {
                offers = context.Offer
                   .Include(offermodel => offermodel.Estimate)
                   .Include(user => user.CreatedBy)
                   .Include(user => user.UpdatedBy)
                    .ToList();

                Connect = context.EstimateConnects
                    .Include(line => line.EstimateLines)
                    .ToList();
            }

            var offer = offers.FirstOrDefault(x => x.Id.Equals(Id));
            var estimate = await EstimateRepository.FindAsync(offer.Estimate.Id.ToString());
            var lines = Connect.Where(x => x.Estimate.Id == estimate.Id).ToList();

            ViewData["estimatelines"] = lines.Select(x => new EstimateConnectViewModel
            {
                EstimateLines = x.EstimateLines
            }).ToList();

            OfferViewModel offerte = new OfferViewModel //get selected offer info
            {
                IndexContent = offer.IndexContent,
                ProjectName = offer.ProjectName,
                Estimate = offer.Estimate.Id,
                Id = offer.Id,
                //DebtorNumber = offer.DebtorNumber
            };

            //var debtorinfo = wefactconnector.GetCustomers(); //get debtors off wefact
            //ViewData["debtors"] = debtorinfo;
            return View(offerte);
        }

        public async Task<IActionResult> ExportOffer(Guid Id, string template)
        {
            var offerrender = new OfferRenderViewModel();
            var offer = await OfferRepository.FindAsync(Id);
            var viewmodelrender = await FillViewModel(Id, offerrender);
            var footercontent = await TemplateService.RenderTemplateAsync("Template/FooterTemplate", new Footer());
            viewmodelrender.FooterContent = footercontent;


            string documentcontent = await TemplateService.RenderTemplateAsync("Template/"+template, viewmodelrender);
            byte[] output = ConverterService.Convert(new HtmlToPdfDocument()
            {
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = documentcontent,
                    }
                }
            });
            Response.Clear();
            offer.IsOpen = 1;
            Response.ContentType = "Application/pdf";
            Response.Headers.Add("Content-Disposition", string.Format("Attachment;FileName=Offer_" + offer.ProjectName + ".pdf;"));
            Response.Headers.Add("Content-Length", output.Length.ToString());
            await OfferRepository.SaveChangesAsync();
            await Response.Body.WriteAsync(output, 0, output.Length);
            return Redirect(nameof(Index));
        }
        public async Task<IActionResult> DeleteOffer(Guid Id)
        {
            ICollection<Offers> offers;
            ICollection<EstimateConnects> Connect;
            using (var context = new DataBaseContext())
            {
                offers = context.Offer
                   .Include(offermodel => offermodel.Estimate)
                   .Include(user => user.CreatedBy)
                   .Include(user => user.UpdatedBy)
                    .ToList();

                Connect = context.EstimateConnects
                    .Include(line => line.EstimateLines)
                    .ToList();
            }
            var offer = offers.FirstOrDefault(x => x.Id.Equals(Id));
            var lines = Connect.Where(x => x.Estimate.Id == offer.Estimate.Id).ToList();

            foreach (var item in lines)
            {
                await EstimateConnectsRepository.RemoveAsync(item.Id);
            }

            await OfferRepository.RemoveAsync(Id);
            await EstimateRepository.RemoveAsync(offer.Estimate.Id);

            return Redirect("../");
        }

        public async Task<OfferRenderViewModel> FillViewModel(Guid Id, OfferRenderViewModel offerrender)
        {
            ICollection<EstimateConnects> Connect;
            ICollection<Offers> offers;

            using (var context = new DataBaseContext())
            {
                offers = context.Offer
               .Include(offermodel => offermodel.Estimate)
               .Include(users => users.CreatedBy)
               .Include(users => users.UpdatedBy)
                .ToList();

                Connect = context.EstimateConnects
                .Include(line => line.EstimateLines)
                .ToList();
            }

            Offers offer = await OfferRepository.FindAsync(Id);
            var dboffer = offers.FirstOrDefault(x => x.Id.Equals(Id));
            var estimate = await EstimateRepository.FindAsync(dboffer.Estimate.Id);
            var connectlines = Connect.Where(x => x.Estimate.Id.Equals(dboffer.Estimate.Id)).ToList();

            List<Match> alineas = Regex.Matches(offer.IndexContent, @"((<h1.*>)([\s\S])+?(?=<h1.*>)|((<h1.*>)([\s\S])+(<\/p>)))").ToList();
            List<PagesViewModel> pages = new List<PagesViewModel>();

            int pagenumber = 4;
            List<Dictionary<string, int>> indexlist = new List<Dictionary<string, int>>();
            foreach (var item in alineas)
            {
                var indexitem = Regex.Matches(item.ToString(), @"<h1>(.|\n)*?</h1>").ToList();
                var replaceindexitem = "";
                foreach (var h1 in indexitem)
                {
                    pagenumber++;
                    replaceindexitem = Regex.Replace(h1.ToString(), @"<[^>]*>", "");

                    if (item.Length >= 3700)
                    {
                        pagenumber++;
                    }

                    var Index = new Dictionary<string, int>
                    {
                    {replaceindexitem, pagenumber}
                    };

                    indexlist.Add(Index);
                }
            }
            indexlist.Add(new Dictionary<string, int> { { "Begroting", pagenumber+1 } });
            indexlist.Add(new Dictionary<string, int> { { "Voor Akkoord", pagenumber + 2 } });

            float exclbtw = 0;
            float btw = 0;
            float totalcost = 0;
            foreach (var item in connectlines)
            {
                exclbtw = exclbtw + item.EstimateLines.TotalCost;
            }
            btw = exclbtw / 100 * 21;
            totalcost = exclbtw + btw;

            #region
            var Page3ViewModel = new Page3ViewModel
            {
                CustomerZipCode = "7957EJ",
                CustomerStreet = "Oosterakker 21",
                CustomerName = "K.Broekstra",
                CustomerCompany = "Broekstra B.V.",
                CustomerEmail = "test@test.te",
                DocumentVersion = offer.DocumentVersion
            };

            var FrontPageViewModel = new FrontPageViewModel
            {
                CreatedBy = dboffer.CreatedBy.FirstName + " " + dboffer.CreatedBy.LastName,
                LastUpdated = offer.LastUpdatedAt.ToString(),
                ProjectName = offer.ProjectName,
                DocumentCode = offer.DocumentCode,
                CustomerZipCode = "7957EJ",
                CustomerStreet = "Oosterakker 21",
                CustomerName = "K.Broekstra",
                CustomerCompany = "Broekstra B.V.",
                CustomerCountry = "The Netherlands"
            };

            var IndexPage = new IndexPage
            {
                IndexItems = indexlist
            };

            var EstimateTablePage = new EstimateTablePage
            {
                EstimateConnects = connectlines,
                BTW = btw.ToString("#,##0.00"),
                ExclBtw = exclbtw.ToString("#,##0.00"),
                Totaal = totalcost.ToString("#,##0.00")
            };
            var ContentPages = new ContentPages
            {
                Alineas = alineas
            };
            offerrender.ContentPages = ContentPages;
            offerrender.EstimateTablePage = EstimateTablePage;
            offerrender.FrontPage = FrontPageViewModel;
            offerrender.Page3 = Page3ViewModel;
            offerrender.IndexPage = IndexPage;
#endregion

            return offerrender;
        }
    }
}
