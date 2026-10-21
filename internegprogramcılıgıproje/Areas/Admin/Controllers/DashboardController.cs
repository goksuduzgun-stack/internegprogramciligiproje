using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using internegprogramcılıgıproje.Data;

namespace internegprogramcılıgıproje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalQuestions = await _context.Questions.CountAsync();
            ViewBag.TotalAnswers = await _context.Answers.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            var recentQuestions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Category)
                .OrderByDescending(q => q.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(recentQuestions);
        }
    }
}