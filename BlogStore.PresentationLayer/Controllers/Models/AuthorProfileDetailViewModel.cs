using BlogStore.EntityLayer.Entities;

namespace BlogStore.PresentationLayer.Controllers.Models
{
    public class AuthorProfileDetailViewModel
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string description { get; set; }
        public string Email { get; set; }
        public string ımageurl { get; set; } // 'ımageurl' yerine düzenli 'ImageUrl' kullanmak daha iyi

        // Yazarın makalelerini göstermek için
        public List<Article> Articles { get; set; }
    }
}
