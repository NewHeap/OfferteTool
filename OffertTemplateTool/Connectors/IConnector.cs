using OffertTemplateTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Connectors
{
    public interface IConnector
    {
        WeFactDebtorsModel GetCustomerInfo(string debtorcode);
        ICollection<WeFactDebtorsModel> GetCustomers();


    }
}
