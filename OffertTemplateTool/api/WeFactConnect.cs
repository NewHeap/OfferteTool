using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.api
{
    public class WeFactConnect
    {
        private string url;
        private string api_key;

        public WeFactConnect()
        {
            url = "localhost/wefact/Pro/apiv2/api.php";
		    api_key = "280dbf8499b1622ed007e06fd2068dc9";
        }
        public void SendRequest(string contoller, string action,)
        {
            
        }

    }
}
