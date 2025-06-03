namespace Coursova.Core.Models.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }      
        public DateTime AddedAt { get; set; }      
        public ICollection<Game> Games { get; set; }
        public ICollection<OpeningStatistic> Openings { get; set; }
        public ICollection<RatingHistory> RatingHistory { get; set; }
    }
}
