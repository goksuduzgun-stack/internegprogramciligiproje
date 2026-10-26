using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using internegprogramcılıgıproje.Data;
using internegprogramcılıgıproje.Models;
using System.Security.Claims;

namespace internegprogramcılıgıproje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        // POST: Admin/User/ChangeRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Kendi rolünü değiştirmeyi engelle
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "Kendi rolünüzü değiştiremezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            user.Role = user.Role == "Admin" ? "Member" : "Admin";
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{user.UserName} kullanıcısının rolü '{user.Role}' olarak değiştirildi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Kendi hesabını silmeyi engelle
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{user.UserName} kullanıcısı silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
