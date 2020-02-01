using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IShops.Data;
using IShops.Models;
using Microsoft.EntityFrameworkCore;

namespace IShops.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Add(UserInfo employees)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                employees.Id = KeyGen.Generate();
                employees.CreateTime = DateTime.UtcNow;
                await _context.AddAsync(employees);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return "Ok";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return $"Error: {ex.Message}";
            }
        }

        public async Task<UserInfo> GetUserDetails(string UserId)
        {
            return await _context.UserInfo.SingleOrDefaultAsync(u => u.AccountId == UserId);
        }
    }
}
