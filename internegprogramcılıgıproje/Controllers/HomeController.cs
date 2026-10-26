using internegprogramcılıgıproje.Data;
using internegprogramcılıgıproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace internegprogramcılıgıproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalQuestions = await _context.Questions.CountAsync();
            ViewBag.TotalAnswers = await _context.Answers.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();

            var latestQuestions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Category)
                .Include(q => q.Answers)
                .Where(q => q.IsApproved)
                .OrderByDescending(q => q.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(latestQuestions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
