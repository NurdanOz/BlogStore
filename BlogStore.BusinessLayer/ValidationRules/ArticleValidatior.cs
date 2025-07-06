using BlogStore.EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.ValidationRules
{
    public class ArticleValidatior:AbstractValidator<Article>
    {

        public ArticleValidatior()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Başlık alanı boş geçilemez").MinimumLength(10).WithMessage("Lütfen en az 10 karakter veri girişi yapınız").MaximumLength(100).WithMessage("Lütfen en fazla 100 karakter veri girişi yapınız");
        }
    }
}
