using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Coursova.Core.Models.Entities;
using Coursova.Core.Models.DTOs;
using Coursova.Core;

namespace Coursova.Infrastructure
{
    public class PlayerInfoRepository: IPlayerInfoRepository
    {
        private readonly ApplicationDbContext _db;

        public PlayerInfoRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<PlayerInfo> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var lowered = username.ToLower();
            return await _db.PlayerInfos
                .FirstOrDefaultAsync(p => p.Username.ToLower() == lowered);
        }

        public async Task<PlayerInfo> UpsertAsync(PlayerInfoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("PlayerInfoDto або Username не можуть бути null/порожніми.");

            var lowered = dto.Username.ToLower();
            var existing = await _db.PlayerInfos
                .FirstOrDefaultAsync(p => p.Username.ToLower() == lowered);

            if (existing == null)
            {
                var entity = new PlayerInfo
                {
                    Username = dto.Username,
                    OnlineRatingRapid = dto.OnlineRatingRapid,
                    OnlineRatingBlitz = dto.OnlineRatingBlitz,
                    OnlineRatingBullet = dto.OnlineRatingBullet,
                    CreatedAt = DateTime.UtcNow,
                    GamesCount = dto.GamesCount,
                    LastSeen = dto.LastSeen,
                    Title = dto.Title,
                    Country = dto.Country,
                    Flag = dto.Flag,
                };

                _db.PlayerInfos.Add(entity);
                await _db.SaveChangesAsync();

                return entity;
            }
            else
            {
                existing.OnlineRatingRapid = dto.OnlineRatingRapid;
                existing.OnlineRatingBlitz = dto.OnlineRatingBlitz;
                existing.OnlineRatingBullet = dto.OnlineRatingBullet;
                existing.GamesCount = dto.GamesCount;
                existing.LastSeen = dto.LastSeen;
                existing.Title = dto.Title;
                existing.Country = dto.Country;
                existing.Flag = dto.Flag;

                _db.PlayerInfos.Update(existing);
                await _db.SaveChangesAsync();

                return existing;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.PlayerInfos.FindAsync(id);
            if (entity != null)
            {
                _db.PlayerInfos.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
