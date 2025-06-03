namespace Coursova.Core.Models.DTOs
{
    public class CompareStatsDto
    {
        public PlayerInfoDto Player1 { get; set; }
        public PlayerInfoDto Player2 { get; set; }
        public PerformanceDto Perf1 { get; set; }
        public PerformanceDto Perf2 { get; set; }
    }

}
