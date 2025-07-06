using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BlogStore.BusinessLayer.Helpers;
using BlogStore.PresentationLayer.Controllers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;


namespace BlogStore.PresentationLayer.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthorController(IArticleService articleService, ICategoryService categoryService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {

                return NotFound($"Kullanıcı yüklenemedi '{_userManager.GetUserName(User)}'.");
            }


            var model = new ProfileViewModel
            {
                name = user.name,
                surname = user.surname,
                description = user.description,
                Email = user.Email,
                ımageurl = user.ımageurl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {

            ModelState.Remove(nameof(model.CurrentPassword));
            ModelState.Remove(nameof(model.NewPassword));
            ModelState.Remove(nameof(model.ConfirmNewPassword));

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound($"Kullanıcı yüklenemedi '{_userManager.GetUserName(User)}'.");
            }


            user.name = model.name;
            user.surname = model.surname;
            user.description = model.description;
            user.ımageurl = model.ımageurl;


            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }


            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ProfileViewModel model)
        {

            ModelState.Remove(nameof(model.name));
            ModelState.Remove(nameof(model.surname));
            ModelState.Remove(nameof(model.description));
            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.ımageurl));

            if (!ModelState.IsValid)
            {

                var user1 = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user1 != null)
                {
                    model.name = user1.name;
                    model.surname = user1.surname;
                    model.description = user1.description;
                    model.Email = user1.Email;
                    model.ımageurl = user1.ımageurl;
                }
                return View("Profile", model);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound($"Kullanıcı yüklenemedi '{_userManager.GetUserName(User)}'.");
            }


            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                ModelState.AddModelError(string.Empty, "Tüm şifre alanları zorunludur.");

                var currentUserData = await _userManager.FindByNameAsync(User.Identity.Name);
                if (currentUserData != null)
                {
                    model.name = currentUserData.name;
                    model.surname = currentUserData.surname;
                    model.description = currentUserData.description;
                    model.Email = currentUserData.Email;
                    model.ımageurl = currentUserData.ımageurl;
                }
                return View("Profile", model);
            }


            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError(string.Empty, "Yeni şifreler eşleşmiyor.");

                var currentUserData = await _userManager.FindByNameAsync(User.Identity.Name);
                if (currentUserData != null)
                {
                    model.name = currentUserData.name;
                    model.surname = currentUserData.surname;
                    model.description = currentUserData.description;
                    model.Email = currentUserData.Email;
                    model.ımageurl = currentUserData.ımageurl;
                }
                return View("Profile", model);
            }


            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var currentUserData = await _userManager.FindByNameAsync(User.Identity.Name);
                if (currentUserData != null)
                {
                    model.name = currentUserData.name;
                    model.surname = currentUserData.surname;
                    model.description = currentUserData.description;
                    model.Email = currentUserData.Email;
                    model.ımageurl = currentUserData.ımageurl;
                }
                return View("Profile", model);
            }

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi!";

            return RedirectToAction("Profile");
        }


        [HttpGet]
        public IActionResult CreateArticle()
        {
            List<SelectListItem> values = (from x in _categoryService.TGetAll()
                                           select new SelectListItem
                                           {
                                               Text = x.CategoryName,
                                               Value = x.CategoryId.ToString()
                                           }).ToList();
            ViewBag.v = values;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle(Article article)
        {
            var userProfile = await _userManager.FindByNameAsync(User.Identity.Name);

            article.AppUserId = userProfile.Id;
            article.CreatedDate = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(article.Title))
            {
                article.Slug = SlugHelper.GenerateSlug(article.Title);
            }
            else
            {

            }

            _articleService.TInsert(article);
            return RedirectToAction("Index", "Default");
        }

        public async Task<IActionResult> MyArticleList()
        {
            var userProfile = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = _articleService.TGetArticleByAppUser(userProfile.Id);
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorList(string categoryId = null)
        {
            var authors = await _userManager.Users.ToListAsync();
            var authorViewModels = new List<AuthorViewModel>();

            foreach (var author in authors)
            {
                var viewModel = new AuthorViewModel
                {
                    Id = author.Id,
                    name = author.name,
                    surname = author.surname,
                    description = author.description,
                    ımageUrl = author.ımageurl
                };

                
                var articlesOfAuthor = _articleService.TGetArticlesByAppUserWithCategories(author.Id);

                var categoriesWrittenByAuthor = articlesOfAuthor
                                     .Where(a => a.category != null)
                                     .Select(a => a.category.CategoryName)
                                     .Distinct()
                                     .ToList();

                viewModel.WrittenCategories = categoriesWrittenByAuthor;
                viewModel.ArticleCount = articlesOfAuthor.Count();

                authorViewModels.Add(viewModel);
            }

            
            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out int selectedCategoryId))
            {
                var filteredAuthors = new List<AuthorViewModel>();
                foreach (var authorVm in authorViewModels)
                {
                    
                    var articles = _articleService.TGetArticlesByAppUserWithCategories(authorVm.Id);

                    if (articles.Any(a => a.category != null && a.category.CategoryId == selectedCategoryId))
                    {
                        filteredAuthors.Add(authorVm);
                    }
                }
                authorViewModels = filteredAuthors;
            }

            List<SelectListItem> categoryList = (from x in _categoryService.TGetAll()
                                                 select new SelectListItem
                                                 {
                                                     Text = x.CategoryName,
                                                     Value = x.CategoryId.ToString()
                                                 }).ToList();

            ViewBag.Categories = categoryList;
            ViewBag.SelectedCategory = categoryId;

            return View(authorViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorProfileDetail(string id) 
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var user = await _userManager.FindByIdAsync(id); 
            if (user == null)
            {
                return NotFound($"ID'si '{id}' olan kullanıcı bulunamadı.");
            }

            
            var articlesOfAuthor = _articleService.TGetArticlesByAppUserWithCategories(user.Id);

            var model = new AuthorProfileDetailViewModel
            {
                Id = user.Id,
                name = user.name,
                surname = user.surname,
                description = user.description,
                Email = user.Email,
                ımageurl = user.ımageurl, 
                Articles = articlesOfAuthor.OrderByDescending(a => a.CreatedDate).ToList()
            };

            return View(model);
        }

        
      

    }
}





