namespace Coursova.Core.Models.Entities
{
    public class RatingHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public string TimeControl { get; set; }   
        public DateTime Timestamp { get; set; }
        public int Rating { get; set; }
    }
}
