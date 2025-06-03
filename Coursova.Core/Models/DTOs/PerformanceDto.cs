namespace Coursova.Core.Models.DTOs
{
    public class PerformanceDto
    {
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public double WinRate { get; set; }
    }

}
