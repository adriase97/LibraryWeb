using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        #region Fields
        private readonly IAuthorService _authorService;
        #endregion

        #region Constructor
        public AuthorController(IAuthorService authorService) => this._authorService = authorService;
        #endregion

        #region Index
        // GET: AuthorController
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks, ViewAuthorsBooks")]
        public async Task<ActionResult> Index(string? name)
        {
            if (User.HasClaim("AuthorsAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var authors = await _authorService.GetBySpecificationWithIncludesAsync(name);

            var authorViewModels = authors.Select(a => new AuthorViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Nationality = a.Nationality,
                BirthDate = a.BirthDate,
                TotalBooks = a.Books.Count
            }).ToList();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return PartialView("_AuthorsTable", authorViewModels);
            }

            return View(authorViewModels);
        }
        #endregion

        #region Details
        // GET: AuthorController/Details/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks, ViewAuthorsBooks")]
        public async Task<ActionResult> Details(int id)
        {
            if (User.HasClaim("AuthorsAccess", "false")) return RedirectToAction("AccessDenied", "Home");

            var author = await _authorService.GetByIdWithIncludesAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }
        #endregion

        #region Create
        // GET: AuthorController/Create
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public ActionResult Create()
        {
            if (User.HasClaim("AuthorsCreate", "false")) return RedirectToAction("AccessDenied", "Home");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Create(AuthorDTO authorDTO)
        {
            if (User.HasClaim("AuthorsCreate", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _authorService.AddAsync(authorDTO);
            return Json(new { success = true });
        }

        #endregion

        #region Edit
        // GET: AuthorController/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Edit(int id)
        {
            if (User.HasClaim("AuthorsEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            var author = await _authorService.GetByIdWithIncludesAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPut]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Edit(AuthorDTO authorDTO)
        {
            if (User.HasClaim("AuthorsEdit", "false")) return RedirectToAction("AccessDenied", "Home");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed", errors });
            }

            await _authorService.UpdateAsync(authorDTO);
            return Json(new { success = true });
        }
        #endregion

        #region Delete
        // GET: AuthorController/Delete/{id}
        [HttpDelete]
        [Authorize(Roles = "Admin, AuthorsBooksPublisher, AuthorsBooks")]
        public async Task<ActionResult> Delete(int id)
        {
            if (User.HasClaim("AuthorsDelete", "false")) return RedirectToAction("AccessDenied", "Home");

            var author = await _authorService.GetByIdAsync(id);

            if (author == null)
                return Json(new { success = false, message = "Author not found" });

            await _authorService.DeleteAsync(id);

            return Json(new { success = true, message = "Author deleted successfully" });
        }
        #endregion

        #region Public Methods
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _authorService.GetAllAsync();
            var authorList = authors.Select(a => new { id = a.Id, name = a.Name }).ToList();

            return Json(authorList);
        }
        #endregion
    }
}
