using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;
using OffertTemplateTool.Connectors;
using OffertTemplateTool.Models;
using Microsoft.AspNetCore.Authorization;

namespace OffertTemplateTool.Controllers
{
    [Authorize]
    public class WeFactController : Controller 
    {
        internal WeFactConnector WeFactConnector { get; }
        public WeFactController(IConnector wefactConnector)
        {
            WeFactConnector = (WeFactConnector)wefactConnector;
        }

        public IActionResult Index()
        {
            var contactList = WeFactConnector.GetCustomers();
            return View(contactList);
        }
    }
}