﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<PlayerAccountResponseDto> CreateAccount(CreatePlayerAccountDto accountDto)
        {
            var existingAccount = await _context.PlayerAccounts
                .FirstOrDefaultAsync(a => a.UserName == accountDto.UserName);
            
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Username '{accountDto.UserName}' already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountDto.Password);

            var account = new PlayerAccount
            {
                UserName = accountDto.UserName,
                Password = hashedPassword,
                FullName = accountDto.FullName,
                PhoneNumber = accountDto.PhoneNumber
            };

            _context.PlayerAccounts.Add(account);
            await _context.SaveChangesAsync();

            var createdAccount = await _context.PlayerAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.ID == account.ID);

            return new PlayerAccountResponseDto
            {
                ID = account.ID,
                UserName = account.UserName,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleID = account.RoleID,
                RoleName = createdAccount?.Role?.Name,
                Characters = new List<CharacterBasicDto>()
            };
        }

        public async Task<bool> DeleteAccount(int id)
        {
            var account = await _context.PlayerAccounts.FindAsync(id);
            if (account == null) return false;

            _context.PlayerAccounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlayerAccountResponseDto?> GetAccountById(int id)
        {
            var account = await _context.PlayerAccounts
                .Include(a => a.Role)
                .Include(a => a.Characters)
                .FirstOrDefaultAsync(a => a.ID == id);

            if (account == null) return null;

            return new PlayerAccountResponseDto
            {
                ID = account.ID,
                UserName = account.UserName,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                RoleID = account.RoleID,
                RoleName = account.Role?.Name,
                Characters = account.Characters.Select(c => new CharacterBasicDto
                {
                    ID = c.ID,
                    Name = c.Name,
                    Level = c.Level
                }).ToList()
            };
        }

        public async Task<IEnumerable<PlayerAccountBasicDto>> GetAllAccounts()
        {
            return await _context.PlayerAccounts
                .Include(a => a.Role)
                .Select(a => new PlayerAccountBasicDto
                {
                    ID = a.ID,
                    UserName = a.UserName,
                    FullName = a.FullName,
                    RoleID = a.RoleID,
                    RoleName = a.Role != null ? a.Role.Name : null
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAccount(int id, UpdatePlayerAccountDto accountDto)
        {
            var account = await _context.PlayerAccounts.FindAsync(id);
            if (account == null) return false;

            if (accountDto.FullName != null)
                account.FullName = accountDto.FullName;
            
            if (accountDto.PhoneNumber != null)
                account.PhoneNumber = accountDto.PhoneNumber;

            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

