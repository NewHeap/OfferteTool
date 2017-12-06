using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using Microsoft.AspNetCore.Http;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UsersRepository UsersRepository { get; set; }
        OfferRepository OfferRepository { get; set; }

        public HomeController(IRepository<Users> userrepository, IRepository<Offers> offerrepository)
        {
            UsersRepository = (UsersRepository)userrepository;
            OfferRepository = (OfferRepository)offerrepository;
           
        }

        public async Task<IActionResult> Index()
        {
            List<string> templates = new List<string>();
            List<Offers> offeropen = new List<Offers>();
            List<Offers> offerexported = new List<Offers>();
            if (UsersRepository.AnyUserByEmail(User.Identity.Name) == true)
            {
                var offers = await OfferRepository.GetAllAsync();
                foreach (var item in offers)
                {
                    if (item.IsOpen == 0)
                    {
                        offeropen.Add(item);
                    }
                    else
                    {
                        offerexported.Add(item);
                    }
                }
                ViewData["open"] = offeropen;
                ViewData["exported"] = offerexported;

                var files = Directory.GetFiles(@"wwwroot/OfferteTemplates/")
                    .Select(Path.GetFileName)
                    .ToArray();
                foreach (var item in files)
                {
                    var file = item.Replace(".docx", "");
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
                return Redirect("../Account/Register");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult DeleteTemplate(string filename)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/OfferteTemplates/" + filename + ".docx");
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
                return Redirect(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
