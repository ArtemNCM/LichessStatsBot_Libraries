using AutoMapper;
using Coursova.Core.Models.DTOs;    

namespace Coursova.Core.Mapping
{
    public class LichessMappingProfile : Profile
    {
        public LichessMappingProfile()
        {
            CreateMap<LichessUserDto, PlayerInfoDto>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Username))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => DateTimeOffset.FromUnixTimeMilliseconds(((DateTimeOffset)s.CreatedAt).ToUnixTimeMilliseconds()).UtcDateTime))
                .ForMember(d => d.GamesCount, o => o.MapFrom(s => s.Count))
                .ForMember(d => d.Online, o => o.MapFrom(s => s.Online))
                .ForMember(d => d.LastSeen, o => o.MapFrom(s => DateTimeOffset.FromUnixTimeMilliseconds(((DateTimeOffset)s.CreatedAt).ToUnixTimeMilliseconds()).UtcDateTime))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Country, o => o.MapFrom(s => s.Country))
                .ForMember(d => d.Flag, o => o.MapFrom(s => s.Flag))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.Bio))
                .ForMember(d => d.Links, o => o.MapFrom(s => s.Links))
                .ForMember(d => d.OnlineRatingBullet, o => o.MapFrom(s => s.Perfs.Bullet.Rating))
                .ForMember(d => d.OnlineRatingRapid, o => o.MapFrom(s => s.Perfs.Rapid.Rating))
                .ForMember(d => d.OnlineRatingBlitz, o => o.MapFrom(s => s.Perfs.Blitz.Rating));
        }
    }
}

