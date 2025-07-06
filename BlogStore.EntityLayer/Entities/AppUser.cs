using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.EntityLayer.Entities
{
    public class AppUser:IdentityUser
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string ımageurl { get; set; }
        public string description { get; set; }
       
        public List<Article> Articles { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
