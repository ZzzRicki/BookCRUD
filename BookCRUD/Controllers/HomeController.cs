using BookCRUD.Models;
using BookCRUD.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }

            // Seleccionar solo las columnas que sabemos que existen
            var booksList = await books
                .Select(b => new Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    PublicationDate = b.PublicationDate
                    // No incluimos las nuevas columnas aquí
                })
                .OrderByDescending(b => b.PublicationDate)
                .Take(6)
                .AsNoTracking()
                .ToListAsync();

            return View(booksList);
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