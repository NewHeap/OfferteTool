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
using Microsoft.AspNetCore.Http;

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
            var offertes = await OfferRepository.FindAsync(Id);
            OfferViewModel offerte = new OfferViewModel {
                IndexContent = offertes.IndexContent,
                ProjectName = offertes.ProjectName,  
               

            };
            return View(offerte);
        }

    }
}