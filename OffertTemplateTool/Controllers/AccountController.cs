using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using OffertTemplateTool.DAL.Context;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using OffertTemplateTool.Models;

namespace OffertTemplateTool.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private UsersRepository UserRepository { get; set; }

        public AccountController(IRepository<Users> userRepository)
        {
            UserRepository = (UsersRepository)userRepository;
        }



        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> MyAccount()
        {
            var users = await UserRepository.GetAllAsync();
            var x = users.FirstOrDefault(u => u.Email == User.Identity.Name);
            var vm = new UsersViewModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Insertion = x.Insertion,
                Initials = x.Initials,
                Email = x.Email,
                Function = x.Function,
                PhoneNumber = x.PhoneNumber
            };
            return View(vm);
        }


        
        [HttpPost]
        public async Task<IActionResult> Register(UsersViewModel model)
        {
            if (ModelState.IsValid) {
                var query = new Users
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Insertion = model.Insertion,
                    Initials = model.Initials,
                    Email = model.Email,
                    Function = model.Function,
                    PhoneNumber = model.PhoneNumber
                };
                await UserRepository.AddAsync(query);

                return Redirect("../Home/Index");
            } else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (UserRepository.AnyUserByEmail(User.Identity.Name) == true)
            {
                return Redirect("../Home/index");
            }
            else
            {
                return View();
            }
        }
    }
}
