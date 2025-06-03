namespace Coursova.Core.Models.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public string Opponent { get; set; }
        public string Result { get; set; }      
        public DateTime PlayedAt { get; set; }
        public string TimeControl { get; set; }      
        public int MovesCount { get; set; }
        public string PgnUrl { get; set; }
        public int? RatingBefore { get; set; }
        public int? RatingAfter { get; set; }
    }
}
