namespace Coursova.Core.Models.Entities
{
    public class OpeningStatistic
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public string EcoCode { get; set; }     
        public string Name { get; set; }     
        public string Color { get; set; }     
        public int GamesCount { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
    }
}
