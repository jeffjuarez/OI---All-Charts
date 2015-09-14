using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Drawing;
using System.Globalization;
using OI.Entities.Models;

using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Point = DotNet.Highcharts.Options.Point;

using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using OI.MVC.Models;


namespace OI.MVC.Controllers
{
    public class ChartDataController : Controller
    {

        private readonly IRepositoryAsync<Document> _documentRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
       
        public ChartDataController(IUnitOfWorkAsync unitOfWorkAsync,
            IRepositoryAsync<Document> documentRepositoryAsync)
        {
            _documentRepositoryAsync = documentRepositoryAsync;
                        _unitOfWorkAsync = unitOfWorkAsync;
        }

     
        [HttpPost]
       public ActionResult Search_Productivity(FormCollection form)
     
            {
                  ChartFilterModel  objChartFilterModel = new ChartFilterModel();
                    
                    objChartFilterModel.SortCenter =  form["SortCenter"].ToString();
                  //  objChartFilterModel.Item = form["ProductivityItems"].ToString();
                    if (form["RecDateFrom"].ToString() != string.Empty && form["RecDateTo"].ToString() != string.Empty)
                    {     
                        objChartFilterModel.DateRecFrom = Convert.ToDateTime(form["RecDateFrom"].ToString());
                        objChartFilterModel.DateRecTo = Convert.ToDateTime(form["RecDateTo"].ToString());
                    }

                    TempData["ChartFilterModel"] = objChartFilterModel;// Assign to Temp Data

                  
                return RedirectToAction("Productivity", "ChartData"); // Reload with Filter
            }



        //Productivity - Chart
        public ActionResult Productivity()
        {


            var _ChartFiltermodel = TempData["ChartFilterModel"] as ChartFilterModel; // Getting the TempData ChartFilter Model

        
            var username = HttpContext.User.Identity.Name;

            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();
            var viewModels = _documentRepositoryAsync
                 .Query(e => e.EmployeeId == employee.Id )
                .Select(d => new DocumentProductivityModel()
                {
                   SortCenter = d.SortCenter,
                   DateRec = d.ReceivedDateFrom,
                   Productivity_100n = d.Productivity_100n,
                   Productivity_201u = d.Productivity_201u,
                   Productivity_300u = d.Productivity_300u,
                   Productivity_400u = d.Productivity_400u,
                })
                .ToList();

            // APPLY FILTERING
     
            if (_ChartFiltermodel != null)
              {
                   if(_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                 {
                       ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center & Received From : " + _ChartFiltermodel.DateRecFrom.ToShortDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToShortDateString() + " ) ";
                       viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter && x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); } 
                 else if( _ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom == Convert.ToDateTime("01/01/0001"))
                 {
                     ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center Data ) ";
            
                      viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter).ToList();// SORT CENTER ONLY  
                 }

