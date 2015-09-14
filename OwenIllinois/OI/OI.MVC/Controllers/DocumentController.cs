using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using OI.Entities.Models;
using OI.MVC.Models;
using System.Web;
using System.Collections.Generic;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;

using Excel;
using System.Text;
using System.Data;



namespace OI.MVC.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IRepositoryAsync<Document> _documentRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;


        public DocumentController(IUnitOfWorkAsync unitOfWorkAsync,
            IRepositoryAsync<Document> documentRepositoryAsync)
        {
            _documentRepositoryAsync = documentRepositoryAsync;
                        _unitOfWorkAsync = unitOfWorkAsync;
        }



        // GET: /Document/
        public ActionResult UploadForm()
        {
            var username = HttpContext.User.Identity.Name;
            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();

            username = employee.EmployeeNumber; // EmployeeNumber = UserId
            var viewModels = _documentRepositoryAsync
                .Query(e => e.EmployeeId == employee.Id)
                .Select(d => new DocumentViewModel()
                {
                    Id = d.Id,
                    SortCenter = d.SortCenter,
                    Filename = d.Filename,
                    ReceivedDateFrom = d.ReceivedDateFrom,
                    PreparedBy = d.PreparedBy,
                    DateUploaded = d.DateUploaded,
                    UserUploaded = username,
                    EmployeeId = employee.Id,

                })
                .ToList();

            return View(viewModels);
        }

        //
        // GET: /Document/
        [Authorize(Roles = "Admin, HR")]
        public ActionResult Documents(int id)
        {
            var username = HttpContext.User.Identity.Name;
            var viewModels = _documentRepositoryAsync
                .Query(e => e.EmployeeId == id)
                .Select(d => new DocumentViewModel()
                {
                    Id = d.Id,
                    SortCenter = d.SortCenter,
                    Filename = d.Filename,
                    ReceivedDateFrom = d.ReceivedDateFrom,
                    PreparedBy = d.PreparedBy,
                    DateUploaded = DateTime.Now,
                    UserUploaded = username,
                })
                .ToList();

            return View(viewModels);
        }

        //
        // GET: /Document/
        public FileResult View(int id)
        {
            var document = _documentRepositoryAsync
                .Find(id);

            var fileName = String.Concat(Server.MapPath(document.FilePath), document.Filename);
            var extension = Path.GetExtension(fileName);
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //if (extension == ".jpg" || extension == ".jpeg")
            //    contentType = "image/jpeg";

            return File(fileName, contentType);
        }



        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadedFile)
        {
            int _CurrentEmpID = (int)System.Web.HttpContext.Current.Session["_EmployeeId"];
            if (uploadedFile != null)
            {
                //   string strFilePath = uploadedFile.
                string strPath = Server.MapPath("~\\UploadedFiles\\");
                // save file
                string nameAndLocation = strPath + uploadedFile.FileName;
                uploadedFile.SaveAs(nameAndLocation);

                //PERFORM UPDATES IN APE FORMS DB
                ReadFile(_CurrentEmpID, nameAndLocation, uploadedFile.FileName);

            }

            //return RedirectToAction("Printed");
            return RedirectToAction("UploadForm", "Document");
        }




        public ActionResult ReadFile(int EmployeeID, string filePath, string filename)
        {
          
            var lstDocument = new List<Document>();

            // DOCUMENT HEADER
            string strHEADER_SORTCENTER = string.Empty;
            string strHEADER_REC_DATEFROM = string.Empty;
            string strHEADER_PREPAREDBY = string.Empty;



            //PRODUCTIVITY
            //**100n**////**201u**////**300u**////**400u**//
            double _Productivity_100n = 0;
            double _Productivity_201u = 0;
            double _Productivity_300u = 0;
            double _Productivity_400u = 0;


            // UR/SC RATIO
            double _URSC_BL_Pallet = 0;
            double _URSC_UR_Pallet = 0;
            double _URSC_SC_Pallet = 0;
            double _URSC_BL_Composite = 0;
            double _URSC_UR_Composite = 0;
            double _URSC_SC_Composite = 0;
            double _URSC_BL_HollowProfile = 0;
            double _URSC_UR_HollowProfile = 0;
            double _URSC_SC_HollowProfile = 0;
            double _URSC_BL_TopFrames = 0;
            double _URSC_UR_TopFrames = 0;
            double _URSC_SC_TopFrames = 0;

         

            //TRUCK MOVEMENT RATIO
            double _Truck_Movement_IN = 0;
            double _Truck_Movement_OUT = 0;
            double _Truck_Movement_Ratio = 0;



            //PACKED ITEM RECEIVED
            //**100n**////**201u**////**300u**////**400u**//
            double _PackedItemsRec_100n_Pallet_TotalRec = 0;
            double _PackedItemsRec_201u_Composite_TotalRec = 0;
            double _PackedItemsRec_300u_BlackHollowProfile_TotalRec = 0;
            double _PackedItemsRec_400u_TopFrames_TotalRec = 0;

            //PACKED ITEM DISPATCHED
            //**100n**////**201u**////**300u**////**400u**//
            double _PackedItemsDisp_100n_Pallet_TotalDisp = 0;
            double _PackedItemsDisp_201u_Composite_TotalDisp = 0;
            double _PackedItemsDisp_300u_BlackHollowProfile_TotalDisp = 0;
            double _PackedItemsDisp_400u_TopFrames_TotalDisp = 0;



            //STOCK ON HAND - QI,UR,BL
            double _Stock_Pallet_QI = 0;
            double _Stock_Composite_QI = 0;
            double _Stock_HollowProfile_QI = 0;
            double _Stock_TopFrames_QI = 0;

            double _Stock_Pallet_UR = 0;
            double _Stock_Composite_UR = 0;
            double _Stock_HollowProfile_UR = 0;
            double _Stock_TopFrames_UR = 0;

            double _Stock_Pallet_BL = 0;
            double _Stock_Composite_BL = 0;
            double _Stock_HollowProfile_BL = 0;
            double _Stock_TopFrames_BL = 0;


            var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);


            //Reading from a OpenXml Excel file (2007 format; *.xlsx)
            //      FileStream stream = new FileStream("../../myxlsx/sample.xlsx", FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet();

          
            //Data Reader methods
            foreach (DataTable table in result.Tables)
            {
                
                if (table.TableName == "Weekly Report")
                {

                            //HEADER 
                            strHEADER_SORTCENTER = table.Rows[5][2].ToString();
                            strHEADER_REC_DATEFROM = table.Rows[6][2].ToString();
                            strHEADER_PREPAREDBY = table.Rows[7][2].ToString();

                            //   PRODUCTIVITY
                            _Productivity_100n = Math.Round(Convert.ToDouble(table.Rows[77][2]), 0);
                            _Productivity_201u = Math.Round(Convert.ToDouble(table.Rows[77][3]), 0);
                            _Productivity_300u = Math.Round(Convert.ToDouble(table.Rows[77][4]), 0);
                            _Productivity_400u = Math.Round(Convert.ToDouble(table.Rows[77][5]), 0);


                            //    UR/SC RATIO
                            _URSC_BL_Pallet = Math.Round(Convert.ToDouble(table.Rows[69][2]) * 100, 1);
                            _URSC_UR_Pallet = Math.Round(Convert.ToDouble(table.Rows[70][2]) * 100, 1);
                            _URSC_SC_Pallet = Math.Round(Convert.ToDouble(table.Rows[71][2]) * 100, 1);
                            _URSC_BL_Composite = Math.Round(Convert.ToDouble(table.Rows[69][3]) * 100, 1);
                            _URSC_UR_Composite = Math.Round(Convert.ToDouble(table.Rows[70][3]) * 100, 1);
                            _URSC_SC_Composite = Math.Round(Convert.ToDouble(table.Rows[71][3]) * 100, 1);
                            _URSC_BL_HollowProfile = Math.Round(Convert.ToDouble(table.Rows[69][4]) * 100, 1);
                            _URSC_UR_HollowProfile = Math.Round(Convert.ToDouble(table.Rows[70][4]) * 100, 1);
                            _URSC_SC_HollowProfile = Math.Round(Convert.ToDouble(table.Rows[71][4]) * 100, 1);
                            _URSC_BL_TopFrames = Math.Round(Convert.ToDouble(table.Rows[69][5]) * 100, 1);
                            _URSC_UR_TopFrames = Math.Round(Convert.ToDouble(table.Rows[70][5]) * 100, 1);
                            _URSC_SC_TopFrames = Math.Round(Convert.ToDouble(table.Rows[71][5]) * 100, 1);


                            //PACKED ITEM RECEIVED
                            //**100n**////**201u**////**300u**////**400u**//
                            _PackedItemsRec_100n_Pallet_TotalRec = (double)table.Rows[16][2];
                            _PackedItemsRec_201u_Composite_TotalRec = (double)table.Rows[16][3];
                            _PackedItemsRec_300u_BlackHollowProfile_TotalRec = (double)table.Rows[16][4];
                            _PackedItemsRec_400u_TopFrames_TotalRec = (double)table.Rows[16][5];
                            //PACKED ITEM DISPATCHED
                            //**100n**////**201u**////**300u**////**400u**//
                            _PackedItemsDisp_100n_Pallet_TotalDisp = (double)table.Rows[20][2];
                            _PackedItemsDisp_201u_Composite_TotalDisp = (double)table.Rows[20][3];
                            _PackedItemsDisp_300u_BlackHollowProfile_TotalDisp = (double)table.Rows[20][4];
                            _PackedItemsDisp_400u_TopFrames_TotalDisp = (double)table.Rows[20][5];


                            //TRUCK MOVEMENT
                            _Truck_Movement_IN = (double)table.Rows[52][2];
                            _Truck_Movement_OUT = (double)table.Rows[53][2];
                            _Truck_Movement_Ratio = Math.Round((double)table.Rows[54][2],1);


                            //STOCK ON HAND
                            _Stock_Pallet_QI = (double)table.Rows[57][2];
                            _Stock_Pallet_UR = (double)table.Rows[57][3];
                            _Stock_Pallet_BL = (double)table.Rows[57][4];

                            _Stock_Composite_QI = (double)table.Rows[58][2];
                            _Stock_Composite_UR = (double)table.Rows[58][3];
                            _Stock_Composite_BL = (double)table.Rows[58][4];

                            _Stock_HollowProfile_QI = (double)table.Rows[59][2];
                            _Stock_HollowProfile_UR = (double)table.Rows[59][3];
                            _Stock_HollowProfile_BL = (double)table.Rows[59][4];


                            _Stock_TopFrames_QI = (double)table.Rows[62][2];
                            _Stock_TopFrames_UR = (double)table.Rows[62][3];
                            _Stock_TopFrames_BL = (double)table.Rows[62][4];

                            


                }

               
            }

            //Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();

            //MAPPING FROM EXCEL TO MODEL ENTITY
            var document = new Document()
            {

                Filename = filename,
                FilePath = "~\\UploadedFiles\\",
                SortCenter = strHEADER_SORTCENTER,
                ReceivedDateFrom = Convert.ToDateTime(strHEADER_REC_DATEFROM),
                PreparedBy = strHEADER_PREPAREDBY,
                DateUploaded = DateTime.Now,
                EmployeeId = EmployeeID,
                //PRODUCTIVITY
                Productivity_100n =  _Productivity_100n,
                Productivity_201u =  _Productivity_201u,
                Productivity_300u =  _Productivity_300u,
                Productivity_400u =  _Productivity_400u,
                //UR SC RATIO
                URSC_BL_Pallet = _URSC_BL_Pallet,
                URSC_BL_Composite = _URSC_BL_Composite,
                URSC_BL_HollowProfile = _URSC_BL_HollowProfile,
                URSC_BL_TopFrames = _URSC_BL_TopFrames,
                URSC_UR_Pallet = _URSC_UR_Pallet,
                URSC_UR_Composite = _URSC_UR_Composite,
                URSC_UR_HollowProfile = _URSC_UR_HollowProfile,
                URSC_UR_TopFrames = _URSC_UR_TopFrames,
                URSC_SC_Pallet =  _URSC_SC_Pallet,
                URSC_SC_Composite = _URSC_SC_Composite,
                URSC_SC_HollowProfile = _URSC_SC_HollowProfile,
                URSC_SC_TopFrames = _URSC_SC_TopFrames,
                //TRUCK MOVEMENT RATIO
                Truck_Movement_IN = _Truck_Movement_IN,
                Truck_Movement_OUT = _Truck_Movement_OUT,
                Truck_Movement_Ratio = _Truck_Movement_Ratio,
                //PACKED ITEM RECEIVED
                PackedItemsRec_100n_Pallet_TotalRec = _PackedItemsRec_100n_Pallet_TotalRec,
                PackedItemsRec_201u_Composite_TotalRec = _PackedItemsRec_201u_Composite_TotalRec,
                PackedItemsRec_300u_BlackHollowProfile_TotalRec = _PackedItemsRec_300u_BlackHollowProfile_TotalRec,
                PackedItemsRec_400u_TopFrames_TotalRec = _PackedItemsRec_400u_TopFrames_TotalRec,
                //PACKED ITEM DISPATCHED
                PackedItemsDisp_100n_Pallet_TotalDisp = _PackedItemsDisp_100n_Pallet_TotalDisp,
                PackedItemsDisp_201u_Composite_TotalDisp = _PackedItemsDisp_201u_Composite_TotalDisp,
                PackedItemsDisp_300u_BlackHollowProfile_TotalDisp = _PackedItemsDisp_300u_BlackHollowProfile_TotalDisp,
                PackedItemsDisp_400u_TopFrames_TotalDisp = _PackedItemsDisp_400u_TopFrames_TotalDisp,
                //STOCK-ON-HAND
                Stock_100n_Pallet_QI = _Stock_Pallet_QI,
                Stock_100n_Pallet_UR = _Stock_Pallet_UR,
                Stock_100n_Pallet_BL = _Stock_Pallet_BL,
                Stock_201u_Composite_QI = _Stock_Composite_QI,
                Stock_201u_Composite_UR = _Stock_Composite_UR,
                Stock_201u_Composite_BL = _Stock_Composite_BL,
                Stock_300u_HollowProfile_QI= _Stock_HollowProfile_QI,
                Stock_300u_HollowProfile_UR = _Stock_HollowProfile_UR,
                Stock_300u_HollowProfile_BL = _Stock_HollowProfile_BL,
                Stock_400u_TopFrame_QI = _Stock_TopFrames_QI,
                Stock_400u_TopFrame_UR =_Stock_TopFrames_UR,
                Stock_400u_TopFrame_BL = _Stock_TopFrames_BL,
                ObjectState = ObjectState.Added,

            };



            _documentRepositoryAsync.GetRepository<Document>().Insert(document);
            _unitOfWorkAsync.SaveChanges();

            return RedirectToAction("UploadForm", "Document");

          
        }
       
	}
}