using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        #region Fields
        private readonly UserManager<IdentityUser> _userManager;
        #endregion

        #region Constructor
        public AdminController(UserManager<IdentityUser> userManager) => _userManager = userManager;
        #endregion

        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
        #endregion

        #region DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User successfully deleted";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region ManageUser
        [HttpGet]
        public async Task<IActionResult> ManageUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var lisRoles = Enum.GetValues(typeof(Role))
                        .Cast<Role>()
                        .ToList();

            var roles = new RolesModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = lisRoles,
                UserRole = userRole
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var lisClaims = Enum.GetValues(typeof(UserClaims))
                        .Cast<UserClaims>()
                        .ToList();

            var claims = new ClaimsModel
            {
                UserId = user.Id,
                UserClaims = lisClaims,
                Claims = userClaims.Select(c => new UserClaimViewModel
                {
                    ClaimType = c.Type,
                    ClaimValue = c.Value,

                }).ToList()
            };

            var model = new ManageUserViewModel
            {
                Roles = roles,
                Claims = claims
            };

            return View(model);
        }
        #endregion

        #region Roles
        [HttpPost]
        public async Task<IActionResult> ChangeRole(RolesModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("ManageUser", new { userId = model.UserId });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "The current roles could not be deleted.";
                return RedirectToAction("ManageUser", new { userId = model.UserId });
            }

            result = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (!result.Succeeded)
            {

                return RedirectToAction("ManageUser", new { userId = model.UserId });
            }

            TempData["SuccessMessage"] = "The selected role was added.";
            return RedirectToAction("ManageUser", new { userId = model.UserId });
        }
        #endregion

        #region Claims
        [HttpPost]
        public async Task<IActionResult> AddClaim(ClaimsModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("ManageUser", new { userId = model.UserId });
            }

            var claim = new System.Security.Claims.Claim(model.NewClaimType, model.NewClaimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "The claim could not be added.";
                return RedirectToAction("ManageUser", new { userId = model.UserId });
            }

            TempData["SuccessMessage"] = "The claim is added.";
            return RedirectToAction("ManageUser", new { userId = model.UserId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveClaim(string userId, string claimType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("ManageUser", new { userId = userId });
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var claimToRemove = claims.FirstOrDefault(c => c.Type == claimType);

            if (claimToRemove != null)
            {
                var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "The claim could not be deleted";
                    return RedirectToAction("ManageUser", new { userId = userId });
                }
            }

            TempData["SuccessMessage"] = "The claim is deleted";
            return RedirectToAction("ManageUser", new { userId = userId });
        }
        #endregion
    }
}
