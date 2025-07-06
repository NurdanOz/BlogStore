using BlogStore.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.DataAccessLayer.Abstract
{
    public interface IArticleDal : IGenericDal<Article>
    {

        List<Article> TGetArticlesWithCategories();

        public AppUser GetAppUserByArticleId(int id);

        List<Article> GetTop3PopulerArticles();

        List<Article> GetArticleByAppUser(string id);

        public Article GetArticleWithUser(int id);

        public Article GetArticleBySlug(string slug);

        List<Article> GetArticlesByAppUserWithCategories(string appUserId);
    }
}
