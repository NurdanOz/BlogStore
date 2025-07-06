using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics; // Debug.WriteLine için ekli

namespace BlogStore.PresentationLayer.Controllers
{
    [Route("[controller]/[action]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToxicityDetectionService _toxicityDetectionService;
        private readonly ITranslationService _translationService;

        public CommentController(ICommentService commentService, UserManager<AppUser> userManager, IToxicityDetectionService toxicityDetectionService, ITranslationService translationService)
        {
            _commentService = commentService;
            _userManager = userManager;
            _toxicityDetectionService = toxicityDetectionService;
            _translationService = translationService;
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
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            try
            {
                if (comment == null || string.IsNullOrWhiteSpace(comment.CommentDetail))
                    return Json(new { status = "error", message = "Yorum verisi eksik veya geçersiz." });

                var translatedCommentDetail = await _translationService.TranslateToEnglishAsync(comment.CommentDetail);

                var detectionResult = await _toxicityDetectionService.DetectToxicityAsync(translatedCommentDetail);
                comment.IsToxic = detectionResult.IsToxic;
                comment.Score = (float)detectionResult.Score;
                comment.AppUserId = _userManager.GetUserId(User);
                comment.UserNameSurname = _userManager.GetUserName(User);
                comment.CommentDate = DateTime.Now;

                _commentService.TInsert(comment);

                if (detectionResult.IsToxic)
                    return Json(new { status = "toxic", message = "Yorumunuz toksik içerik barındırıyor." });
                return Json(new { status = "success", message = "Yorumunuz başarıyla eklendi." });

            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = $"Bir hata oluştu: {ex.Message}" });
            }
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

                var translatedCommentDetail = await _translationService.TranslateToEnglishAsync(commentDetail);

                if (string.IsNullOrEmpty(translatedCommentDetail))
                {
                    return Json(new { success = false, message = "Yorum çevrilemedi, lütfen tekrar deneyin." });
                }

                var detectionResult = await _toxicityDetectionService.DetectToxicityAsync(translatedCommentDetail);

                var comment = new Comment
                {
                    ArticleId = articleId,
                    CommentDetail = commentDetail,
                    CommentDate = DateTime.Now,
                    AppUserId = user.Id,
                    UserNameSurname = user.name + " " + user.surname,
                    IsValid = true, // Varsayılan olarak geçerli kabul edelim
                    IsToxic = detectionResult.IsToxic,
                    Score = (float)detectionResult.Score
                };

                // Toksik ise IsValid'ı false yapıp kaydedebiliriz, böylece admin onayı bekler.
                if (detectionResult.IsToxic)
                {
                    comment.IsValid = false; // Toksik ise, varsayılan olarak geçerli değil (admin onayı gerekecek)
                }
                else
                {
                    comment.IsValid = true; // Toksik değilse, varsayılan olarak geçerli
                }

                _commentService.TInsert(comment); // Yorumu her durumda kaydet

                if (detectionResult.IsToxic)
                {
                    return Json(new
                    {
                        success = true, // AJAX isteği başarılı oldu
                        type = "toxic", // Yanıt tipini belirtiyoruz
                        message = "Yorumunuz toksik içerik barındırıyor ve admin onayı bekleyecektir.",
                        comment = new // Yeni eklenen yorumu döndürebiliriz (isteğe bağlı)
                        {
                            userNameSurname = comment.UserNameSurname,
                            commentDetail = comment.CommentDetail,
                            commentDate = comment.CommentDate.ToString("dd-MMM-yyyy"),
                            userImageUrl = user.ımageurl,
                            isToxic = comment.IsToxic, // Toksik olduğu bilgisini de gönder
                            isValid = comment.IsValid // Geçerlilik durumunu da gönder
                        }
                    });
                }

                return Json(new
                {
                    success = true,
                    type = "success", // Yanıt tipini belirtiyoruz
                    message = "Yorumunuz başarıyla eklendi!",
                    comment = new // Yeni eklenen yorumu döndürebiliriz (isteğe bağlı)
                    {
                        userNameSurname = comment.UserNameSurname,
                        commentDetail = comment.CommentDetail,
                        commentDate = comment.CommentDate.ToString("dd-MMM-yyyy"),
                        userImageUrl = user.ımageurl,
                        isToxic = comment.IsToxic,
                        isValid = comment.IsValid
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddCommentAjax Hatası: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, type = "error", message = "Bir hata oluştu: " + ex.Message });
            }
        }

        // ... Diğer metodlar ...
    }
}