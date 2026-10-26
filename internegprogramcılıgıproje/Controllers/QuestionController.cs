using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using internegprogramcılıgıproje.Data;
using internegprogramcılıgıproje.Models;
using System.Security.Claims;

namespace internegprogramcılıgıproje.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Soruları Listele (Filtreleme & Arama)
        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            var query = _context.Questions
                .Include(q => q.Category)
                .Include(q => q.User)
                .Include(q => q.Answers)
                .Where(q => q.IsApproved)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(q => q.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(q => q.Title.Contains(search) || q.Content.Contains(search));

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.Search = search;

            var questions = await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
            return View(questions);
        }

        // Soru Detayı & Cevapları Görüntüle
        public async Task<IActionResult> Details(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Category)
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsApproved);

            if (question == null)
                return NotFound();

            // Görüntülenme sayısını 1 artır
            question.ViewCount++;
            await _context.SaveChangesAsync();

            return View(question);
        }

        // Yeni Soru Sor (Sadece Giriş Yapanlar)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            var model = new QuestionCreateViewModel
            {
                Categories = categories
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var question = new Question
            {
                Title = model.Title,
                Content = model.Content,
                CategoryId = model.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = question.Id });
        }

        // Soruya Cevap Ekle
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer(AnswerCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = model.QuestionId });

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var answer = new Answer
            {
                QuestionId = model.QuestionId,
                Content = model.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = model.QuestionId });
        }

        // AJAX ile Cevap Ekle (Sayfa yenilemeden)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAnswerAjax([FromBody] AnswerCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Content) || model.Content.Length < 5)
                return Json(new { success = false, message = "Cevap en az 5 karakter olmalıdır." });

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Json(new { success = false, message = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın." });

            var answer = new Answer
            {
                QuestionId = model.QuestionId,
                Content = model.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Cevabınız başarıyla eklendi!",
                answer = new
                {
                    id = answer.Id,
                    content = answer.Content,
                    username = User.FindFirstValue(ClaimTypes.Name),
                    fullname = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name),
                    createdDate = answer.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm")
                }
            });
        }
    }
}