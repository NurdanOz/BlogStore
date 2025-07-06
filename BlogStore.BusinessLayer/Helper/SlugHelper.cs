using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Helpers
{
    public static class SlugHelper
    {

        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            title = title.Replace("ı", "i")
                        .Replace("ğ", "g")
                        .Replace("ü", "u")
                        .Replace("ş", "s")
                        .Replace("ö", "o")
                        .Replace("ç", "c")
                        .Replace("İ", "i")
                        .Replace("Ğ", "g")
                        .Replace("Ü", "u")
                        .Replace("Ş", "s")
                        .Replace("Ö", "o")
                        .Replace("Ç", "c");


            title = title.ToLowerInvariant();

            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");

            title = Regex.Replace(title, @"\s+", " ").Trim();

            title = Regex.Replace(title, @"\s", "-");

            return title;
        }
    }
}
