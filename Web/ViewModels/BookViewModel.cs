using Core.Enums;

namespace Web.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }
}
