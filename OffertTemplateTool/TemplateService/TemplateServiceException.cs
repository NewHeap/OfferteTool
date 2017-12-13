using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.TemplateService
{
    public class TemplateServiceException : Exception
    {
        public TemplateServiceException(string message) : base(message)
        {

        }
        public TemplateServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
