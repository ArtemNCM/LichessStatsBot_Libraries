namespace Coursova.Core.Models.DTOs
{
    public class PlayerInfoDto
    {
        public string Username { get; set; }
        public int OnlineRatingRapid { get; set; }   
        public int OnlineRatingBlitz { get; set; }
        public int OnlineRatingBullet { get; set; }
        public DateTime CreatedAt { get; set; }   
        public int GamesCount { get; set; }   
        public bool Online { get; set; }
        public DateTime LastSeen { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Flag { get; set; }
        public string Bio { get; set; }
        public string Links { get; set; }
    }

}
