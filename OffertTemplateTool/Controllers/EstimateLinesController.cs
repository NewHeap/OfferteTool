using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Repositories;
using OffertTemplateTool.DAL.Models;

namespace OffertTemplateTool.Controllers
{
    public class EstimateLinesController : Controller
    {
        EstimateLinesRepository EstimateLinesRepository { get; set; }

        public EstimateLinesController(IRepository<EstimateLines> EstimateLinesRepository)
        {
            EstimateLinesRepository = (EstimateLinesRepository)EstimateLinesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(EstimateLinesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Line = new EstimateLines
                {
                    Specification = model.Specification,
                    HourCost = model.HourCost,
                    Hours = model.Hours,
                    TotalCost = model.TotalCost
                };
                await EstimateLinesRepository.AddAsync(Line);

                return Redirect("../Estimate/Create/");
            }
            else
            {
                return View();
            }
          
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}