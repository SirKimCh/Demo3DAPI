﻿using Microsoft.EntityFrameworkCore;
using Demo3DAPI.Data;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Demo3DAPI.Models;

namespace Demo3DAPI.Services
{
    public class PlayerCharacterService : IPlayerCharacterService
    {
        private readonly ApplicationDbContext _context;

        public PlayerCharacterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerCharacterResponseDto?> CreateCharacter(CreatePlayerCharacterDto characterDto)
        {
            var account = await _context.PlayerAccounts.FindAsync(characterDto.PlayerAccountID);
            if (account == null)
            {
                return null;
            }

            if (characterDto.Level < 1)
            {
                throw new InvalidOperationException("Level must be at least 1.");
            }

            var character = new PlayerCharacter
            {
                Name = characterDto.Name,
                Level = characterDto.Level,
                PlayerAccountID = characterDto.PlayerAccountID
            };

            _context.PlayerCharacters.Add(character);
            await _context.SaveChangesAsync();

            return new PlayerCharacterResponseDto
            {
                ID = character.ID,
                Name = character.Name,
                Level = character.Level,
                PlayerAccountID = character.PlayerAccountID,
                PlayerAccountUserName = account.UserName,
                PlayerAccountFullName = account.FullName
            };
        }

        public async Task<bool> DeleteCharacter(int id)
        {
            var character = await _context.PlayerCharacters.FindAsync(id);
            if (character == null) return false;

            _context.PlayerCharacters.Remove(character);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PlayerCharacterResponseDto>> GetAllCharacters()
        {
            return await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .Select(c => new PlayerCharacterResponseDto
                {
                    ID = c.ID,
                    Name = c.Name,
                    Level = c.Level,
                    PlayerAccountID = c.PlayerAccountID,
                    PlayerAccountUserName = c.PlayerAccount != null ? c.PlayerAccount.UserName : null,
                    PlayerAccountFullName = c.PlayerAccount != null ? c.PlayerAccount.FullName : null
                })
                .ToListAsync();
        }

        public async Task<PlayerCharacterResponseDto?> GetCharacterById(int id)
        {
            var character = await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (character == null) return null;

            return new PlayerCharacterResponseDto
            {
                ID = character.ID,
                Name = character.Name,
                Level = character.Level,
                PlayerAccountID = character.PlayerAccountID,
                PlayerAccountUserName = character.PlayerAccount?.UserName,
                PlayerAccountFullName = character.PlayerAccount?.FullName
            };
        }

        public async Task<IEnumerable<PlayerCharacterResponseDto>> GetCharactersByAccountId(int accountId)
        {
            return await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .Where(c => c.PlayerAccountID == accountId)
                .Select(c => new PlayerCharacterResponseDto
                {
                    ID = c.ID,
                    Name = c.Name,
                    Level = c.Level,
                    PlayerAccountID = c.PlayerAccountID,
                    PlayerAccountUserName = c.PlayerAccount != null ? c.PlayerAccount.UserName : null,
                    PlayerAccountFullName = c.PlayerAccount != null ? c.PlayerAccount.FullName : null
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateCharacter(int id, UpdatePlayerCharacterDto characterDto)
        {
            var character = await _context.PlayerCharacters.FindAsync(id);
            if (character == null) return false;

            if (characterDto.Level < 1)
            {
                throw new InvalidOperationException("Level must be at least 1.");
            }

            character.Name = characterDto.Name;
            character.Level = characterDto.Level;

            _context.Entry(character).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

