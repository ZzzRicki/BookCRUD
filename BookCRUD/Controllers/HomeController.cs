using BookCRUD.Data;
using BookCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookCRUD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var books = from b in _context.Books
                        select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) ||
                                         s.Author.Contains(searchString));
            }

            // Obtener los libros más recientes primero
            books = books.OrderByDescending(b => b.Id);

            return View(await books.AsNoTracking().ToListAsync());
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