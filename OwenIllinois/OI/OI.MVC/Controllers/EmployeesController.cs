using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OI.Entities.Models;
using OI.MVC.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using System.Collections.Generic;

namespace OI.MVC.Controllers
{
    [Authorize(Roles = "Admin, HR")]
    public class EmployeesController : Controller
    {
        private readonly IRepositoryAsync<Employee> _employeeRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public EmployeesController(IUnitOfWorkAsync unitOfWorkAsync,
            IRepositoryAsync<Employee> employeeRepositoryAsync)
        {
            _employeeRepositoryAsync = employeeRepositoryAsync;
            _unitOfWorkAsync = unitOfWorkAsync;
        }


        // GET: Employees
        public ActionResult Index()
        {
            var employees = _employeeRepositoryAsync.Query()
                .Include(c => c.Company)
                .OrderBy(o => o.OrderBy(e => e.Fullname))
                .Select().ToList();
                
            return View(employees);
        }

        // GET: Employees
        public ActionResult ViewList(int id)
        {
            return RedirectToAction("ViewDocument", "Document");
        }

        // GET: Employees
        public ActionResult Upload(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var employee = _employeeRepositoryAsync.Find(id);
            if (employee == null) return HttpNotFound();

            ViewBag.resultTypeList = new List<SelectListItem>() 
            {
                new SelectListItem 
                {
                    Text = "Drug Test",
                    Value = "DrugTest",
                    Selected = false,
                },
                new SelectListItem 
                {
                    Text = "ECG Test",
                    Value = "ECGTest",
                    Selected = false,
                },
                new SelectListItem 
                {
                    Text = "Spirometry Test",
                    Value = "SpirometryTest",
                    Selected = false,
                },
            };
            return View(employee);
        }

        public FileResult DownloadAllFiles()
        {
            var context = System.Web.HttpContext.Current;
            var folderPath = context.Server.MapPath("~/UploadedFiles/");
            var baseOutputStream = new MemoryStream();
            ZipOutputStream zipOutput = new ZipOutputStream(baseOutputStream) {IsStreamOwner = false};

            /*  
            * Higher compression level will cause higher usage of reources 
            * If not necessary do not use highest level 9 
            */

            zipOutput.SetLevel(4);
            SharpZipLibHelper.ZipFolder(folderPath, zipOutput);

            zipOutput.Finish();
            zipOutput.Close();

            /* Set position to 0 so that cient start reading of the stream from the begining */
            baseOutputStream.Position = 0;

            /* Set custom headers to force browser to download the file instad of trying to open it */
            return new FileStreamResult(baseOutputStream, "application/x-zip-compressed")
            {
                FileDownloadName = "eResult.zip"
            };  
        }

       [HttpPost]
       public ActionResult UploadFile(int id, FormCollection form)
       {
           if (Request.Files.Count <= 0) return RedirectToAction("Index", "Employees");
           var file = Request.Files[0];

           if (file == null || file.ContentLength <= 0) return RedirectToAction("Index", "Employees");
           var fileName = Path.GetFileName(file.FileName);
           if (fileName == null) return RedirectToAction("Index", "Employees");
           var path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
           file.SaveAs(path);

           var extension = Path.GetExtension(file.FileName);
           var description = form["resultType"].ToString();
           //var document = new Document()
           //{
           //    Filename = fileName,
           //    FilePath = "~/UploadedFiles/",
           //    SortCenter = description,
           //    ExamDate = DateTime.Now,
           //    DateUploaded = DateTime.Now,
           //    EmployeeId = id,
           //    ObjectState = ObjectState.Added,
           //};

           //_employeeRepositoryAsync.GetRepository<Document>().Insert(document);
           //_unitOfWorkAsync.SaveChanges();

           return RedirectToAction("Index", "Employees");
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
