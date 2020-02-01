using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Models
{
    public class KeyGen
    {
        public static string Generate()
        {
            return $"{DateTime.UtcNow.ToString("ddMMyyyyhhmmssms")}{Guid.NewGuid().ToString()}";
        }
    }
}
