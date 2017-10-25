using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;

namespace OffertTemplateTool.Controllers
{
    public class WeFactController : Controller
    {
        string baseurl = "http://localhost/wefact/Pro";
        public IActionResult Index()
        {
            var client = new RestClient();
            client.BaseUrl = new System.Uri(baseurl);

            var request = new RestRequest();
            request.Resource = "api/debtor/debtor.list.php";
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);
            var test = response.Headers;
            var wefactinfo = response.Content;
            var jsoninfo = JsonConvert.DeserializeObject(wefactinfo);
            
            foreach (object item in jsoninfo)
            {

            }
            return View();
        }
    }
}