using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class PublisherController : Controller
    {
        #region Fields
        private readonly IPublisherService _publisherService;
        private readonly IBookService _bookService;
        #endregion

        #region Constructor
        public PublisherController(IPublisherService publisherService, IBookService bookService)
        {
            this._publisherService = publisherService;
            _bookService = bookService;
        }
        #endregion

        #region Index
        // GET: PublisherController
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, Publisher")]
        public async Task<ActionResult> Index(string? name, string? country)
        {
            if (User.HasClaim("PublishersAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var publishers = await _publisherService.GetBySpecificationWithIncludesAsync(name, country);

            var publisherViewModels = publishers.Select(p => new PublisherViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Country = p.Country,
                FoundedYear = p.FoundedYear,
                TotalBooks = p.BookPublishers.Count
            }).ToList();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_PublishersTable", publisherViewModels);
            }

            return View(publisherViewModels);
        }
        #endregion

        #region Details
        // GET: PublisherController/Details/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, Publisher")]
        public async Task<ActionResult> Details(int id)
        {
            if (User.HasClaim("PublishersAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var publisher = await _publisherService.GetByIdWithIncludesAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }
        #endregion

        #region Create
        // GET: PublisherController/Create
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, Publisher")]
        public async Task<ActionResult> Create()
        {
            if (User.HasClaim("PublishersCreate", "false")) return RedirectToAction("AccessDenied", "Home");

            var books = await _bookService.GetAllWithIncludesAsync();
            ViewBag.Books = books;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PublisherDTO publisherDTO)
        {
            if (User.HasClaim("PublishersCreate", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _publisherService.AddAsync(publisherDTO);
            return Json(new { success = true });
        }

        #endregion

        #region Edit
        // GET: PublisherController/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, Publisher")]
        public async Task<ActionResult> Edit(int id)
        {
            if (User.HasClaim("PublishersEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            var publisher = await _publisherService.GetByIdWithIncludesAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            var books = await _bookService.GetAllWithIncludesAsync();
            ViewBag.Books = books.Select(book => new
            {
                book.Id,
                book.Title,
                book.Author,
                book.PublicationYear,
                IsSelected = publisher.BookPublishers.Any(bp => bp.BookId == book.Id)
            }).ToList();

            return View(publisher);
        }

        [HttpPut]
        public async Task<ActionResult> Edit([FromBody] PublisherDTO publisherDTO)
        {
            if (User.HasClaim("PublishersEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _publisherService.UpdateAsync(publisherDTO);
            return Json(new { success = true });
        }
        #endregion

        #region Delete
        // GET: PublisherController/Delete/{id}
        [HttpDelete]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, Publisher")]
        public async Task<ActionResult> Delete(int id)
        {
            if (User.HasClaim("PublishersDelete", "false")) return RedirectToAction("AccessDenied", "Home");

            var publisher = await _publisherService.GetByIdAsync(id);

            if (publisher == null)
                return Json(new { success = false, message = "Publisher not found" });

            await _publisherService.DeleteAsync(id);

            return Json(new { success = true, message = "Publisher deleted successfully" });
        }
        #endregion
    }
}
