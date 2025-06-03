namespace Coursova.Core.Models.Entities
{
    public class PlayerInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int OnlineRatingRapid { get; set; }  
        public int OnlineRatingBlitz { get; set; }   
        public int OnlineRatingBullet { get; set; }  
        public DateTime CreatedAt { get; set; }
        public int GamesCount { get; set; }
        public DateTime LastSeen { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Flag { get; set; }
    }
}
