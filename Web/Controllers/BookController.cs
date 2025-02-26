using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        #region Fields
        private readonly IBookService _bookService;
        #endregion

        #region Constructor
        public BookController(IBookService bookService) => this._bookService = bookService;
        #endregion

        #region Index
        // GET: BookController
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks, ViewAuthorsBooks")]
        public async Task<ActionResult> Index(string? title, Genre? genre, decimal? minPrice, decimal? maxPrice)
        {
            if (User.HasClaim("BooksAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var books = await _bookService.GetBySpecificationWithIncludesAsync(title, genre, minPrice, maxPrice);

            var bookViewModels = books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Genre = b.Genre,
                Price = b.Price,
                AuthorName = b.Author.Name
            }).ToList();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_BooksTable", bookViewModels);
            }

            return View(bookViewModels);
        }
        #endregion

        #region Details
        // GET: BookController/Details/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks, ViewAuthorsBooks")]
        public async Task<ActionResult> Details(int id)
        {
            if (User.HasClaim("BooksAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var book = await _bookService.GetByIdWithIncludesAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }
        #endregion

        #region Create
        // GET: BookController/Create
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public ActionResult Create()
        {
            if (User.HasClaim("BooksCreate", "false")) return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Create(BookDTO bookDTO)
        {
            if (User.HasClaim("BooksCreate", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _bookService.AddAsync(bookDTO);
            return Json(new { success = true });
        }

        #endregion

        #region Edit
        // GET: BookController/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Edit(int id)
        {
            if (User.HasClaim("BooksEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            var book = await _bookService.GetByIdWithIncludesAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPut]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Edit(BookDTO bookDTO)
        {
            if (User.HasClaim("BooksEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _bookService.UpdateAsync(bookDTO);
            return Json(new { success = true });
        }
        #endregion

        #region Delete
        // GET: BookController/Delete/{id}
        [HttpDelete]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Delete(int id)
        {
            if (User.HasClaim("BooksDelete", "false")) return RedirectToAction("AccessDenied", "Home");

            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
                return Json(new { success = false, message = "Book not found" });

            await _bookService.DeleteAsync(id);

            return Json(new { success = true, message = "Book deleted successfully" });
        }
        #endregion
    }
}
