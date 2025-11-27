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

        public async Task<LoginResponseDto?> Login(LoginDto loginDto, IJwtService jwtService)
        {
            var account = await _context.PlayerAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.UserName == loginDto.UserName);

            if (account == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password);
            
            if (!isPasswordValid)
            {
                return null;
            }

            var token = jwtService.GenerateToken(account);

            return new LoginResponseDto
            {
                ID = account.ID,
                UserName = account.UserName,
                FullName = account.FullName,
                RoleID = account.RoleID,
                RoleName = account.Role?.Name,
                Token = token,
                Message = "Login successful"
            };
        }

        public async Task<PlayerAccountResponseDto> Register(RegisterDto registerDto)
        {
            var existingAccount = await _context.PlayerAccounts
                .FirstOrDefaultAsync(a => a.UserName == registerDto.UserName);
            
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Username '{registerDto.UserName}' already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var account = new PlayerAccount
            {
                UserName = registerDto.UserName,
                Password = hashedPassword,
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                RoleID = 2 // Default User role
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
                PhoneNumber = accountDto.PhoneNumber,
                RoleID = 2
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
            var account = await _context.PlayerAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.ID == id);
            
            if (account == null) return false;

            // Prevent Admin deletion
            if (account.RoleID == 1 || account.Role?.Name == "Admin")
            {
                throw new InvalidOperationException("Cannot delete Admin account.");
            }

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

