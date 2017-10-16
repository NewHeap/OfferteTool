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

namespace OffertTemplateTool.Controllers
{
    public class OfferController : Controller
    {
        private OfferRepository OfferRepository { get; set; }
        private UsersRepository UserRepository { get; set; }
        private EstimateRepository EstimateRepository { get; set; }
        private EstimateLinesRepository EstimateLinesRepository { get; set; }
        private EstimateConnectsRepository EstimateConnectsRepository { get; set; }

        public OfferController(IRepository<Offers> offerrepository, IRepository<Users> userrepository, IRepository<Estimates> estimaterepository,
            IRepository<EstimateLines> estimatlinesrepository, IRepository<EstimateConnects> estimateconnectsrepository)
        {
            OfferRepository = (OfferRepository)offerrepository;
            UserRepository = (UsersRepository)userrepository;
            EstimateRepository = (EstimateRepository)estimaterepository;
            EstimateLinesRepository = (EstimateLinesRepository)estimatlinesrepository;
            EstimateConnectsRepository = (EstimateConnectsRepository)estimateconnectsrepository;
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

                var offerte = new Offers
                {
                    IndexContent = model.IndexContent,
                    ProjectName = model.ProjectName,
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

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOffer(OfferViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                Offers offer = await OfferRepository.FindAsync(model.Id);
                Users user = UserRepository.FindUserByEmail(User.Identity.Name);
                var estimate = await EstimateRepository.FindAsync(model.Estimate);

                if (model.EstimateLines != null)
                {
                    foreach (var item in model.EstimateLines)
                    {
                        var line = await EstimateLinesRepository.FindAsync(item.Id);
                        line.Specification = item.Specification;
                        line.Hours = item.Hours;
                        line.HourCost = item.HourCost;
                        line.TotalCost = item.TotalCost;
                        await EstimateLinesRepository.SaveChangesAsync();

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

            OfferViewModel offerte = new OfferViewModel
            {
                IndexContent = offer.IndexContent,
                ProjectName = offer.ProjectName,
                Estimate = offer.Estimate.Id,
                Id = offer.Id
            };
            return View(offerte);
        }

        public async Task<IActionResult> ExportOffer(Guid Id)
        {
            var rows = 0;
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



            DateTime lastupdate = DateTime.Parse(offer.LastUpdatedAt.ToString());

            var app = new Application();
            var doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/OfferteTemplates/NewHeapTemplate.docx"));
            doc.Activate();

            this.FindAndReplace(app, "<ProjectName>", offer.ProjectName);
            this.FindAndReplace(app, "<LastUpdated>", lastupdate.ToShortDateString());
            this.FindAndReplace(app, "<CreatedBy>", offer.CreatedBy.FirstName);
            this.FindAndReplace(app, "<IndexContent>", offer.IndexContent);


            app.Selection.Find.Execute("<Estimate>");
            Table table = doc.Tables.Add(app.Selection.Range, lines.Count, 4);

            foreach (var item in lines)
            {
                rows++;

                table.Cell(rows, 1).Select();
                app.Selection.TypeText(item.EstimateLines.Specification);

                table.Cell(rows, 2).Select();
                app.Selection.TypeText(item.EstimateLines.Hours.ToString());

                table.Cell(rows, 3).Select();
                app.Selection.TypeText(item.EstimateLines.HourCost.ToString());

                table.Cell(rows, 4).Select();
                app.Selection.TypeText(item.EstimateLines.TotalCost.ToString());

                table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;


            }

            doc.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer" + offer.ProjectName + ".docx"));
            doc.Close();
            app.Quit();


            return View();
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
        
        private void FindAndReplace(Application app, object find, object replacewith)
        {
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object nmatchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = false;
            object replace = 2;
            object wrap = 1;

            app.Selection.Find.Execute(find,
                 matchCase, matchWholeWord,
                 matchWildCards, matchSoundsLike,
                 nmatchAllWordForms, forward,
                 wrap, format, replacewith,
                 replace, matchKashida,
                 matchDiacritics, matchAlefHamza,
                 matchControl);
        }

    }
}