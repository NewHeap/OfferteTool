using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.Models
{
    public class UsersViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Initials { get; set; }
        public string Insertion { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Function { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
    }
}
