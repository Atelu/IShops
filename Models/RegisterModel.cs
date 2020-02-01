using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Models
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string AccountId { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public string UserId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public DateTime DOB { get; set; }
        public DateTime Created { get; set; }
        public bool Canlogin { get; set; }
        public bool Isactive { get; set; }
      
    }
}
