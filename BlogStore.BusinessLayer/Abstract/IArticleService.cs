using BlogStore.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Abstract
{
    public interface IArticleService:IGenericService<Article>
    {
        public List<Article> TGetArticlesWithCategories();

        public AppUser TGetAppUserByArticleId(int id);

        public List<Article> TGetTop3PopulerArticles();

        public List<Article> TGetArticleByAppUser(string id);

        public Article TGetArticleWithUser(int id);

        public Article TGetArticleBySlug(string slug);

        List<Article> TGetArticlesByAppUserWithCategories(string appUserId);
    }
}
