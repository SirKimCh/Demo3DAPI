﻿using Demo3DAPI.DTOs;

namespace Demo3DAPI.Interfaces
{
    public interface IPlayerAccountService
    {
        Task<IEnumerable<PlayerAccountBasicDto>> GetAllAccounts();
        Task<PlayerAccountResponseDto?> GetAccountById(int id);
        Task<PlayerAccountResponseDto> CreateAccount(CreatePlayerAccountDto accountDto);
        Task<bool> UpdateAccount(int id, UpdatePlayerAccountDto accountDto);
        Task<bool> DeleteAccount(int id);
    }
}

