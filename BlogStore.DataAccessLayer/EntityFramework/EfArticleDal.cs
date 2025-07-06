using BlogStore.DataAccessLayer.Abstract;
using BlogStore.DataAccessLayer.Context;
using BlogStore.DataAccessLayer.Repositories;
using BlogStore.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.DataAccessLayer.EntityFramework
{
    public class EfArticleDal : GenericRepository<Article>, IArticleDal

    {

        private readonly BlogContext _context;

        public EfArticleDal(BlogContext context) : base(context)
        {
            _context = context;
        }

        public AppUser GetAppUserByArticleId(int id)
        {
            string userId = _context.Articles.Where(x=>x.ArticleId==id).Select(y=>y.AppUserId).FirstOrDefault();
            var userValue=_context.Users.Where(x=>x.Id==userId).FirstOrDefault();
            return userValue;
        }

        public List<Article> GetArticleByAppUser(string id)
        {
            return _context.Articles.Where(x => x.AppUserId == id).ToList();
        }

        public Article GetArticleBySlug(string slug)
        {
            return _context.Articles.Include(x => x.AppUser).Include(x => x.category).FirstOrDefault(x => x.Slug == slug);
        }

        public List<Article> GetArticlesByAppUserWithCategories(string appUserId)
        {
            return _context.Articles 
                          .Where(a => a.AppUserId == appUserId)
                          .Include(a => a.category) 
                          .ToList(); 
        }

        public Article GetArticleWithUser(int id)
        {
            return _context.Articles.Include(x => x.AppUser).FirstOrDefault(x => x.ArticleId == id);
        }

        public List<Article> GetTop3PopulerArticles()
        {
            var values=_context.Articles.OrderByDescending(x=>x.ArticleId).Take(3).ToList();
            return values;
        }

        public List<Article> TGetArticlesWithCategories()
        {
            return _context.Articles.Include(x => x.category).ToList();
        }


    }
}
