namespace AlicanteWeb.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public bool Active { get; set; }
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal CourseRating { get; set; }
        public int Slope { get; set; }
        public int Hcp { get; set; }
        public int Par { get; set; }
        public int Score { get; set; }
        public int Birdies { get; set; }
        public int Par3 { get; set; }
        public int PlayerId { get; set; } = 0;
        public string PlayerName { get; set; }
        public decimal HcpIndex { get; set; }
    }
}
