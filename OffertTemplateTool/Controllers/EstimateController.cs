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
    public class EstimateController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateEstimateLines(EstimateLinesViewModel model)
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateEstimateLines()
        {
            return View();
        }
    }
}