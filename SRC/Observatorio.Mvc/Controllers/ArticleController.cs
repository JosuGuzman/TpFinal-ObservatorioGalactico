using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.DTOs.Requests;
using Observatorio.Core.DTOs.Responses;
using Observatorio.Core.Entities.Content;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Article;
using System.Security.Claims;

namespace Observatorio.Mvc.Controllers;

[Authorize]
public class ArticleController : BaseController
{
    private readonly IContentService _contentService;
    private readonly ILoggingService _loggingService;

    public ArticleController(IContentService contentService, ILoggingService loggingService)
    {
        _contentService = contentService;
        _loggingService = loggingService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        try
        {
            var articles = await _contentService.GetPublishedArticlesAsync();
            var totalArticles = articles.Count();
            
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArticleViewModel
                {
                    ArticleID = a.ArticleID,
                    Title = a.Title,
                    Slug = a.Slug,
                    Content = a.Content.Length > 200 ? a.Content.Substring(0, 200) + "..." : a.Content,
                    AuthorName = a.Author?.DisplayName ?? "Unknown",
                    State = a.State.ToString(),
                    Tags = a.Tags,
                    FeaturedImage = a.FeaturedImage,
                    PublishedAt = a.PublishedAt,
                    CreatedAt = a.CreatedAt,
                    IsPublished = a.IsPublished
                })
                .ToList();

            var model = new ArticleIndexViewModel
            {
                Articles = pagedArticles,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalArticles,
                TotalPages = (int)Math.Ceiling(totalArticles / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving articles");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(string slug)
    {
        try
        {
            var article = await _contentService.GetArticleBySlugAsync(slug);
            if (article == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (!article.IsPublished && article.AuthorUserID != currentUserId && !IsAdmin())
                return Forbid();

            var model = new ArticleViewModel
            {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Slug = article.Slug,
                Content = article.Content,
                AuthorName = article.Author?.DisplayName ?? "Unknown",
                AuthorUserID = article.AuthorUserID,
                State = article.State.ToString(),
                Tags = article.Tags,
                FeaturedImage = article.FeaturedImage,
                PublishedAt = article.PublishedAt,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                IsPublished = article.IsPublished,
                CanEdit = article.AuthorUserID == currentUserId || IsAdmin()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving article with slug: {slug}");
            return View("Error");
        }
    }

    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public IActionResult Create()
    {
        return View(new CreateArticleViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var authorId = GetCurrentUserId();
            var article = await _contentService.CreateArticleAsync(
                model.Title,
                model.Content,
                authorId,
                model.Tags,
                model.FeaturedImage);

            await _loggingService.LogInfoAsync("ArticleCreated",
                $"Article '{model.Title}' created by user {authorId}", authorId);

            TempData["SuccessMessage"] = "Article created successfully!";
            return RedirectToAction("Details", new { slug = article.Slug });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error creating article");
            return View(model);
        }
    }

    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (article.AuthorUserID != currentUserId && !IsAdmin())
                return Forbid();

            var model = new EditArticleViewModel
            {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Content = article.Content,
                State = article.State.ToString(),
                Tags = article.Tags,
                FeaturedImage = article.FeaturedImage
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving article for edit: {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditArticleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (article.AuthorUserID != currentUserId && !IsAdmin())
                return Forbid();

            await _contentService.UpdateArticleAsync(
                id,
                model.Title,
                model.Content,
                model.State,
                model.Tags);

            TempData["SuccessMessage"] = "Article updated successfully!";
            return RedirectToAction("Details", new { slug = article.Slug });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, $"Error updating article: {id}");
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (article.AuthorUserID != currentUserId && !IsAdmin())
                return Forbid();

            await _contentService.DeleteArticleAsync(id);
            
            await _loggingService.LogInfoAsync("ArticleDeleted",
                $"Article {id} deleted", currentUserId);

            TempData["SuccessMessage"] = "Article deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting article: {id}");
            TempData["ErrorMessage"] = "Error deleting article.";
            return RedirectToAction("Details", new { id });
        }
    }

    [Authorize(Roles = "Astronomer,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            await _contentService.PublishArticleAsync(id);
            
            await _loggingService.LogInfoAsync("ArticlePublished",
                $"Article {id} published", GetCurrentUserId());

            TempData["SuccessMessage"] = "Article published successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing article: {id}");
            TempData["ErrorMessage"] = "Error publishing article.";
            return RedirectToAction("Details", new { id });
        }
    }

    [Authorize(Roles = "Astronomer,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish(int id)
    {
        try
        {
            await _contentService.UnpublishArticleAsync(id);
            
            TempData["SuccessMessage"] = "Article unpublished successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error unpublishing article: {id}");
            TempData["ErrorMessage"] = "Error unpublishing article.";
            return RedirectToAction("Details", new { id });
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> ByAuthor(int authorId, int page = 1, int pageSize = 10)
    {
        try
        {
            var articles = await _contentService.GetArticlesByAuthorAsync(authorId);
            var currentUserId = GetCurrentUserId();

            // Solo mostrar artículos publicados a menos que sea el autor o admin
            if (authorId != currentUserId && !IsAdmin())
                articles = articles.Where(a => a.IsPublished).ToList();

            var totalArticles = articles.Count();
            
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArticleViewModel
                {
                    ArticleID = a.ArticleID,
                    Title = a.Title,
                    Slug = a.Slug,
                    Content = a.Content.Length > 200 ? a.Content.Substring(0, 200) + "..." : a.Content,
                    AuthorName = a.Author?.DisplayName ?? "Unknown",
                    State = a.State.ToString(),
                    PublishedAt = a.PublishedAt,
                    CreatedAt = a.CreatedAt,
                    IsPublished = a.IsPublished
                })
                .ToList();

            var model = new ArticleByAuthorViewModel
            {
                Articles = pagedArticles,
                AuthorId = authorId,
                AuthorName = articles.FirstOrDefault()?.Author?.DisplayName ?? "Unknown",
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalArticles,
                TotalPages = (int)Math.Ceiling(totalArticles / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving articles by author: {authorId}");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction("Index");

        try
        {
            var articles = await _contentService.SearchArticlesAsync(query);
            var publishedArticles = articles.Where(a => a.IsPublished).ToList();
            var totalArticles = publishedArticles.Count;
            
            var pagedArticles = publishedArticles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArticleViewModel
                {
                    ArticleID = a.ArticleID,
                    Title = a.Title,
                    Slug = a.Slug,
                    Content = a.Content.Length > 200 ? a.Content.Substring(0, 200) + "..." : a.Content,
                    AuthorName = a.Author?.DisplayName ?? "Unknown",
                    PublishedAt = a.PublishedAt,
                    CreatedAt = a.CreatedAt,
                    IsPublished = a.IsPublished
                })
                .ToList();

            var model = new ArticleSearchViewModel
            {
                Articles = pagedArticles,
                Query = query,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalArticles,
                TotalPages = (int)Math.Ceiling(totalArticles / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching articles: {query}");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Latest(int limit = 5)
    {
        try
        {
            var articles = await _contentService.GetLatestArticlesAsync(limit);
            
            var model = articles.Select(a => new ArticleViewModel
            {
                ArticleID = a.ArticleID,
                Title = a.Title,
                Slug = a.Slug,
                Content = a.Content.Length > 100 ? a.Content.Substring(0, 100) + "..." : a.Content,
                AuthorName = a.Author?.DisplayName ?? "Unknown",
                PublishedAt = a.PublishedAt,
                CreatedAt = a.CreatedAt
            }).ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest articles");
            return View("Error");
        }
    }
}