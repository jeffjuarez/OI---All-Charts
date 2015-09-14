using System.Web.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.Helpers;
namespace OI.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {


        public ActionResult CreateBar()
        {
            //Create bar chart
            var chart = new Chart(width: 300, height: 200)
            .AddSeries(chartType: "column",
                            xValue: new[] { "10 ", "50", "30 ", "70" },
                            yValues: new[] { "50", "70", "90", "110" })
                            .GetBytes("png");
            return File(chart, "image/bytes");
        }

        public ActionResult CreatePie()
        {
            //Create bar chart
            var chart = new Chart(width: 300, height: 200)
            .AddSeries(chartType: "pie",
                            xValue: new[] { "10 ", "50", "30 ", "70" },
                            yValues: new[] { "50", "70", "90", "110" })
                            .GetBytes("png");
            return File(chart, "image/bytes");
        }

        public ActionResult CreateLine()
        {
            //Create bar chart
            var chart = new Chart(width: 600, height: 200)
            .AddSeries(chartType: "line",
                            xValue: new[] { "10 ", "50", "30 ", "70" },
                            yValues: new[] { "50", "70", "90", "110" })
                            .GetBytes("png");
            return File(chart, "image/bytes");
        }

        //public ActionResult Index()
        //{
        //    return View("Index");
        //}


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Chart()
        {

            ViewBag.Message = "Charting Here";

            return View();
        }


        public ActionResult About()
        {
            FormsAuthentication.SignOut();
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult AboutOI()
        {
          
            ViewBag.Message = "Your application description page.";

            return View();
        }

     


        public ActionResult Contact()
        {
            FormsAuthentication.SignOut();
            ViewBag.Message = "Your contact page.";

            return View();
        }












    }
}