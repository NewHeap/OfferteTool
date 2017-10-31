using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OffertTemplateTool.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OffertTemplateTool.Connectors
{
    public class WeFactConnector : IConnector
    {
        internal string ApiKey { get; }
        internal string ApiUrl { get; }
        internal IConfiguration Configuration { get; }
        
        public WeFactConnector(IConfiguration configuration)
        {
            Configuration = configuration;
            ApiKey = Configuration["WeFact:Key"];
            ApiUrl = Configuration["WeFact:Url"];
        }

        public WeFactDebtorsModel GetCustomerInfo(string debtorcode)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);

            request.AddParameter("api_key", ApiKey);
            request.AddParameter("controller", "debtor");
            request.AddParameter("action", "show");
            request.AddParameter("DebtorCode", debtorcode);

            var response = client.Execute<WeFactResponseModel>(request);
            if(response.StatusCode == (HttpStatusCode)200)
            {
                var json = JsonConvert.DeserializeObject(response.Content);
                return response.Data.Debtor;
            }
            return null;
        }

        public ICollection<WeFactDebtorsModel> GetCustomers()
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);

            request.AddParameter("api_key", ApiKey);
            request.AddParameter("controller", "debtor");
            request.AddParameter("action", "list");

            var response = client.Execute<WeFactResponseModel>(request);
            if (response.StatusCode == (HttpStatusCode)200)
            {
                return response.Data.Debtors;
            }
            return null;
        }
    }
}
