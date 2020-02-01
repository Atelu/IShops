using IShops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Services
{
    public interface IUserService
    {
        /// <summary>
        /// adds a new employee
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        Task<string> Add(UserInfo employees);

        /// <summary>
        /// gets the login details
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<UserInfo> GetUserDetails(string UserId);
    }
}
