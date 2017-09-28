using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Models
{
    public class Users : IDb
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Insertion { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Function { get; set; }
    }
}
