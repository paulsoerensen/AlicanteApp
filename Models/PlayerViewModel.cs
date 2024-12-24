namespace AlicanteWeb.Models
{
    public class PlayerViewModel
    {
        public int PlayerId { get; set; }
        public int? ResultId { get; set; }
        public string PlayerName { get; set; }
        public decimal? HcpIndex { get; set; }
        public int Hcp { get; set; }
    }
}
