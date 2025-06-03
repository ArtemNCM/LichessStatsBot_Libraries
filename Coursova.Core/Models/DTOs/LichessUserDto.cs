using Coursova.Core.Mapping;

namespace Coursova.Core.Models.DTOs
{
    public class LichessUserDto
    {
        public string Username { get; set; }
        public PerfsDto Perfs { get; set; }
        public DateTime CreatedAt { get; set; }   
        public int Count { get; set; }   
        public bool Online { get; set; }
        public DateTime LastSeen { get; set; }
        public string Title { get; set; }

        public string Country { get; set; }
        public string Flag { get; set; }
        public string Bio { get; set; }
        public string Links { get; set; }
    }
    public class PerfsDto
    {
        public PerfStatDto Bullet { get; set; }
        public PerfStatDto Blitz { get; set; }
        public PerfStatDto Rapid { get; set; }
        
    }

    public class PerfStatDto
    {
        public int Rating { get; set; }
     
    }
}
