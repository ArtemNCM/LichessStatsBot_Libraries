namespace Coursova.Core.Models.DTOs
{
    public class OpeningDto
    {
        public string EcoCode { get; set; }
        public string Name { get; set; }
        public int GamesCount { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public double WinRate { get; set; }
    }

}
