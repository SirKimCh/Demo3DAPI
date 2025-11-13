using Demo3DAPI.DTOs;
using Demo3DAPI.Models;

namespace Demo3DAPI.Interfaces
{
    public interface IPlayerCharacterService
    {
        Task<IEnumerable<PlayerCharacter>> GetAllCharacters();
        Task<PlayerCharacter?> GetCharacterById(int id);
        Task<IEnumerable<PlayerCharacter>> GetCharactersByAccountId(int accountId);
        Task<PlayerCharacter?> CreateCharacter(CreatePlayerCharacterDto characterDto);
        Task<bool> UpdateCharacter(int id, UpdatePlayerCharacterDto characterDto);
        Task<bool> DeleteCharacter(int id);
    }
}

