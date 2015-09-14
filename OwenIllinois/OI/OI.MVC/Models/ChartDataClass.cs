using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OI.MVC.Models
{
  
        public class ChartDataModel
        {
            public string Item { get; set; }
            public double SeriesValue { get; set; }
        }




        public class ChartFilterModel
        {
            public string SortCenter { get; set; }
            public string Item { get; set; }
            public DateTime DateRecFrom { get; set; }
            public DateTime DateRecTo{ get; set; }
        }


}