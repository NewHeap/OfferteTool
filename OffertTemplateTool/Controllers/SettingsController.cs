using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace OffertTemplateTool.Controllers
{
    public class SettingsController : Controller
    {
        private SettingsRepository SettingsRepository { get; set; }

        public SettingsController(IRepository<Settings> settingsrepository)
        {
            SettingsRepository = (SettingsRepository)settingsrepository;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await SettingsRepository.GetAllAsync();
            var result = settings.Select(x => new SettingsViewModel { Key = x.Key, Value = x.Value, Id = x.Id}).ToList();
            ViewData["settings"] = result;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(SettingsViewModel model)
        {
            if (ModelState.IsValid) {
                var settings = new Settings
                {
                    Key = model.Key,
                    Value = model.Value
                };
                await SettingsRepository.AddAsync(settings);
                return Redirect("../Settings");
            }
            else
            {
                return Redirect("../Settings");
            }  
        }

        public IActionResult Edit()
        {
            return View();
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            await SettingsRepository.RemoveAsync(id);
            return Redirect("../");
        }
    }
}