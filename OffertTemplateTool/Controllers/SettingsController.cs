using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private SettingsRepository SettingsRepository { get; set; }

        public SettingsController(IRepository<Settings> settingsrepository)
        {
            SettingsRepository = (SettingsRepository)settingsrepository;
        }

        public async Task<IActionResult> Keywords()
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
                return Redirect("../Settings/keywords");
            }
            else
            {
                return Redirect("../Settings/KeyWords");
            }  
        }

        public async Task<IActionResult> Edit(SettingsViewModel model)
        {
            var setting = await SettingsRepository.FindAsync(model.Id);
            setting.Key = model.Key;
            setting.Value = model.Value;
            await SettingsRepository.SaveChangesAsync();
            return Redirect("../KeyWords");
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            await SettingsRepository.RemoveAsync(id);
            return Redirect("../KeyWords");
        }

        public IActionResult PlaceHolders()
        {
            return View();
        }
        
        public async Task<Settings> GetSetting(Guid id)
        {
            return await SettingsRepository.FindAsync(id);
        }
    }
}