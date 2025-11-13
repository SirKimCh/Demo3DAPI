using Microsoft.EntityFrameworkCore;
using Demo3DAPI.Data;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Demo3DAPI.Models;

namespace Demo3DAPI.Services
{
    public class PlayerAccountService : IPlayerAccountService
    {
        private readonly ApplicationDbContext _context;

        public PlayerAccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerAccount> CreateAccount(CreatePlayerAccountDto accountDto)
        {
            var existingAccount = await _context.PlayerAccounts
                .FirstOrDefaultAsync(a => a.UserName == accountDto.UserName);
            
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Username '{accountDto.UserName}' already exists.");
            }

            var account = new PlayerAccount
            {
                UserName = accountDto.UserName,
                Password = accountDto.Password,
                FullName = accountDto.FullName,
                PhoneNumber = accountDto.PhoneNumber
            };

            _context.PlayerAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> DeleteAccount(int id)
        {
            var account = await _context.PlayerAccounts.FindAsync(id);
            if (account == null) return false;

            _context.PlayerAccounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlayerAccount?> GetAccountById(int id)
        {
            return await _context.PlayerAccounts
                .Include(a => a.Characters)
                .FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<IEnumerable<PlayerAccount>> GetAllAccounts()
        {
            return await _context.PlayerAccounts
                .Include(a => a.Characters)
                .ToListAsync();
        }

        public async Task<bool> UpdateAccount(int id, UpdatePlayerAccountDto accountDto)
        {
            var account = await _context.PlayerAccounts.FindAsync(id);
            if (account == null) return false;

            account.FullName = accountDto.FullName;
            account.PhoneNumber = accountDto.PhoneNumber;

            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

