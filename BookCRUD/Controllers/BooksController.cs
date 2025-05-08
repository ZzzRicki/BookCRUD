using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookCRUD.Data;
using BookCRUD.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BookCRUD.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "author" ? "author_desc" : "author";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CurrentFilter"] = searchString;

            var books = from b in _context.Books
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "author":
                    books = books.OrderBy(b => b.Author);
                    break;
                case "author_desc":
                    books = books.OrderByDescending(b => b.Author);
                    break;
                case "date":
                    books = books.OrderBy(b => b.PublicationDate);
                    break;
                case "date_desc":
                    books = books.OrderByDescending(b => b.PublicationDate);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }

            // Seleccionar solo las columnas que sabemos que existen
            var booksList = await books
                .Select(b => new Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    PublicationDate = b.PublicationDate,
                    // Incluir las nuevas columnas con manejo seguro
                    FilePath = b.FilePath ?? null,
                    Content = b.Content ?? null,
                    Format = b.Format,
                    CoverImageUrl = b.CoverImageUrl ?? null
                })
                .AsNoTracking()
                .ToListAsync();

            return View(booksList);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,PublicationDate,Content,CoverImageUrl")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Si hay contenido, establecer el formato como Text
                if (!string.IsNullOrEmpty(book.Content))
                {
                    book.Format = BookFormat.Text;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Libro creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,PublicationDate,FilePath,Content,Format,CoverImageUrl")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Si hay contenido pero no hay formato establecido, establecerlo como Text
                    if (!string.IsNullOrEmpty(book.Content) && book.Format == 0)
                    {
                        book.Format = BookFormat.Text;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Libro actualizado correctamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Si hay un archivo asociado, eliminarlo
            if (!string.IsNullOrEmpty(book.FilePath))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", book.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Libro eliminado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Read/5
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/UploadBookFile/5
        [HttpPost]
        public async Task<IActionResult> UploadBookFile(int id, IFormFile bookFile)
        {
            if (bookFile == null || bookFile.Length == 0)
            {
                TempData["ErrorMessage"] = "No se ha seleccionado ningún archivo";
                return RedirectToAction(nameof(Details), new { id });
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Determinar el formato basado en la extensión
            string extension = Path.GetExtension(bookFile.FileName).ToLowerInvariant();
            if (extension == ".pdf")
            {
                book.Format = BookFormat.PDF;
            }
            else if (extension == ".epub")
            {
                book.Format = BookFormat.EPUB;
            }
            else if (extension == ".txt")
            {
                book.Format = BookFormat.Text;

                // Leer el contenido del archivo de texto
                using (var reader = new StreamReader(bookFile.OpenReadStream()))
                {
                    book.Content = await reader.ReadToEndAsync();
                }

                _context.Update(book);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contenido del libro subido correctamente";
                return RedirectToAction(nameof(Details), new { id = book.Id });
            }
            else
            {
                TempData["ErrorMessage"] = "Formato de archivo no soportado. Use PDF, EPUB o TXT.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Guardar el archivo
            string fileName = $"{Guid.NewGuid()}{extension}";
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");

            // Asegurarse de que el directorio existe
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bookFile.CopyToAsync(stream);
            }

            book.FilePath = fileName;
            _context.Update(book);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Archivo del libro subido correctamente";
            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}