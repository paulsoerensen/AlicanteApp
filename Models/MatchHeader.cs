namespace AlicanteWeb.Models
{
    public class MatchHeader  {
        public int MatchId { get; set; }
        public DateTime? MatchDate { get; set; }
        public string CourseName { get; set; }
        public string MatchText => MatchId < 0 ? "Total" : $"{CourseName}, {MatchDate:dd MMM, yyyy}";
    }
}
