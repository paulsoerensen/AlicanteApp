namespace AlicanteWeb.Models
{
    public class MatchModel
    {
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int CourseId { get; set; }
        public int TournamentId { get; set; }
    }
}
