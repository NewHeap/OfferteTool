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

namespace OffertTemplateTool.Controllers
{
    public class OfferController : Controller
    {
        private OfferRepository OfferRepository { get; set; }
        private UsersRepository UserRepository { get; set; }
        private EstimateRepository EstimateRepository { get; set; }
        private EstimateLinesRepository EstimateLinesRepository { get; set; }
        private EstimateConnectsRepository EstimateConnectsRepository { get; set; }

        public OfferController(IRepository<Offers> offerrepository, IRepository<Users> userrepository, IRepository<Estimates> estimaterepository ,
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
        public IActionResult EditOffer(OfferViewModel model)
        {
            return Ok();
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
              
            ViewData["estimatelines"] = lines.Select(x => new EstimateConnectViewModel {
                EstimateLines = x.EstimateLines
            }).ToList();

            OfferViewModel offerte = new OfferViewModel {
                IndexContent = offer.IndexContent,
                ProjectName = offer.ProjectName,  
            };
            return View(offerte);
        }

        public async Task<IActionResult> ExportOffer(Guid Id)
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

            DateTime lastupdate = DateTime.Parse(offer.LastUpdatedAt.ToString());

            var app = new Application();
            var doc = app.Documents.Open(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/OfferteTemplates/NewHeapTemplate.docx"));

            doc.Activate();
            this.FindAndReplace(app, "<ProjectName>", offer.ProjectName);
            this.FindAndReplace(app, "<LastUpdated>", lastupdate.ToShortDateString());
            this.FindAndReplace(app, "<CreatedBy>", offer.CreatedBy.FirstName);
            this.FindAndReplace(app, "<IndexContent>", offer.IndexContent);
            this.FindAndReplace(app, "<Estimate>", );
            
            
             

            doc.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Exporteoffers/Offer"+ offer.ProjectName +".docx"));
            doc.Close();

            return View();
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
            object visible = true;
            object replace = 2;
            object wrap = 1;

            app.Selection.Find.Execute(find,
                 matchCase,  matchWholeWord,
                 matchWildCards,  matchSoundsLike,
                 nmatchAllWordForms,  forward,
                 wrap,  format,  replacewith,
                 replace,  matchKashida,
                 matchDiacritics,  matchAlefHamza, 
                 matchControl);
        }

    }
}