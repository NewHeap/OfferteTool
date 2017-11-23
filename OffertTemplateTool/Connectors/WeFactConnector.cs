using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OffertTemplateTool.Models;
using OffertTemplateTool.Models.wefactModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Repositories;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace OffertTemplateTool.Connectors
{
    public class WeFactConnector : IConnector
    {
        internal string ApiKey { get; }
        internal string ApiUrl { get; }
        internal IConfiguration Configuration { get; }
        private OfferRepository OfferRepository { get; set; }
        private EstimateRepository EstimateRepository { get; set; }
        private EstimateLinesRepository EstimateLinesRepository { get; set; }

        public WeFactConnector(IConfiguration configuration, IRepository<Offers> offerrepository, IRepository<Estimates> estimaterepository,
            IRepository<EstimateLines> estimatelinesrepository)
        {
            Configuration = configuration;
            ApiKey = Configuration["WeFact:Key"];
            ApiUrl = Configuration["WeFact:Url"];
            OfferRepository = (OfferRepository)offerrepository;
            EstimateRepository = (EstimateRepository)estimaterepository;
            EstimateLinesRepository = (EstimateLinesRepository)estimatelinesrepository;
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
            if (response.StatusCode == (HttpStatusCode)200)
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

        public Invoice getInvoice(int identifier)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);

            request.AddParameter("api_key", ApiKey);
            request.AddParameter("controller", "invoice");
            request.AddParameter("action", "show");
            request.AddParameter("Identifier", identifier);


            var response = client.Execute<Invoice>(request);
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var line = new Dictionary<string, string>
            {
                {"Identifier", ""}
            };
            list.Add(line);
            if (response.StatusCode == (HttpStatusCode)200)
            {

                var responseObject = JsonConvert.DeserializeObject<Invoice>(response.Content);
                return responseObject;
            }
            return null;
        }

        public void AddOffer(string debtor, System.Guid offerid)
        {
            ICollection<Offers> offers;
            ICollection<EstimateConnects> Connect;
            using (var context = new DataBaseContext())
            {
                offers = context.Offer
                   .Include(offermodel => offermodel.Estimate)
                   .Include(user => user.CreatedBy)
                   .Include(user => user.UpdatedBy)
                    .ToList();

                Connect = context.EstimateConnects
                    .Include(line => line.EstimateLines)
                    .ToList();
            }
            var offer = offers.FirstOrDefault(x => x.Id.Equals(offerid));
            var guid = offer.Estimate.Id;
            var estimateid = Guid.Parse(guid.ToString());
            var estimate = EstimateRepository.Find(estimateid);
            var lines = Connect.Where(x => x.Estimate.Id == estimate.Id).ToList();

            var debtornumber = Regex.Replace(debtor, "[^0-9.]", "");

            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);
            var InvoiceLines = new List<Dictionary<string, string>>();

            foreach (var item in lines)
            {
                var line = new Dictionary<string, string>
                    {
                        { "Description" , item.EstimateLines.Specification },
                        { "PriceExcl" , item.EstimateLines.TotalCost.ToString() }
                    };
                InvoiceLines.Add(line);
            }
            var invoice = new Invoice
            {
                api_key = ApiKey,
                controller = "invoice",
                action = "add",
                Debtor = debtornumber.ToString(),

                Concept = 0,
                InvoiceLines = InvoiceLines
            };

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(invoice);

            var response = client.Execute<WeFactResponseModel>(request);
            var offerte = OfferRepository.Find(offerid);
            OfferRepository.SaveChanges();
            EditWefactLines(response.Data.Invoice.InvoiceLines, offerid);

        }

        public void EditInvoice(Guid offerid)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);

            var offer = OfferRepository.Find(offerid);

            request.AddParameter("api_key", ApiKey);
            request.AddParameter("action", "edit");
            request.AddParameter("controller", "invoice");
            request.AddParameter("Identifier", offer.DocumentCode);
        }

        public void DeleteInvoice(int invoicecode)
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("/apiv2/api.php", Method.POST);

            request.AddParameter("api_key", ApiKey);
            request.AddParameter("action", "delete");
            request.AddParameter("controller", "invoice");
            request.AddParameter("Identifier", invoicecode);

            var body = new WefactDeleteInvoiceModel
            {
                api_key = ApiKey,
                controller = "invoice",
                action = "delete",
                Identifier = 18
            };
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(body);

            var response = client.Execute(request);
        }

        public void EditWefactLines(List<Dictionary<string, string>> invoicelines, Guid id)
        {
            ICollection<Offers> offers;
            ICollection<EstimateConnects> Connect;
            using (var context = new DataBaseContext())
            {
                offers = context.Offer
                   .Include(offermodel => offermodel.Estimate)
                   .Include(user => user.CreatedBy)
                   .Include(user => user.UpdatedBy)
                    .ToList();

                Connect = context.EstimateConnects
                    .Include(line => line.EstimateLines)
                    .ToList();
            }

            var offer = offers.FirstOrDefault(x => x.Id.Equals(id));
            var estimate = EstimateRepository.Find(offer.Estimate.Id.ToString());
            var lineconnect = Connect.Where(x => x.Estimate.Id == estimate.Id).ToList();
            foreach (var line in invoicelines)
            {
                foreach (var item in line)
                {
                    if (item.Key == "Identifier")
                    {
                        foreach (var connects in lineconnect)
                        {
                            var lines = EstimateLinesRepository.Find(connects.EstimateLines.Id);
                            for (int i = 0; i < lineconnect.Count; i++)
                            {
                                lines.WefactIdentifier  = int.Parse(item.Value);
                                EstimateLinesRepository.SaveChanges();

                            }
                        }

                    }
                }
            }
        }

    }


}