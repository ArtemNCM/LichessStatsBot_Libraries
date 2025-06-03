using Coursova.Core.Models.DTOs;
using Coursova.Core.Models.Requests;
using Coursova.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coursova.Infrastructure
{
    public class LichessService : ILichessService
    {
        private readonly HttpClient _http;

        public LichessService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<PlayerInfoDto?> GetPlayerInfoAsync(string username)
        {
            
            using var resp = await _http.GetAsync($"/api/user/{username}");
            if (!resp.IsSuccessStatusCode) return null;

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            var root = doc.RootElement;

            var createdAtMs = root.GetProperty("createdAt").GetInt64();
            var createdAt = DateTimeOffset
                                  .FromUnixTimeMilliseconds(createdAtMs)
                                  .UtcDateTime;
            var gamesCount = root.GetProperty("count")
                                  .GetProperty("all")
                                  .GetInt32();

            
            bool online = root.TryGetProperty("online", out var onProp) && onProp.GetBoolean();

            
            DateTime lastSeen = root.TryGetProperty("seenAt", out var seenProp)
                ? DateTimeOffset.FromUnixTimeMilliseconds(seenProp.GetInt64()).UtcDateTime
                : DateTime.MinValue;

            
            string title = root.TryGetProperty("title", out var titleProp)
                ? titleProp.GetString()!
                : string.Empty;

            
            JsonElement profileEl = root.TryGetProperty("profile", out var pEl)
                ? pEl
                : default;

            string country = profileEl.ValueKind != JsonValueKind.Object
                ? string.Empty
                : profileEl.TryGetProperty("country", out var cProp)
                    ? cProp.GetString()!
                    : string.Empty;

            string flag = profileEl.ValueKind != JsonValueKind.Object
                ? string.Empty
                : profileEl.TryGetProperty("flag", out var fProp)
                    ? fProp.GetString()!
                    : string.Empty;

            string bio = profileEl.ValueKind != JsonValueKind.Object
                ? string.Empty
                : profileEl.TryGetProperty("bio", out var bProp)
                    ? bProp.GetString()!
                    : string.Empty;

            string links = profileEl.ValueKind != JsonValueKind.Object
                ? string.Empty
                : profileEl.TryGetProperty("links", out var lProp)
                    ? lProp.GetString()!
                    : string.Empty;

            
            return new PlayerInfoDto
            {
                Username = root.GetProperty("username").GetString()!,
                OnlineRatingRapid = root
                    .GetProperty("perfs")
                    .GetProperty("rapid")
                    .GetProperty("rating")
                    .GetInt32(),


                OnlineRatingBlitz = root
                    .GetProperty("perfs")
                    .GetProperty("blitz")
                    .GetProperty("rating")
                    .GetInt32(),

                OnlineRatingBullet = root
                    .GetProperty("perfs")
                    .GetProperty("bullet")
                    .GetProperty("rating")
                    .GetInt32(),

                CreatedAt = createdAt,
                GamesCount = gamesCount,

                Online = online,
                LastSeen = lastSeen,

                Title = title,
                Country = country,
                Flag = flag,

                Bio = bio,
                Links = links

            };
        }        

        public async Task<IEnumerable<RatingDto>> GetPlayerRatingsAsync(string username)
        {
            using var resp = await _http.GetAsync($"/api/user/{username}");
            if (!resp.IsSuccessStatusCode) return Array.Empty<RatingDto>();

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            var perfs = doc.RootElement.GetProperty("perfs");
            var result = new List<RatingDto>();

            foreach (var type in new[] { "bullet", "blitz", "rapid", "classical" })
            {
                var p = perfs.GetProperty(type);
                result.Add(new RatingDto
                {
                    TimeControl = type,
                    Rating = p.GetProperty("rating").GetInt32(),
                    GamesPlayed = p.GetProperty("games").GetInt32()
                });
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetPlayerGamesPgnAsync(string username, int gamesCount = 5)
        {
            
            var url = $"/api/games/user/{username}"
                    + $"?max={gamesCount}"
                    + "&moves=true"         
                    + "&tags=true"          
                    + "&pgnInJson=true";    

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.ParseAdd("application/x-ndjson");

            using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var pgns = new List<string>();
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var elem = JsonDocument.Parse(line).RootElement;

                if (elem.TryGetProperty("pgn", out var pgnProp))
                    pgns.Add(pgnProp.GetString()!);
            }

            return pgns;
        }
        public async Task<IEnumerable<OpeningDto>> GetPlayerOpeningsAsync(
        string username,
        string? color = null,    
        int fetchCount = 50,      
        int topCount = 3)       
        {
            var url = $"/api/games/user/{username}"
                    + $"?max={fetchCount}"
                    + "&opening=true"
                    + "&moves=false";
            if (!string.IsNullOrEmpty(color))
                url += $"&color={color.ToLowerInvariant()}";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.ParseAdd("application/x-ndjson");
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            var dict = new Dictionary<string, OpeningDto>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(await resp.Content.ReadAsStreamAsync());
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var e = JsonDocument.Parse(line).RootElement;
                if (!e.TryGetProperty("opening", out var op)) continue;

                var eco = op.GetProperty("eco").GetString()!;
                var name = op.GetProperty("name").GetString()!;

                var winner = e.TryGetProperty("winner", out var w) ? w.GetString() : null;
                bool isWhite = color == "white";

                if (color is null)
                {
                    var players = e.GetProperty("players");
                    var whiteId = players.GetProperty("white").GetProperty("user").GetProperty("id").GetString()!;
                    isWhite = string.Equals(whiteId, username, StringComparison.OrdinalIgnoreCase);
                }

                string result = winner == null
                    ? "draw"
                    : winner == (isWhite ? "white" : "black") ? "win" : "loss";

                if (!dict.TryGetValue(eco, out var stat))
                {
                    stat = new OpeningDto
                    {
                        EcoCode = eco,
                        Name = name,
                        GamesCount = 0,
                        Wins = 0,
                        Draws = 0,
                        Losses = 0
                    };
                    dict[eco] = stat;
                }

                stat.GamesCount++;
                if (result == "win") stat.Wins++;
                else if (result == "draw") stat.Draws++;
                else stat.Losses++;
            }

            var list = dict.Values
                .Select(o => {
                    o.WinRate = o.GamesCount > 0
                        ? Math.Round((o.Wins + 0.5 * o.Draws) / o.GamesCount, 2)
                        : 0;
                    return o;
                })
                .OrderByDescending(o => o.GamesCount)
                .Take(topCount)
                .ToList();

            return list;
        }
        public async Task<FavoriteControlDto> GetPlayerFavoriteControlAsync(string username)
        {

            var ratings = await GetPlayerRatingsAsync(username);
            if (ratings == null || !ratings.Any())
                return new FavoriteControlDto();

            var fav = ratings
                .OrderByDescending(r => r.GamesPlayed)
                .First();

            return new FavoriteControlDto
            {
                TimeControl = fav.TimeControl,
                GamesCount = fav.GamesPlayed,
                Rating = fav.Rating,
            };
        }
        public async Task<CompareStatsDto> ComparePlayersAsync(ComparePlayersRequest req)
        {

            var p1Info = await GetPlayerInfoAsync(req.Username1);
            var p2Info = await GetPlayerInfoAsync(req.Username2);

            if (p1Info is null || p2Info is null)
                return null!; 

            async Task<PerformanceDto> getPerf(string u)
            {
                using var resp = await _http.GetAsync($"/api/user/{u}");
                resp.EnsureSuccessStatusCode();
                using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
                var cnt = doc.RootElement.GetProperty("count");
                var wins = cnt.GetProperty("win").GetInt32();
                var draws = cnt.GetProperty("draw").GetInt32();
                var losses = cnt.GetProperty("loss").GetInt32();
                var total = wins + draws + losses;
                return new PerformanceDto
                {
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    WinRate = total > 0 ? Math.Round(wins / (double)total, 2) : 0
                };
            }

            var perf1 = await getPerf(req.Username1);
            var perf2 = await getPerf(req.Username2);

            return new CompareStatsDto
            {
                Player1 = p1Info,
                Player2 = p2Info,
                Perf1 = perf1,
                Perf2 = perf2
            };
        }
        public async Task<PerformanceDto> GetPlayerPerformanceAsync(
        string username,
        DateTime from,
        DateTime to)
        {

            var since = new DateTimeOffset(from).ToUnixTimeMilliseconds();
            var until = new DateTimeOffset(to).ToUnixTimeMilliseconds();

            var url = $"/api/games/user/{username}"
                    + $"?since={since}&until={until}"
                    + "&moves=false"    
                    + "&opening=false"  
                    + $"&max=300";      
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.ParseAdd("application/x-ndjson");

            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            int wins = 0, draws = 0, losses = 0;
            using var stream = await resp.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var e = JsonDocument.Parse(line).RootElement;

                var players = e.GetProperty("players");
                var whiteUser = players.GetProperty("white")
                                       .GetProperty("user")
                                       .GetProperty("name")
                                       .GetString()!;
                var isWhite = string.Equals(whiteUser, username, StringComparison.OrdinalIgnoreCase);

                var winner = e.TryGetProperty("winner", out var w) ? w.GetString() : null;

                if (winner == null)
                    draws++;
                else if (winner == (isWhite ? "white" : "black"))
                    wins++;
                else
                    losses++;
            }

            var total = wins + draws + losses;
            var effectiveWins = wins + 0.5 * draws;
            return new PerformanceDto
            {
                TotalGames = total,
                Wins = wins,
                Draws = draws,
                Losses = losses,
                WinRate = total > 0 ? Math.Round(effectiveWins / total, 2) : 0.0
            };
        }

        public async Task<IEnumerable<ChartPointDto>> GetRatingHistoryAsync(
        string username,
        string timeControl,
        DateTime from,
        DateTime to)
        {

            using var resp = await _http.GetAsync($"/api/user/{username}/rating-history");
            if (!resp.IsSuccessStatusCode)
                return Array.Empty<ChartPointDto>();

            var arr = await JsonSerializer
                .DeserializeAsync<JsonElement[]>(await resp.Content.ReadAsStreamAsync());

            if (arr == null || arr.Length == 0)
                return Array.Empty<ChartPointDto>();
            
            var series = arr.FirstOrDefault(s =>
                string.Equals(s.GetProperty("name").GetString(),
                  timeControl,
                  StringComparison.OrdinalIgnoreCase));

            if (series.ValueKind == JsonValueKind.Undefined)
                return Array.Empty<ChartPointDto>();

            var points = series.GetProperty("points").EnumerateArray();
            var list = new List<ChartPointDto>();
            foreach (var p in points)
            {
                int year = p[0].GetInt32();
                int monthZero = p[1].GetInt32();
                int day = p[2].GetInt32();
                int rating = p[3].GetInt32();
                var date = new DateTime(year, monthZero + 1, day);

                if (date.Date < from.Date || date.Date > to.Date)
                    continue;

                list.Add(new ChartPointDto
                {
                    Timestamp = date,
                    Rating = rating
                });
            }
            return list
                .OrderBy(pt => pt.Timestamp)
                .ToList();
        }

        public async Task<byte[]> GetRatingHistoryChartAsync(
        string username,
        string timeControl,
        DateTime from,
        DateTime to)
        {
            var points = await GetRatingHistoryAsync(username, timeControl, from, to);
            if (!points.Any()) return Array.Empty<byte>();

            var labels = points.Select(p => p.Timestamp.ToString("dd.MM")).ToArray();
            var data = points.Select(p => p.Rating).ToArray();

            var chartConfig = new
            {
                type = "line",
                data = new
                {
                    labels,
                    datasets = new[] {
                new {
                    label = $"{timeControl} rating",
                    data,
                    fill = false,
                    tension = 0.2     
                }
            }
                },
                options = new
                {
                    plugins = new { legend = new { display = false } },
                    scales = new
                    {
                        x = new { ticks = new { maxRotation = 90, minRotation = 90 } }
                    }
                }
            };

            var json = JsonSerializer.Serialize(chartConfig);
            var url = $"https://quickchart.io/chart?c={Uri.EscapeDataString(json)}";

            return await _http.GetByteArrayAsync(url);
        }

    }
}   


