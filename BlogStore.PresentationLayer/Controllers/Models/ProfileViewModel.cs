namespace BlogStore.PresentationLayer.Controllers.Models
{
    public class ProfileViewModel
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string description { get; set; }
        public string Email { get; set; }
        public string ımageurl { get; set; }

        // Şifre değiştirme için alanlar
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
