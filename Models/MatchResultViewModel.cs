namespace AlicanteWeb.Models
{
    public class MatchResultViewModel
    {
        public int MatchId { get; set; }
        public int MatchRank { get; set; }
        public DateTime MatchDate { get; set; }
        public string CourseName { get; set; }
        public int? Hcp { get; set; }
        public int Par { get; set; }
        public int Score { get; set; }
        public int Netto { get; set; }

        public int Birdies { get; set; }
        public int Par3 { get; set; }
        public string PlayerName { get; set; }
        public decimal HcpIndex { get; set; }
    }
}
