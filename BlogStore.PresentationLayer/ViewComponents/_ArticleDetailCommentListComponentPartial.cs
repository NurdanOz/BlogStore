using BlogStore.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogStore.PresentationLayer.ViewComponents
{
    public class _ArticleDetailCommentListComponentPartial:ViewComponent
    {
        private readonly ICommentService _commentService;

        public _ArticleDetailCommentListComponentPartial(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var values = _commentService.TGetCommentsByArticle(id);
            ViewBag.ArticleId = id;
            return View(values);
        }
    }
}
