using Coursova.Core.Models.DTOs;
using Coursova.Core.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coursova.Core
{
    public interface ILichessService
    {
        Task<PlayerInfoDto?> GetPlayerInfoAsync(string username);

        Task<IEnumerable<RatingDto>> GetPlayerRatingsAsync(string username);

        Task<IEnumerable<string>> GetPlayerGamesPgnAsync(string username, int gamesCount = 5);

        Task<FavoriteControlDto> GetPlayerFavoriteControlAsync(string username);

        Task<CompareStatsDto> ComparePlayersAsync(ComparePlayersRequest req);

        Task<PerformanceDto> GetPlayerPerformanceAsync(string username, DateTime from, DateTime to);

        Task<IEnumerable<ChartPointDto>> GetRatingHistoryAsync(string username,string timeControl, DateTime from, DateTime to);

        Task<byte[]> GetRatingHistoryChartAsync( string username,string timeControl, DateTime from, DateTime to);

        Task<IEnumerable<OpeningDto>> GetPlayerOpeningsAsync(string username, string color = "white",int fetch = 50, int top = 3);
    }
}

