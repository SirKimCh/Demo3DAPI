using Demo3DAPI.DTOs;
using Demo3DAPI.Models;

namespace Demo3DAPI.Interfaces
{
    public interface IPlayerAccountService
    {
        Task<IEnumerable<PlayerAccount>> GetAllAccounts();
        Task<PlayerAccount?> GetAccountById(int id);
        Task<PlayerAccount> CreateAccount(CreatePlayerAccountDto accountDto);
        Task<bool> UpdateAccount(int id, UpdatePlayerAccountDto accountDto);
        Task<bool> DeleteAccount(int id);
    }
}

