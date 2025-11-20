using Demo3DAPI.Models;

namespace Demo3DAPI.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(PlayerAccount account);
    }
}

