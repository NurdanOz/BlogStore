using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogStore.PresentationLayer.ViewComponents.ArticleDetailViewComponents
{
    public class _ArticleDetailMainCoverImageComponentPartial : ViewComponent
    {
        private readonly IArticleService _articleService;

        public _ArticleDetailMainCoverImageComponentPartial(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var values=_articleService.TGetArticleWithUser(id);

            return View(values);

        }
    }

}