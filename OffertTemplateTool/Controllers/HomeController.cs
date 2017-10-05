using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.Models;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UsersRepository UsersRepository { get; set; }
        public HomeController(IRepository<Users> userrepository)
        {
            UsersRepository = (UsersRepository)userrepository;
        }

        public IActionResult Index()
        {
            if (UsersRepository.AnyUserByEmail(User.Identity.Name) == true)
            {
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

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
