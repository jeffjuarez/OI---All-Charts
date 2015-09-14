using System.Collections.Generic;
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
    [Authorize(Roles="Admin")]
    public class UserController : Controller
    {
        private readonly IRepositoryAsync<Account> _accountRepositoryAsync;
        private readonly IRepositoryAsync<Employee> _employeeRepositoryAsync;
        private readonly IRepositoryAsync<Role> _roleRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public UserController(IUnitOfWorkAsync unitOfWorkAsync,
            IRepositoryAsync<Account> accountRepositoryAsync, 
            IRepositoryAsync<Employee> employeeRepositoryAsync,
            IRepositoryAsync<Role> roleRepositoryAsync)
        {
            _accountRepositoryAsync = accountRepositoryAsync;
            _employeeRepositoryAsync = employeeRepositoryAsync;
            _roleRepositoryAsync = roleRepositoryAsync;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public ActionResult Index()
        {
            var account = _accountRepositoryAsync
                .Query()
                .Include(r => r.Role)
                .Include(e => e.Employee)
                .Select()
                .AsQueryable();

            return View(account);
        }

        public ActionResult UserEdit(int id)
        {
            IEnumerable<SelectListItem> categories = _employeeRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Fullname
                });
            ViewBag.EmployeeId = new SelectList(categories, "Value", "Text");

            IEnumerable<SelectListItem> roles = _roleRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.RoleType
                });
            ViewBag.RoleId = new SelectList(roles, "Value", "Text");

            var account = _accountRepositoryAsync
                .Query(x => x.Id == id)
                .Include(r => r.Role)
                .Select()
                .SingleOrDefault();

            return View("Edit", account);
        }

        public ActionResult UserAdd()
        {
            Account model = new Account();

            IEnumerable<SelectListItem> categories = _employeeRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Fullname
                });
            ViewBag.EmployeeId = new SelectList(categories, "Value", "Text");
            
            IEnumerable<SelectListItem> roles = _roleRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.RoleType
                });
            ViewBag.RoleId = new SelectList(roles, "Value", "Text");
            
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult UserEdit(Account model)
        {
            IEnumerable<SelectListItem> categories = _employeeRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Fullname
                });
            ViewBag.EmployeeId = new SelectList(categories, "Value", "Text");

            IEnumerable<SelectListItem> roles = _roleRepositoryAsync.Query().Select().ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.RoleType
                });
            ViewBag.RoleId = new SelectList(roles, "Value", "Text");

            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    var account = _accountRepositoryAsync
                        .Query(e => e.Username == model.Username)
                        .Include(r => r.Role)
                        .Select()
                        .SingleOrDefault();

                    if (account != null)
                    {
                        ModelState.AddModelError("Username", "Username already exists!");
                    }
                    else
                    {
                        var password = AccountHelpers.HashPassword(model.Password);
                        model.Salt = password.Salt;
                        model.Password = password.HashPassword;
                        _accountRepositoryAsync.Insert(model);
                        _unitOfWorkAsync.SaveChanges();
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    var account = _accountRepositoryAsync
                        .Query(e => e.Username == model.Username)
                        .Include(r => r.Role)
                        .Select()
                        .SingleOrDefault();

                    if (account != null)
                    {
                        if (account.Id != model.Id)
                        {
                            ModelState.AddModelError("Username", "Username already exists!");
                        }
                    }
                    else
                    {
                        if (model.Password != "")
                        {
                            var password = AccountHelpers.HashPassword(model.Password);
                            model.Salt = password.Salt;
                            model.Password = password.HashPassword;
                        }
                        _accountRepositoryAsync.Update(model);
                        _unitOfWorkAsync.SaveChanges();
                        return RedirectToAction("Index", "User");
                    }
                }
                
            }

            return View("Edit", model);
        }


    }
}