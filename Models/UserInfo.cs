using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IShops.Models
{
    public class UserInfo
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool CanLogin { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserId { get; set; }
        public string AccountId { get; set; }
        public string Country { get; set; }
        public virtual ICollection<ProductsModel> Products { get; set; }



    }
}
