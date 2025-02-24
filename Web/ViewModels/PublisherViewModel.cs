namespace Web.ViewModels
{
    public class PublisherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public int TotalBooks { get; set; }
    }
}
