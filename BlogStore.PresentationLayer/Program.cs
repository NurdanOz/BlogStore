using BlogStore.BusinessLayer.Abstract;
using BlogStore.BusinessLayer.Concrete;
using BlogStore.BusinessLayer.Container;
using BlogStore.DataAccessLayer.Context;
using BlogStore.EntityLayer.Entities; 
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection; // AddHttpClient ve diðer DI metodlarý için gerekli olabilir

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusinessLayerDependencies();
builder.Services.AddDbContext<BlogContext>();


builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<BlogContext>();


builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IToxicityDetectionService, ToxicityManager>();
builder.Services.AddHttpClient<ITranslationService, TranslationManager>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();


app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();


app.MapStaticAssets();


app.MapControllerRoute(
    name: "articleDetail",
    pattern: "Article/ArticleDetail/{slug}",
    defaults: new { controller = "Article", action = "ArticleDetail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 


app.Run();