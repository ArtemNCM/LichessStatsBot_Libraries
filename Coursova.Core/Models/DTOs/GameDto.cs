namespace Coursova.Core.Models.DTOs
{
    public class GameDto
    {
        public string Opponent { get; set; }
        public string Result { get; set; }
        public DateTime PlayedAt { get; set; }
        public string TimeControl { get; set; }
        public int MovesCount { get; set; }
        public string PgnUrl { get; set; }
        public int? RatingChange { get; set; }
    }

}
