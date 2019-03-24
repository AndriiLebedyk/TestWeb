using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TestWeb.Models;
using TestWeb.Models.ManageViewModels;

namespace TestWeb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller {
        private readonly UserManager<ApplicationUser> m_userManager;
        private readonly SignInManager<ApplicationUser> m_signInManager;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UrlEncoder urlEncoder) {
            m_userManager = userManager;
            m_signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            var model = new IndexViewModel {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model) {
            if (!ModelState.IsValid)return View(model);

            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            if (model.Email != user.Email) {
                var setEmailResult = await m_userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded) {
                    throw new ApplicationException(
                        $"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber) {
                var setPhoneResult = await m_userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded) {
                    throw new ApplicationException(
                        $"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            var hasPassword = await m_userManager.HasPasswordAsync(user);

            if (!hasPassword) return RedirectToAction(nameof(SetPassword));

            var model = new ChangePasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (!ModelState.IsValid)return View(model);

            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            var changePasswordResult =
                await m_userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded) {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await m_signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword() {
            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            var hasPassword = await m_userManager.HasPasswordAsync(user);

            if (hasPassword) return RedirectToAction(nameof(ChangePassword));

            var model = new SetPasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = await m_userManager.GetUserAsync(User);

            if (user == null) throw new ApplicationException($"Unable to load user with ID '{m_userManager.GetUserId(User)}'.");

            var addPasswordResult = await m_userManager.AddPasswordAsync(user, model.NewPassword);

            if (!addPasswordResult.Succeeded) {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await m_signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }
        #region Helpers

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}