using System.Threading.Tasks;
using Coursova.Core.Models.Entities;
using Coursova.Core.Models.DTOs;
namespace Coursova.Core
{
    public interface IPlayerInfoRepository
    {
    Task<PlayerInfo> GetByUsernameAsync(string username);
    Task<PlayerInfo> UpsertAsync(PlayerInfoDto dto);
    Task DeleteAsync(int id);
    }
}
