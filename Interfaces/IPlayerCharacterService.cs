﻿using Demo3DAPI.DTOs;

namespace Demo3DAPI.Interfaces
{
    public interface IPlayerCharacterService
    {
        Task<IEnumerable<PlayerCharacterResponseDto>> GetAllCharacters();
        Task<PlayerCharacterResponseDto?> GetCharacterById(int id);
        Task<IEnumerable<PlayerCharacterResponseDto>> GetCharactersByAccountId(int accountId);
        Task<PlayerCharacterResponseDto?> CreateCharacter(CreatePlayerCharacterDto characterDto);
        Task<bool> UpdateCharacter(int id, UpdatePlayerCharacterDto characterDto);
        Task<bool> DeleteCharacter(int id);
    }
}

