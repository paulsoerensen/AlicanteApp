namespace AlicanteWeb.Models
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal CourseRating { get; set; } = 72.0m;
        public int Slope { get; set; } = 125;
        public int Par { get; set; } = 72;
    }
}
