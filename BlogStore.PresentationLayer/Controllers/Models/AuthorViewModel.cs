namespace BlogStore.PresentationLayer.Controllers.Models
{
    public class AuthorViewModel
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string description { get; set; }
        public string ımageUrl { get; set; }

        public List<string> WrittenCategories { get; set; } = new List<string>(); 
        public int ArticleCount { get; set; } 
    }
}
