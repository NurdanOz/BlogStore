using BlogStore.EntityLayer.Entities;
using BlogStore.PresentationLayer.Controllers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogStore.PresentationLayer.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }



        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> CreateUser(UserRegisterViewModel userRegisterViewModel)
        {
            AppUser appUser = new AppUser()
            {
                ımageurl = "test",
                description = "test",
                name =userRegisterViewModel.Name,
                Email = userRegisterViewModel.Email,
                surname = userRegisterViewModel.Surname,
                UserName = userRegisterViewModel.Username,
            };
            await _userManager.CreateAsync(appUser, userRegisterViewModel.Password);
            return RedirectToAction("UserLogin", "Login");


        }

       

    }
}
