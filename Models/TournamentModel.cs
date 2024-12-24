namespace AlicanteWeb.Models
{
    public class TournamentModel
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public bool Active { get; set; } = false;
    }
}
