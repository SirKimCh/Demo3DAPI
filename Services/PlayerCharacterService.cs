using Microsoft.EntityFrameworkCore;
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

        public async Task<PlayerCharacter?> CreateCharacter(CreatePlayerCharacterDto characterDto)
        {
            var accountExists = await _context.PlayerAccounts.AnyAsync(a => a.ID == characterDto.PlayerAccountID);
            if (!accountExists)
            {
                return null;
            }

            var character = new PlayerCharacter
            {
                Name = characterDto.Name,
                Level = characterDto.Level,
                PlayerAccountID = characterDto.PlayerAccountID
            };

            _context.PlayerCharacters.Add(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<bool> DeleteCharacter(int id)
        {
            var character = await _context.PlayerCharacters.FindAsync(id);
            if (character == null) return false;

            _context.PlayerCharacters.Remove(character);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PlayerCharacter>> GetAllCharacters()
        {
            return await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .ToListAsync();
        }

        public async Task<PlayerCharacter?> GetCharacterById(int id)
        {
            return await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<IEnumerable<PlayerCharacter>> GetCharactersByAccountId(int accountId)
        {
            return await _context.PlayerCharacters
                .Include(c => c.PlayerAccount)
                .Where(c => c.PlayerAccountID == accountId)
                .ToListAsync();
        }

        public async Task<bool> UpdateCharacter(int id, UpdatePlayerCharacterDto characterDto)
        {
            var character = await _context.PlayerCharacters.FindAsync(id);
            if (character == null) return false;

            character.Name = characterDto.Name;
            character.Level = characterDto.Level;

            _context.Entry(character).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

