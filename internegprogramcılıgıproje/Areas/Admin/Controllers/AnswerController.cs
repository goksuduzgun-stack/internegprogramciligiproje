using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using internegprogramcılıgıproje.Data;

namespace internegprogramcılıgıproje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnswerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnswerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Answer
        public async Task<IActionResult> Index()
        {
            var answers = await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return View(answers);
        }

        // POST: Admin/Answer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
                return NotFound();

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cevap başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
