using BlogStore.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogStore.PresentationLayer.ViewComponents
{
    public class _ArticleDetailContentComponentPartial:ViewComponent
    {

        private readonly IArticleService _articleService;

        public _ArticleDetailContentComponentPartial(IArticleService articleservice)
        {
            _articleService= articleservice;
        }

        public IViewComponentResult Invoke(int id)
        {
            var value = _articleService.TGetById(id);
            return View(value);
        }
    }
}
