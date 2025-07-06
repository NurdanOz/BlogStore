using BlogStore.BusinessLayer.Abstract;
using BlogStore.DataAccessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Concrete
{
    public class ArticleManager : IArticleService
    {
        private readonly IArticleDal _articleDal;

        public ArticleManager(IArticleDal articleDal)
        {
            _articleDal = articleDal;
        }

        public AppUser TGetAppUserByArticleId(int id)
        {
           return _articleDal.GetAppUserByArticleId(id);
        }

        public void TDelete(int id)
        {
            _articleDal.Delete(id);
        }

        public List<Article> TGetAll()
        {
            return _articleDal.GetAll();
        }

        public List<Article> TGetArticlesWithCategories()
        {
            return _articleDal.TGetArticlesWithCategories();
        }

        
        public Article TGetById(int id)
        {
           return _articleDal.GetById(id);
        }

        public void TInsert(Article entity)
        {
            if (entity.Title.Length >= 10 && entity.Title.Length <= 100 && entity.Description != "" && !string.IsNullOrWhiteSpace(entity.ImageUrl)) 
            {
                _articleDal.Insert(entity);
            }
            else
            {
               throw new ArgumentException("Makale başlığı en az 10, en fazla 100 karakter olmalı; açıklama boş olamaz ve görsel URL'si boş olamaz.");
            }
        }

        public void TUpdate(Article entity)
        {
            _articleDal.Update(entity);
        }

        public List<Article> TGetTop3PopulerArticles()
        {
            return _articleDal.GetTop3PopulerArticles();
        }

        public List<Article> TGetArticleByAppUser(string id)
        {
            return _articleDal.GetArticleByAppUser(id);
        }

        public Article TGetArticleWithUser(int id)
        {
            return _articleDal.GetArticleWithUser(id);
        }

        public Article TGetArticleBySlug(string slug)
        {
            return _articleDal.GetArticleBySlug(slug);
        }

        public List<Article> TGetArticlesByAppUserWithCategories(string appUserId)
        {
            
            return _articleDal.GetArticlesByAppUserWithCategories(appUserId);
        }
    }
}
