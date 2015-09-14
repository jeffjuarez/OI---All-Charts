using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using OI.Entities.Models;
using OI.MVC.Helpers;
using OI.MVC.Models;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace OI.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepositoryAsync<Account> _accountRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public AccountController(IUnitOfWorkAsync unitOfWorkAsync,
            IRepositoryAsync<Account> accountRepositoryAsync)
        {
            _accountRepositoryAsync = accountRepositoryAsync;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var account = _accountRepositoryAsync
                .Query(x => x.Username == model.UserName)
                .Include(r => r.Role)
                .Select()
                .SingleOrDefault();

            if (account != null)
            {
                var role = account.Role.RoleType;
               // if (AccountHelpers.ValidatePassword(model.Password, account.Salt, account.Password))
               // {
                    FormsAuthentication.SetAuthCookie(account.Username, false);

                    System.Web.HttpContext.Current.Session["_EmployeeID"] = account.EmployeeId;
                    return account.IsResetPassword ? RedirectToAction("Manage", "Account") : RedirectToAction("UploadForm", "Document");
               // }
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.OldPasswordFailed ? "Old password did not match."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageUserViewModel model)
        {
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (!ModelState.IsValid) return View(model);
            var username = HttpContext.User.Identity.Name;
            var account = _accountRepositoryAsync
                .Query(x => x.Username == username)
                .Include(r => r.Role)
                .Select()
                .SingleOrDefault();

            if (account == null)
                return RedirectToAction("Manage", new {Message = ManageMessageId.Error});

            // validate first for old password
            if (!AccountHelpers.ValidatePassword(model.OldPassword, account.Salt, account.Password))
                return RedirectToAction("Manage", new { Message = ManageMessageId.OldPasswordFailed });

             
            var newPassword = AccountHelpers.HashPassword(model.NewPassword);
            account.Salt = newPassword.Salt;
            account.Password = newPassword.HashPassword;
            account.IsResetPassword = false;
            account.ObjectState = ObjectState.Modified;

            _accountRepositoryAsync.Update(account);
            _unitOfWorkAsync.SaveChanges();

            var role = account.Role.RoleType;
            return RedirectToAction("Index", IsAdminHr(role) ? "Employees" : "Document");
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //return Redirect("http://design1.casamedicaph.com/");
            return RedirectToAction("Login", "Account");
        }

        public enum ManageMessageId
        {
            OldPasswordFailed,
            Error
        }

        private bool IsAdminHr(string role)
        {
            return role == "Admin" || role == "HR";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWorkAsync.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
