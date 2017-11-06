using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using OffertTemplateTool.DAL;
using OffertTemplateTool.DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetOffice.WordApi;
using NetOffice.WordApi.Enums;
using System.Text.RegularExpressions;
using OffertTemplateTool.Connectors;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Text;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private OfferRepository OfferRepository { get; set; }
        private UsersRepository UserRepository { get; set; }
        private EstimateRepository EstimateRepository { get; set; }
        private EstimateLinesRepository EstimateLinesRepository { get; set; }
        private EstimateConnectsRepository EstimateConnectsRepository { get; set; }
        private SettingsRepository SettingsRepository { get; set; }
        internal WeFactConnector wefactconnector { get; }
        Application app;
        Document doc;
        string projectname;

        public OfferController(IRepository<Offers> offerrepository, IRepository<Users> userrepository, IRepository<Estimates> estimaterepository,
            IRepository<EstimateLines> estimatlinesrepository, IRepository<EstimateConnects> estimateconnectsrepository,
            IRepository<Settings> settingsrepository,
            IConnector wefactConnector)
        {
            OfferRepository = (OfferRepository)offerrepository;
            UserRepository = (UsersRepository)userrepository;
            EstimateRepository = (EstimateRepository)estimaterepository;
            EstimateLinesRepository = (EstimateLinesRepository)estimatlinesrepository;
            EstimateConnectsRepository = (EstimateConnectsRepository)estimateconnectsrepository;
            SettingsRepository = (SettingsRepository)settingsrepository;
            wefactconnector = (WeFactConnector)wefactConnector;
            app = new Application();
        }

        public async Task<IActionResult> Index()
        {
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
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/OfferteTemplates",
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
                var customer = wefactconnector.GetCustomerInfo(model.DebtorNumber);

                var offerte = new Offers
                {
                    IndexContent = model.IndexContent,
                    ProjectName = model.ProjectName,
                    DebtorNumber = model.DebtorNumber,
                    CreatedBy = user,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedBy = user,
                };

                await OfferRepository.AddAsync(offerte);

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
            var debtorinfo = wefactconnector.GetCustomers();
            ViewData["debtors"] = debtorinfo;

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
                Offers offer = await OfferRepository.FindAsync(model.Id);
                Users user = UserRepository.FindUserByEmail(User.Identity.Name);
                var estimate = await EstimateRepository.FindAsync(model.Estimate);

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
                }

                offer.IndexContent = model.IndexContent;
                offer.LastUpdatedAt = DateTime.Now;
                offer.UpdatedBy = user;
                offer.ProjectName = model.ProjectName;
                offer.DebtorNumber = model.DebtorNumber;
                offer.DocumentCode = model.DocumentCode;

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
                DebtorNumber = offer.DebtorNumber
            };

            var debtorinfo = wefactconnector.GetCustomers(); //get debtors off wefact
            ViewData["debtors"] = debtorinfo;
            return View(offerte);
        }

        public async Task<IActionResult> ExportOffer(Guid Id, bool download)
        {
            var rows = 1;
            decimal totalcost = 0;
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
            var debtor = wefactconnector.GetCustomerInfo(offer.DebtorNumber);
            projectname = offer.ProjectName;

            DateTime lastupdate = DateTime.Parse(offer.LastUpdatedAt.ToString());

            doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/OfferteTemplates/NewHeapTemplate.docx"));
            app.Visible = false;
            doc.Activate();

            FindAndReplace("<CustomerCompany>", debtor.CompanyName, false);
            FindAndReplace("<CustomerName>", debtor.Initials + ". " + debtor.SurName, false);
            FindAndReplace("<CustomerStreet>", debtor.Address, false);
            FindAndReplace("<CustomerZipCode>", debtor.ZipCode + " " + debtor.City, false);
            FindAndReplace("<Customercountry>", debtor.Country, false);
            //projectinfo
            FindAndReplace("<ProjectName>", offer.ProjectName, false);
            FindAndReplace("<LastUpdated>", lastupdate.ToShortDateString().ToString(), false);
            FindAndReplace("<CreatedBy>", offer.CreatedBy.FirstName, false);
            FindAndReplace("<ProjectName>", offer.ProjectName, false);
            //customer info relatie
            FindAndReplace("<CustomerCompany>", debtor.CompanyName, false);
            FindAndReplace("<CustomerName>", debtor.Initials + ". " + debtor.SurName, false);
            FindAndReplace("<CustomerStreet>", debtor.Address, false);
            FindAndReplace("<CustomerZipCode>", debtor.ZipCode + " " + debtor.City, false);
            FindAndReplace("<EmailCustomer>", debtor.EmailAddress, false);

            //Estimate table
            app.Selection.Find.Execute("<Estimate>");
            app.Selection.InsertBreak(WdBreakType.wdLineBreak);
            Table table = doc.Tables.Add(app.Selection.Range, lines.Count + 1, 4);
            table.Style = WdBuiltinStyle.wdStyleTableLightShading;

            table.Cell(rows, 1).Select();
            app.Selection.TypeText("Specification");

            table.Cell(rows, 2).Select();
            app.Selection.TypeText("HourCost");

            table.Cell(rows, 3).Select();
            app.Selection.TypeText("Hours");

            table.Cell(rows, 4).Select();
            app.Selection.TypeText("TotalCost");

            foreach (var item in lines)
            {
                rows++;

                table.Cell(rows, 1).Select();
                app.Selection.TypeText(item.EstimateLines.Specification);

                table.Cell(rows, 2).Select();
                app.Selection.TypeText("\u20AC" + item.EstimateLines.HourCost.ToString());

                table.Cell(rows, 3).Select();
                app.Selection.TypeText(item.EstimateLines.Hours.ToString());

                table.Cell(rows, 4).Select();
                app.Selection.TypeText("\u20AC" + item.EstimateLines.TotalCost.ToString("#,##0.00"));

                totalcost = totalcost + item.EstimateLines.TotalCost;
            }
            var btwpercentage = SettingsRepository.getBTW();

            //totalcost table
            app.Selection.Find.Execute("<TotalsCosts>");
            Table tableTotal = doc.Tables.Add(app.Selection.Range, 3, 2);
            tableTotal.Style = WdBuiltinStyle.wdStyleTableLightShading;
            tableTotal.PreferredWidth = 140;
            tableTotal.Rows.Alignment = WdRowAlignment.wdAlignRowRight;

            tableTotal.Cell(1, 1).Select();
            app.Selection.TypeText("excl. btw");

            tableTotal.Cell(1, 2).Select();
            app.Selection.TypeText("\u20AC" + totalcost.ToString("#,##0.00"));

            tableTotal.Cell(2, 1).Select();
            app.Selection.TypeText("btw " + btwpercentage.Value + "%");
            var btwdecimal = decimal.Parse(btwpercentage.Value);

            decimal btw = totalcost / 100 * btwdecimal;
            tableTotal.Cell(2, 2).Select();
            app.Selection.TypeText("\u20AC" + btw.ToString("#,##0.00"));

            tableTotal.Cell(3, 1).Select();
            app.Selection.TypeText("Subtotaal");

            tableTotal.Cell(3, 2).Select();
            app.Selection.TypeText("\u20AC" + (totalcost + btw).ToString("#,##0.00"));
            doc.SaveAs2(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".docx"));
            doc.Close();

            doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".docx"));
            FindAndReplace("<Content>", offer.IndexContent, true);

            doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".html"));
            doc.SaveAs2(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".pdf"), WdSaveFormat.wdFormatPDF);
            doc.Close();
            app.Quit();

            if (download == true)
            {
                Response.Clear();
                Response.ContentType = "Application/pdf";
                Response.Headers.Add("Content-Disposition", string.Format("Attachment;FileName=Offer" + offer.ProjectName + ".pdf;"));

                byte[] arr = System.IO.File.ReadAllBytes(@"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".pdf");
                Response.Headers.Add("Content-Length", arr.Length.ToString());
                await Response.Body.WriteAsync(arr, 0, arr.Length);
                Response.Clear();
            }
            return Redirect("../offer");
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

        //find and replace function 
        private void FindAndReplace(string find, string replace, bool indexcontent)
        {
            if (indexcontent == true)
            {
                var h1tags = Regex.Matches(replace, @"<h1>(.|\n)*?</h1>");
                var ptags = Regex.Matches(replace, @"<p>(.|\n)*?</p>");
                List<string> pagenumbers = new List<string>();
                var characters = "";

                app.Selection.Find.Execute("<Index>");
                for (int i = 0; i < h1tags.Count; i++)
                {
                    app.Selection.Font.Name = "Courier new";
                    int h1tagslength = Regex.Replace(h1tags[i].Value, @"<[^>]*>", "").Length;
                    int charrepeat = 67 - h1tagslength;
                    for (int c = 0; c < charrepeat; c++)
                    {
                        characters += ".";
                    }

                    ContentStyle(12, 0);
                    app.Selection.TypeText(Regex.Replace(h1tags[i].Value, @"<[^>]*>", "") + characters + "<PageNumber>");
                    app.Selection.InsertBreak(WdBreakType.wdLineBreak);
                    characters = "";
                }

                for (int i = 0; i < h1tags.Count; i++)
                {
                    app.Selection.Font.Name = "Calibri";
                    app.Selection.Find.Execute(find);

                    
                    app.Selection.TypeText(h1tags[i].Value);
                    app.Selection.InsertBreak(WdBreakType.wdLineBreak);

                    app.Selection.TypeText(ptags[i].Value);
                    pagenumbers.Add(app.Selection.Information(WdInformation.wdActiveEndAdjustedPageNumber).ToString());
                    if (i != h1tags.Count-1)
                    {
                        app.Selection.InsertBreak();
                    }
                    
                }

                doc.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + projectname + ".html"), fileFormat: WdSaveFormat.wdFormatHTML);
                doc.Close();

                
                HtmlConverter(@"wwwroot/Exporteoffers/Offer" + projectname + ".html");
                doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + projectname + ".html"));
            }
            else
            {
                app.Selection.Find.Execute(find);
                app.Selection.TypeText(replace);
            }
        }

        private void ContentStyle(int fontsize, int fontbold)
        {
            app.Selection.Font.Size = fontsize;
            app.Selection.Font.Bold = fontbold;
        }

        private void HtmlConverter(string FilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string[] lines = System.IO.File.ReadAllLines(FilePath, Encoding.GetEncoding(1252));

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("&lt;"))
                {
                    lines[i] = lines[i].Replace("&lt;", "<");
                }
                if (lines[i].Contains("&gt;"))
                {
                    lines[i] = lines[i].Replace("&gt;", ">");
                }
                if (lines[i].Contains("<h1>"))
                {
                    lines[i] = lines[i].Replace("<h1>", "<h1 style='margin:0; color:red; font-size: 20px;'>");
                }
            }

            System.IO.File.WriteAllLines(FilePath, lines, Encoding.GetEncoding(1252));
        }
    }
}