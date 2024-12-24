namespace AlicanteWeb.Models
{
    public class MatchViewModel
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public bool Active { get; set; }
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal CourseRating { get; set; }
        public int Slope { get; set; }
        public int Par { get; set; }
        public string MatchText => $"{CourseName}, {MatchDate:dd MMM, yyyy}";

    }
}
