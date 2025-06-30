using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace BlogStore.PresentationLayer.Controllers
{
    public class CommentController : Controller
    {

        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentService commentService, UserManager<AppUser> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        public IActionResult CommentList()
        {
            var values = _commentService.TGetAll();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateComment()
        {
            return View();
        }



        [HttpPost]
        public IActionResult CreateComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.IsValid = false;
            _commentService.TInsert(comment);
            return RedirectToAction("CommentList");
        }

        public IActionResult DeleteComment(int id)
        {
            _commentService.TDelete(id);
            return RedirectToAction("CommentList");
        }

        [HttpGet]

        public IActionResult UpdateComment(int id)

        {
            var value = _commentService.TGetById(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.IsValid = false;
            _commentService.TUpdate(comment);
            return RedirectToAction("CommentList");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCommentAjax(int articleId, string commentDetail)
        {
            try
            {
                if (string.IsNullOrEmpty(commentDetail) || commentDetail.Length < 3)
                {
                    return Json(new { success = false, message = "Yorum en az 3 karakter olmalıdır." });
                }


                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }


                var comment = new Comment
                {
                    ArticleId = articleId,
                    CommentDetail = commentDetail,
                    CommentDate = DateTime.Now,
                    AppUserId = user.Id,
                    UserNameSurname = user.name + " " + user.surname,
                    IsValid = true
                };

                _commentService.TInsert(comment);


                return Json(new
                {
                    success = true,
                    message = "Yorumunuz başarıyla eklendi!",
                    comment = new
                    {
                        userNameSurname = comment.UserNameSurname,
                        commentDetail = comment.CommentDetail,
                        commentDate = comment.CommentDate.ToString("dd-MMM-yyyy"),
                        userImageUrl = user.ımageurl
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        // ⭐ MAKALE YORUMLARİNİ AJAX İLE GETIRME
        [HttpGet]
        public IActionResult GetCommentsByArticle(int articleId)
        {
            var comments = _commentService.TGetCommentsByArticle(articleId);
            var commentList = comments.Select(c => new
            {
                userNameSurname = c.AppUser.name + " " + c.AppUser.surname,
                commentDetail = c.CommentDetail,
                commentDate = c.CommentDate.ToString("dd-MMM-yyyy"),
                userImageUrl = c.AppUser.ımageurl
            });

            return Json(new { success = true, comments = commentList });


        }

    }
}