                   else if (_ChartFiltermodel.SortCenter == "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                   {
                       ViewBag.ReportParam = "  Filter: ( Data from all Sort Centers  & Received From : " + _ChartFiltermodel.DateRecFrom.ToLongDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToLongDateString() + " ) ";

                       viewModels = viewModels.Where(x => x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); // DATE RECEIVED FROM ONLY
                   }
             }


            string[] _strDateRec = new string[viewModels.Count];

            object[] _objDateRec = new object[viewModels.Count];
            object[] _obj100n = new object[viewModels.Count];
            object[] _obj201u = new object[viewModels.Count];
            object[] _obj300u = new object[viewModels.Count];
            object[] _obj400u = new object[viewModels.Count];

            int i = 0;

            foreach (DocumentProductivityModel item in viewModels)
            {
                
                _objDateRec.SetValue(item.DateRec.ToString("d MMM"), i);
                _obj100n.SetValue(item.Productivity_100n, i);
                _obj201u.SetValue(item.Productivity_201u, i);
                _obj300u.SetValue(item.Productivity_300u, i);
                _obj400u.SetValue(item.Productivity_400u, i);

                _strDateRec[i] = string.Format(item.DateRec.ToString("d MMM"), i);
                
                i++;
            }

        

            Highcharts chart = new Highcharts("chart")
                //.InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .InitChart(new Chart
                 {
                     Type = ChartTypes.Column,
                     Margin = new[] { 80 },
                     Options3d = new ChartOptions3d
                     {
                         Enabled = true,
                         Alpha = 10,
                         Beta = 0,
                         Depth = 75
                     }
                 })

                .SetTitle(new Title { Text = "OI - Productivity " })
                //.SetSubtitle(new Subtitle { Text = "Source: Jeff@Drake.Com" })
             
                .SetXAxis(new XAxis { Categories = _strDateRec }) // The Date Received
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Items Sorted per Hour" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 50,
                    Y = 10,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +' '; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.1,
                        BorderWidth = 0
                        
                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "100n",  Data = new Data(_obj100n) },
                    new Series { Name = "201u",  Data = new Data(_obj201u) },
                    new Series { Name = "300u",  Data = new Data(_obj300u) },
                    new Series { Name = "400u",  Data = new Data(_obj400u) }// Added Series


                });

            return View(chart);


        }


        //SEARCHING UR/SC RATIO
        [HttpPost]
        public ActionResult Search_URSC(FormCollection form)
        {
            ChartFilterModel objChartFilterModel = new ChartFilterModel();

            objChartFilterModel.SortCenter = form["SortCenter"].ToString();
            objChartFilterModel.Item = form["URSCItem"].ToString();
            if (form["RecDateFrom"].ToString() != string.Empty && form["RecDateTo"].ToString() != string.Empty)
            {
                objChartFilterModel.DateRecFrom = Convert.ToDateTime(form["RecDateFrom"].ToString());
                objChartFilterModel.DateRecTo = Convert.ToDateTime(form["RecDateTo"].ToString());
            }

            TempData["ChartFilterModel"] = objChartFilterModel;// Assign to Temp Data

            return RedirectToAction("URSC", "ChartData"); // Reload with Filter
        }



        //UR/SC  - Chart
        public ActionResult URSC()
        {


            var _ChartFiltermodel = TempData["ChartFilterModel"] as ChartFilterModel; // Getting the TempData ChartFilter Model


            var username = HttpContext.User.Identity.Name;

            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();
            var viewModels = _documentRepositoryAsync
                 .Query(e => e.EmployeeId == employee.Id)
                .Select(d => new DocumentURSCModel()
                {
                    SortCenter = d.SortCenter,
                    DateRec = d.ReceivedDateFrom,
                    URSC_BL_Pallet = d.URSC_BL_Pallet,
                    URSC_BL_Composite = d.URSC_BL_Composite,
                    URSC_BL_HollowProfile = d.URSC_BL_HollowProfile,
                    URSC_BL_TopFrames = d.URSC_BL_TopFrames,
                    URSC_UR_Pallet = d.URSC_UR_Pallet,
                    URSC_UR_Composite = d.URSC_UR_Composite,
                    URSC_UR_HollowProfile = d.URSC_UR_HollowProfile,
                    URSC_UR_TopFrames = d.URSC_UR_TopFrames,
                    URSC_SC_Pallet = d.URSC_SC_Pallet,
                    URSC_SC_Composite = d.URSC_SC_Composite,
                    URSC_SC_HollowProfile = d.URSC_SC_HollowProfile,
                    URSC_SC_TopFrames = d.URSC_SC_TopFrames,
                })
                .ToList();

            // APPLY FILTERING

            if (_ChartFiltermodel != null)
            {
                if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center & Received From : " + _ChartFiltermodel.DateRecFrom.ToShortDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToShortDateString() + " ) ";
                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter && x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList();
                }
                else if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom == Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center Data ) ";

                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter).ToList();// SORT CENTER ONLY  
                }

                else if (_ChartFiltermodel.SortCenter == "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( Data from all Sort Centers  & Received From : " + _ChartFiltermodel.DateRecFrom.ToLongDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToLongDateString() + " ) ";

                    viewModels = viewModels.Where(x => x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); // DATE RECEIVED FROM ONLY
                }
            }

            else
            {
                ChartFilterModel _URSCFilter = new ChartFilterModel();
                _URSCFilter.SortCenter = "DrakeWestEnd";
                _URSCFilter.Item = "Pallete";// Default to Pallete

                viewModels = viewModels.Where(x => x.SortCenter == _URSCFilter.SortCenter).ToList();
                _ChartFiltermodel = _URSCFilter;
            }


            string[] _strDateRec = new string[viewModels.Count];
            object[] _objDateRec = new object[viewModels.Count];
            
            //==PALETTE==
            object[] _objBL_Pallete = new object[viewModels.Count];
            object[] _objUR_Pallete = new object[viewModels.Count];
            object[] _objSC_Pallete = new object[viewModels.Count];

            //==COMPOSITE==
            object[] _objBL_Composite = new object[viewModels.Count];
            object[] _objUR_Composite = new object[viewModels.Count];
            object[] _objSC_Composite = new object[viewModels.Count];

            //==HOLLOW PROFILE==
            object[] _objBL_HollowProfile = new object[viewModels.Count];
            object[] _objUR_HollowProfile = new object[viewModels.Count];
            object[] _objSC_HollowProfile = new object[viewModels.Count];

            //==TOP FRAMES==
             object[] _objBL_TopFrames = new object[viewModels.Count];
             object[] _objUR_TopFrames = new object[viewModels.Count];
             object[] _objSC_TopFrames = new object[viewModels.Count];

             object[] _objBL_ = new object[viewModels.Count];
             object[] _objSC_ = new object[viewModels.Count];
             object[] _objUR_ = new object[viewModels.Count];



            int i = 0;

            foreach (DocumentURSCModel item in viewModels)
            {

                _objDateRec.SetValue(item.DateRec.ToString("d MMM"), i);

             
                if (_ChartFiltermodel.Item == "Pallete")
                {
                    _objBL_.SetValue(item.URSC_BL_Pallet,i);
                    _objUR_.SetValue(item.URSC_UR_Pallet, i);
                    _objSC_.SetValue(item.URSC_SC_Pallet, i);
                }
                if (_ChartFiltermodel.Item == "Composite") 
                {
                    _objBL_.SetValue(item.URSC_BL_Composite, i);
                    _objUR_.SetValue(item.URSC_UR_Composite, i);
                    _objSC_.SetValue(item.URSC_SC_Composite, i);

                }
                if (_ChartFiltermodel.Item == "HollowProfile")
                { 
                    _objBL_.SetValue(item.URSC_BL_HollowProfile, i);
                    _objUR_.SetValue(item.URSC_UR_HollowProfile, i);
                    _objSC_.SetValue(item.URSC_SC_HollowProfile, i);
           
                }
                if (_ChartFiltermodel.Item == "TopFrames") 
                { 
                    _objBL_.SetValue(item.URSC_BL_TopFrames, i);
                    _objUR_.SetValue(item.URSC_UR_TopFrames, i);
                    _objSC_.SetValue(item.URSC_SC_TopFrames, i);
           
                }
              


                _strDateRec[i] = string.Format(item.DateRec.ToString("d MMM"), i);

                i++;
            }



            Highcharts chart = new Highcharts("chart")
                //.InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .InitChart(new Chart
                 {
                     Type = ChartTypes.Column,
                     Margin = new[] { 80 },
                     Options3d = new ChartOptions3d
                     {
                         Enabled = true,
                         Alpha = 10,
                         Beta = 0,
                         Depth = 75
                     }
                 })

                .SetTitle(new Title { Text = "OI - URSC/ Ratio " })
                //.SetSubtitle(new Subtitle { Text = "Source: Jeff@Drake.Com" })

                .SetXAxis(new XAxis { Categories = _strDateRec }) // The Date Received
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Items Sorted by Percentage" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 50,
                    Y = 10,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +' %'; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.1,
                        BorderWidth = 0

                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "BL/QI",  Data = new Data(_objBL_) },
                    new Series { Name = "UR/QI",  Data = new Data(_objUR_) },
                    new Series { Name = "SC/QI",  Data = new Data(_objSC_) },


                });

            return View(chart);


        }




        //SEARCHING TRUCK MOVEMEBT
        [HttpPost]
        public ActionResult Search_TruckMovement(FormCollection form)
        {
            ChartFilterModel objChartFilterModel = new ChartFilterModel();

            objChartFilterModel.SortCenter = form["SortCenter"].ToString();
            if (form["RecDateFrom"].ToString() != string.Empty && form["RecDateTo"].ToString() != string.Empty)
            {
                objChartFilterModel.DateRecFrom = Convert.ToDateTime(form["RecDateFrom"].ToString());
                objChartFilterModel.DateRecTo = Convert.ToDateTime(form["RecDateTo"].ToString());
            }

            TempData["ChartFilterModel"] = objChartFilterModel;// Assign to Temp Data

            return RedirectToAction("TruckMovement", "ChartData"); // Reload with Filter
        }



        //TRUCK MOVEMEBT  - Chart
        public ActionResult TruckMovement()
        {


            var _ChartFiltermodel = TempData["ChartFilterModel"] as ChartFilterModel; // Getting the TempData ChartFilter Model


            var username = HttpContext.User.Identity.Name;

            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();
            var viewModels = _documentRepositoryAsync
                 .Query(e => e.EmployeeId == employee.Id)
                .Select(d => new DocumentTruckMovementModel()
                {
                    SortCenter = d.SortCenter,
                    DateRec = d.ReceivedDateFrom,
                    Truck_Movement_Ratio = d.Truck_Movement_Ratio,
                })
                .ToList();

            // APPLY FILTERING

            if (_ChartFiltermodel != null)
            {
                if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center & Received From : " + _ChartFiltermodel.DateRecFrom.ToShortDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToShortDateString() + " ) ";
                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter && x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList();
                }
                else if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom == Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center Data ) ";

                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter).ToList();// SORT CENTER ONLY  
                }

                else if (_ChartFiltermodel.SortCenter == "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( Data from all Sort Centers  & Received From : " + _ChartFiltermodel.DateRecFrom.ToLongDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToLongDateString() + " ) ";

                    viewModels = viewModels.Where(x => x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); // DATE RECEIVED FROM ONLY
                }
            }

            

            string[] _strDateRec = new string[viewModels.Count];
            object[] _objDateRec = new object[viewModels.Count];

            //==TruckMovement Ratio==
            object[] _objTruckMovement = new object[viewModels.Count];
           


            int i = 0;

            foreach (DocumentTruckMovementModel item in viewModels)
            {

                _objDateRec.SetValue(item.DateRec.ToString("d MMM"), i);

                    _objTruckMovement.SetValue(item.Truck_Movement_Ratio, i);

                _strDateRec[i] = string.Format(item.DateRec.ToString("d MMM"), i);

                i++;
            }



            Highcharts chart = new Highcharts("chart")
                //.InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .InitChart(new Chart
                 {
                     Type = ChartTypes.Column,
                     Margin = new[] { 80 },
                     Options3d = new ChartOptions3d
                     {
                         Enabled = true,
                         Alpha = 10,
                         Beta = 0,
                         Depth = 75
                     }
                 })

                .SetTitle(new Title { Text = "OI - Truck Movement Ratio " })
                //.SetSubtitle(new Subtitle { Text = "Source: Jeff@Drake.Com" })

                .SetXAxis(new XAxis { Categories = _strDateRec }) // The Date Received
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Items Sorted by Percentage" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 50,
                    Y = 10,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +''; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.1,
                        BorderWidth = 0

                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "Truck Movement Rate",  Data = new Data(_objTruckMovement) },
               


                });

            return View(chart);


        }




        //SEARCHING RECEIVED AND DISPATCHED
        [HttpPost]
        public ActionResult Search_RecDisp(FormCollection form)
        {
            ChartFilterModel objChartFilterModel = new ChartFilterModel();

            objChartFilterModel.SortCenter = form["SortCenter"].ToString();
       //    objChartFilterModel.Item = form["URSCItem"].ToString();
            if (form["RecDateFrom"].ToString() != string.Empty && form["RecDateTo"].ToString() != string.Empty)
            {
                objChartFilterModel.DateRecFrom = Convert.ToDateTime(form["RecDateFrom"].ToString());
                objChartFilterModel.DateRecTo = Convert.ToDateTime(form["RecDateTo"].ToString());
            }

            TempData["ChartFilterModel"] = objChartFilterModel;// Assign to Temp Data

            return RedirectToAction("RecDisp", "ChartData"); // Reload with Filter
        }



        //UR/SC  - Chart
        public ActionResult RecDisp()
        {


            var _ChartFiltermodel = TempData["ChartFilterModel"] as ChartFilterModel; // Getting the TempData ChartFilter Model


            var username = HttpContext.User.Identity.Name;

            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();
            var viewModels = _documentRepositoryAsync
                 .Query(e => e.EmployeeId == employee.Id)
                .Select(d => new DocumentRecDispModel()
                {
                    SortCenter = d.SortCenter,
                    DateRec = d.ReceivedDateFrom,
                    RecDisp_100nRec_Pallet = d.PackedItemsRec_100n_Pallet_TotalRec,
                    RecDisp_201uRec_Composite = d.PackedItemsRec_201u_Composite_TotalRec,
                    RecDisp_300uRec_HollowProfile = d.PackedItemsRec_300u_BlackHollowProfile_TotalRec,
                    RecDisp_400uRec_TopFrames = d.PackedItemsRec_400u_TopFrames_TotalRec,
                    RecDisp_100nDisp_Pallet = d.PackedItemsDisp_100n_Pallet_TotalDisp,
                    RecDisp_201uDisp_Composite = d.PackedItemsDisp_201u_Composite_TotalDisp,
                    RecDisp_300uDisp_HollowProfile = d.PackedItemsDisp_300u_BlackHollowProfile_TotalDisp,
                    RecDisp_400uDisp_TopFrames = d.PackedItemsDisp_400u_TopFrames_TotalDisp,
                   
                })
                .ToList();

            // APPLY FILTERING

            if (_ChartFiltermodel != null)
            {
                if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center & Received From : " + _ChartFiltermodel.DateRecFrom.ToShortDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToShortDateString() + " ) ";
                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter && x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList();
                }
                else if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom == Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center Data ) ";

                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter).ToList();// SORT CENTER ONLY  
                }

                else if (_ChartFiltermodel.SortCenter == "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( Data from all Sort Centers  & Received From : " + _ChartFiltermodel.DateRecFrom.ToLongDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToLongDateString() + " ) ";

                    viewModels = viewModels.Where(x => x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); // DATE RECEIVED FROM ONLY
                }
            }

            else
            {
                ChartFilterModel _RecDispFilter = new ChartFilterModel();
                _RecDispFilter.SortCenter = "DrakeWestEnd";
            
                viewModels = viewModels.Where(x => x.SortCenter == _RecDispFilter.SortCenter).ToList();
                _ChartFiltermodel = _RecDispFilter;
            }


            string[] _strDateRec = new string[viewModels.Count];
            object[] _objDateRec = new object[viewModels.Count];

            object[] _objRec = new object[4];
            object[] _objDisp = new object[4];



            int i = 0;

            foreach (DocumentRecDispModel item in viewModels)
            {

             //   _objDateRec.SetValue(item.DateRec.ToString("d MMM"), i);

                _objRec.SetValue(item.RecDisp_100nRec_Pallet, 0);
                _objDisp.SetValue(item.RecDisp_100nDisp_Pallet, 0);

                _objRec.SetValue(item.RecDisp_201uRec_Composite, 1);
                _objDisp.SetValue(item.RecDisp_201uDisp_Composite,1);

                _objRec.SetValue(item.RecDisp_300uRec_HollowProfile, 2);
                _objDisp.SetValue(item.RecDisp_300uDisp_HollowProfile,2);

                _objRec.SetValue(item.RecDisp_300uRec_HollowProfile, 3);
                _objDisp.SetValue(item.RecDisp_300uDisp_HollowProfile, 3);

            
                _strDateRec[i] = string.Format(item.DateRec.ToString("d MMM"), i);

                i++;
            }



            Highcharts chart = new Highcharts("chart")
                //.InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                 .InitChart(new Chart
                 {
                     Type = ChartTypes.Column,
                     Margin = new[] { 80 },
                     Options3d = new ChartOptions3d
                     {
                         Enabled = true,
                         Alpha = 10,
                         Beta = 0,
                         Depth = 75
                     }
                 })

                .SetTitle(new Title { Text = "OI - Packed Item Received / Dispatched " })
                //.SetSubtitle(new Subtitle { Text = "Source: Jeff@Drake.Com" })

                .SetXAxis(new XAxis { Categories = new [] {"100n","201u","300u","400u"} }) // The Date Received
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Items Sorted by Percentage" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 50,
                    Y = 10,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +' '; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.1,
                        BorderWidth = 0

                    }
                })
                .SetSeries(new[]
                {
                    new Series { Name = "Packed Item Received",  Data = new Data(_objRec) },
                    new Series { Name = "Packed Item Dispatched",  Data = new Data(_objDisp) },

                });

            return View(chart);


        }




        [HttpPost]
        public ActionResult Search_StockonHand(FormCollection form)
        {
            ChartFilterModel objChartFilterModel = new ChartFilterModel();

            objChartFilterModel.SortCenter = form["SortCenter"].ToString();
            //  objChartFilterModel.Item = form["ProductivityItems"].ToString();
            if (form["RecDateFrom"].ToString() != string.Empty && form["RecDateTo"].ToString() != string.Empty)
            {
                objChartFilterModel.DateRecFrom = Convert.ToDateTime(form["RecDateFrom"].ToString());
                objChartFilterModel.DateRecTo = Convert.ToDateTime(form["RecDateTo"].ToString());
            }

            TempData["ChartFilterModel"] = objChartFilterModel;// Assign to Temp Data


            return RedirectToAction("StockonHand", "ChartData"); // Reload with Filter
        }



        //Productivity - Chart
        public ActionResult StockonHand()
        {


            var _ChartFiltermodel = TempData["ChartFilterModel"] as ChartFilterModel; // Getting the TempData ChartFilter Model


            var username = HttpContext.User.Identity.Name;

            var employee = _documentRepositoryAsync
                .GetRepository<Account>()
                .Query(a => a.Username == username)
                .Select(e => e.Employee)
                .SingleOrDefault();

            if (employee == null) return View();
            var viewModels = _documentRepositoryAsync
                 .Query(e => e.EmployeeId == employee.Id)
                .Select(d => new DocumentStockModel()
                {
                    SortCenter = d.SortCenter,
                    DateRec = d.ReceivedDateFrom,
                    Stock_100n_Palette_QI = d.Stock_100n_Pallet_QI,
                    Stock_100n_Palette_UR = d.Stock_100n_Pallet_UR,
                    Stock_100n_Palette_BL  = d.Stock_100n_Pallet_BL,
                    Stock_201u_Composite_QI = d.Stock_201u_Composite_QI,
                    Stock_201u_Composite_UR = d.Stock_201u_Composite_UR,
                    Stock_201u_Composite_BL = d.Stock_201u_Composite_BL,
                    Stock_300u_HollowProfile_QI = d.Stock_300u_HollowProfile_QI,
                    Stock_300u_HollowProfile_UR = d.Stock_300u_HollowProfile_UR,
                    Stock_300u_HollowProfile_BL = d.Stock_300u_HollowProfile_BL,
                    Stock_400u_TopFrames_QI = d.Stock_400u_TopFrame_QI,
                    Stock_400u_TopFrames_UR = d.Stock_400u_TopFrame_UR,
                    Stock_400u_TopFrames_BL = d.Stock_400u_TopFrame_BL,

                })
                .ToList();

            // APPLY FILTERING

            if (_ChartFiltermodel != null)
            {
                if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center & Received From : " + _ChartFiltermodel.DateRecFrom.ToShortDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToShortDateString() + " ) ";
                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter && x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList();
                }
                else if (_ChartFiltermodel.SortCenter != "All" && _ChartFiltermodel.DateRecFrom == Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( " + _ChartFiltermodel.SortCenter + " Sort Center Data ) ";

                    viewModels = viewModels.Where(x => x.SortCenter == _ChartFiltermodel.SortCenter).ToList();// SORT CENTER ONLY  
                }

                else if (_ChartFiltermodel.SortCenter == "All" && _ChartFiltermodel.DateRecFrom != Convert.ToDateTime("01/01/0001"))
                {
                    ViewBag.ReportParam = "  Filter: ( Data from all Sort Centers  & Received From : " + _ChartFiltermodel.DateRecFrom.ToLongDateString() + "  to  " + _ChartFiltermodel.DateRecTo.ToLongDateString() + " ) ";

                    viewModels = viewModels.Where(x => x.DateRec >= _ChartFiltermodel.DateRecFrom && x.DateRec <= _ChartFiltermodel.DateRecTo).ToList(); // DATE RECEIVED FROM ONLY
                }
            }


            string[] _strDateRec = new string[viewModels.Count];

           object[] _objDateRec = new object[viewModels.Count];
         
            double _Palette_QI = 0;
            double _Palette_UR = 0;
            double _Palette_BL =0;
            double _Composite_QI = 0;
            double _Composite_UR = 0;
            double _Composite_BL = 0;
            double _HProfile_QI = 0;
            double _HProfile_UR = 0;
            double _HProfile_BL = 0;
            double _TopFrames_QI = 0;
            double _TopFrames_UR = 0;
            double _TopFrames_BL = 0;


            double _TotalPalette = 0;
            double _TotalComposite = 0;
            double _TotalHProfile = 0;
            double _TotalTopFrames = 0;

            int i = 0;

            foreach (DocumentStockModel item in viewModels)
            {
                
                _TotalPalette = item.Stock_100n_Palette_QI + item.Stock_100n_Palette_UR + item.Stock_100n_Palette_BL;
                _TotalComposite = item.Stock_201u_Composite_QI + item.Stock_201u_Composite_UR + item.Stock_201u_Composite_BL;
                _TotalHProfile = item.Stock_300u_HollowProfile_QI + item.Stock_300u_HollowProfile_UR + item.Stock_300u_HollowProfile_BL;
                _TotalTopFrames = item.Stock_400u_TopFrames_QI + item.Stock_400u_TopFrames_UR + item.Stock_400u_TopFrames_BL;



                _objDateRec.SetValue(item.DateRec.ToString("d MMM"), i);

                //PALETTE
                _Palette_QI = (_TotalPalette / item.Stock_100n_Palette_QI) * 100;
                _Palette_UR = (_TotalPalette / item.Stock_100n_Palette_UR) * 100;
                _Palette_BL = (_TotalPalette / item.Stock_100n_Palette_BL) * 100;

               // COMPOSITE
                _Composite_QI = (_TotalComposite / item.Stock_201u_Composite_QI) * 100;
                _Composite_UR = (_TotalComposite / item.Stock_201u_Composite_UR) * 100;
                _Composite_BL = (_TotalComposite / item.Stock_201u_Composite_BL) * 100;
               
                //HOLLOW PROFILE
                _HProfile_QI = (_TotalHProfile / item.Stock_300u_HollowProfile_QI) * 100;
                _HProfile_UR = (_TotalHProfile / item.Stock_300u_HollowProfile_UR) * 100;
                _HProfile_BL = (_TotalHProfile / item.Stock_300u_HollowProfile_BL) * 100;
              
                //TOP FRAMES
                _TopFrames_QI = (_TotalTopFrames / item.Stock_400u_TopFrames_QI) * 100;
                _TopFrames_UR = (_TotalTopFrames / item.Stock_400u_TopFrames_UR) * 100;
                _TopFrames_BL = (_TotalTopFrames / item.Stock_400u_TopFrames_BL) * 100;
              


             //   _objPallete_QI.SetValue(item.Stock_201u_Composite_UR / (item.Stock_201u_Composite_QI+ item.Stock_201u_Composite_UR + item.Stock_201u_Composite_BL) * 100, i);
               // _objPallete_BL.SetValue(item.Stock_300u_HollowProfile_QI / (item.Stock_100n_Palette_QI + item.Stock_100n_Palette_UR + item.Stock_100n_Palette_BL) * 100, i);
             
                _strDateRec[i] = string.Format(item.DateRec.ToString("d MMM"), i);

                i++;
            }



            Highcharts chartPalette = new Highcharts("chartPalette")
                .InitChart(new Chart
                {
                    Type = ChartTypes.Pie,
                    MarginTop = 80,
                    MarginRight = 40,
                    Options3d = new ChartOptions3d
                    {
                        Enabled = true,
                        Alpha = 45,
                        Beta = 0
                    }
                })
                .SetTitle(new Title { Text = "Palette" })
                .SetTooltip(new Tooltip { PointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" })
                 .SetLegend(new Legend
                 {
                     Layout = Layouts.Vertical,
                     Align = HorizontalAligns.Left,
                     VerticalAlign = VerticalAligns.Top,
                     X = 50,
                     Y = 10,
                     Floating = true,
                     BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                     Shadow = true
                 })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        Depth = 35,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = true,
                            Format = "{point.name}"
                        }
                    }
                })
               
                 .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Palette",
                    Data = new Data(new object[]
                    {
                        new object[] { "QI", _Palette_QI },
                        new object[] { "UR",_Palette_UR },
                        new object[] { "BL", _Palette_BL}
                    })
                });

            Highcharts chartComposite = new Highcharts("chartComposite")
              .InitChart(new Chart
              {
                  Type = ChartTypes.Pie,
                  MarginTop = 80,
                  MarginRight = 40,
                  Options3d = new ChartOptions3d
                  {
                      Enabled = true,
                      Alpha = 45,
                      Beta = 0
                  }
              })
              .SetTitle(new Title { Text = "Composite" })
              .SetTooltip(new Tooltip { PointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" })
               .SetLegend(new Legend
               {
                   Layout = Layouts.Vertical,
                   Align = HorizontalAligns.Left,
                   VerticalAlign = VerticalAligns.Top,
                   X = 50,
                   Y = 10,
                   Floating = true,
                   BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                   Shadow = true
               })
              .SetPlotOptions(new PlotOptions
              {
                  Pie = new PlotOptionsPie
                  {
                      AllowPointSelect = true,
                      Cursor = Cursors.Pointer,
                      Depth = 35,
                      DataLabels = new PlotOptionsPieDataLabels
                      {
                          Enabled = true,
                          Format = "{point.name}"
                      }
                  }
              })

               .SetSeries(new Series
               {
                   Type = ChartTypes.Pie,
                   Name = "Composite",
                   Data = new Data(new object[]
                    {
                        new object[] { "QI", _Composite_QI },
                        new object[] { "UR",_Composite_UR },
                        new object[] { "BL", _Composite_BL}
                    })
               });


            Highcharts chartHProfile = new Highcharts("chartHProfile")
              .InitChart(new Chart
              {
                  Type = ChartTypes.Pie,
                  MarginTop = 80,
                  MarginRight = 40,
                  Options3d = new ChartOptions3d
                  {
                      Enabled = true,
                      Alpha = 45,
                      Beta = 0
                  }
              })
              .SetTitle(new Title { Text = "Hollow Profile" })
              .SetTooltip(new Tooltip { PointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" })
               .SetLegend(new Legend
               {
                   Layout = Layouts.Vertical,
                   Align = HorizontalAligns.Left,
                   VerticalAlign = VerticalAligns.Top,
                   X = 50,
                   Y = 10,
                   Floating = true,
                   BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                   Shadow = true
               })
              .SetPlotOptions(new PlotOptions
              {
                  Pie = new PlotOptionsPie
                  {
                      AllowPointSelect = true,
                      Cursor = Cursors.Pointer,
                      Depth = 35,
                      DataLabels = new PlotOptionsPieDataLabels
                      {
                          Enabled = true,
                          Format = "{point.name}"
                      }
                  }
              })

               .SetSeries(new Series
               {
                   Type = ChartTypes.Pie,
                   Name = "Hollow Profile",
                   Data = new Data(new object[]
                    {
                        new object[] { "QI", _HProfile_QI },
                        new object[] { "UR",_HProfile_UR },
                        new object[] { "BL", _HProfile_BL}
                    })
               });

            Highcharts chartTopFrames = new Highcharts("chartTopFrames")
              .InitChart(new Chart
              {
                  Type = ChartTypes.Pie,
                  MarginTop = 80,
                  MarginRight = 40,
                  Options3d = new ChartOptions3d
                  {
                      Enabled = true,
                      Alpha = 45,
                      Beta = 0
                  }
              })
              .SetTitle(new Title { Text = "Top Frames" })
              .SetTooltip(new Tooltip { PointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" })
               .SetLegend(new Legend
               {
                   Layout = Layouts.Vertical,
                   Align = HorizontalAligns.Left,
                   VerticalAlign = VerticalAligns.Top,
                   X = 50,
                   Y = 10,
                   Floating = true,
                   BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                   Shadow = true
               })
              .SetPlotOptions(new PlotOptions
              {
                  Pie = new PlotOptionsPie
                  {
                      AllowPointSelect = true,
                      Cursor = Cursors.Pointer,
                      Depth = 35,
                      DataLabels = new PlotOptionsPieDataLabels
                      {
                          Enabled = true,
                          Format = "{point.name}"
                      }
                  }
              })

               .SetSeries(new Series
               {
                   Type = ChartTypes.Pie,
                   Name = "Top Frames",
                   Data = new Data(new object[]
                    {
                        new object[] { "QI", _TopFrames_QI },
                        new object[] { "UR",_TopFrames_UR },
                        new object[] { "BL", _TopFrames_BL}
                    })
               });




            return View(new Container(new[] { chartPalette, chartComposite ,chartHProfile,chartTopFrames}));

       //     return View(chart);


        }






    }
}
