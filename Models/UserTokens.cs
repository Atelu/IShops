using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IShops.Models
{
    public class UserTokens
    {
        [Key]
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}

