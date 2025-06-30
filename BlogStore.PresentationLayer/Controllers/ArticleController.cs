using BlogStore.BusinessLayer.Abstract;
using BlogStore.BusinessLayer.Helpers;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using BlogStore.BusinessLayer.Helpers;

namespace BlogStore.PresentationLayer.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [Route("Article/ArticleDetail/{slug}")]
        public IActionResult ArticleDetail(string slug)
        {
            var article = _articleService.TGetArticleBySlug(slug);

            if (article == null)
            {
                return NotFound();
            }

            ViewBag.i = article.ArticleId;
            return View();
        }

        
    }
}
