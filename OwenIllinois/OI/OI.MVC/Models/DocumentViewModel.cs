using System.ComponentModel.DataAnnotations;
using System;
namespace OI.MVC.Models
{
    public class DocumentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Filename")]
        public string Filename { get; set; }

        [Display(Name = "SortCenter")]
        public string SortCenter { get; set; }

        [Display(Name = "ReceivedDateFrom")]
        public DateTime ReceivedDateFrom { get; set; }

        [Display(Name = "PreparedBy")]
        public string PreparedBy { get; set; }

        [Display(Name = "UserUploaded")]
        public string UserUploaded { get; set; }


        [Display(Name = "DateUploaded")]
        public DateTime DateUploaded { get; set; }

        [Display(Name = "EmployeeId")]
        public int EmployeeId { get; set; }


    }

      //PRODUCTIVITY
      //**100n**////**201u**////**300u**////**400u**//
      
     public class DocumentProductivityModel
    {
        public int Id { get; set; }

        [Display(Name = "SortCenter")]
        public string SortCenter { get; set; }


        [Display(Name = "DateRec")]
        public DateTime DateRec { get; set; }

        [Display(Name = "100n")]
        public double Productivity_100n { get; set; }
        
        [Display(Name = "201u")]
        public double Productivity_201u { get; set; }

        [Display(Name = "300u")]
        public double Productivity_300u { get; set; }

        [Display(Name = "400u")]
        public double Productivity_400u { get; set; }

         
    }

     //UR SC 
     //**BL**////**UR**////**SC//

     public class DocumentURSCModel
     {
         public int Id { get; set; }

         [Display(Name = "SortCenter")]
         public string SortCenter { get; set; }
         [Display(Name = "DateRec")]
         public DateTime DateRec { get; set; }


         //-Palette-\\
         
         [Display(Name = "URSC_BL_Pallet")]
         public double URSC_BL_Pallet { get; set; }

         [Display(Name = "URSC_UR_Pallet")]
         public double URSC_UR_Pallet { get; set; }

         [Display(Name = "URSC_SC_Pallet")]
         public double URSC_SC_Pallet { get; set; }


         //-Composite-\\
     
         [Display(Name = "URSC_BL_Composite")]
         public double URSC_BL_Composite { get; set; }

         [Display(Name = "URSC_UR_Composite")]
         public double URSC_UR_Composite { get; set; }

         [Display(Name = "URSC_SC_Composite")]
         public double URSC_SC_Composite { get; set; }

         //-HollowProfile-\\
     
         [Display(Name = "URSC_BL_HollowProfile")]
         public double URSC_BL_HollowProfile { get; set; }

         [Display(Name = "URSC_UR_HollowProfile")]
         public double URSC_UR_HollowProfile { get; set; }

         [Display(Name = "URSC_SC_HollowProfile")]
         public double URSC_SC_HollowProfile { get; set; }


         //-TopFrames-\\

         [Display(Name = "URSC_BL_TopFrames")]
         public double URSC_BL_TopFrames { get; set; }

         [Display(Name = "URSC_UR_TopFrames")]
         public double URSC_UR_TopFrames { get; set; }

         [Display(Name = "URSC_SC_TopFrames")]
         public double URSC_SC_TopFrames { get; set; }

     }

     //TRUCK MOVEMENT RATE
     public class DocumentTruckMovementModel
     {
         public int Id { get; set; }

         [Display(Name = "SortCenter")]
         public string SortCenter { get; set; }
         [Display(Name = "DateRec")]
         public DateTime DateRec { get; set; }


         //-Palette-\\

         [Display(Name = "Truck_Movement_Ratio")]
         public double Truck_Movement_Ratio { get; set; }

      

     }



     //RECEIVED and DISPATCHED
     //**Pallete**////**Composite*////**HollowProfile**// TopFrames//

     public class DocumentRecDispModel
     {
         public int Id { get; set; }

         [Display(Name = "SortCenter")]
         public string SortCenter { get; set; }
         [Display(Name = "DateRec")]
         public DateTime DateRec { get; set; }


         //-Received-\\

         [Display(Name = "RecDisp_100nRec_Pallet")]
         public double RecDisp_100nRec_Pallet { get; set; }

         [Display(Name = "RecDisp_201uRec_Composite")]
         public double RecDisp_201uRec_Composite { get; set; }

         [Display(Name = "RecDisp_300uRec_HollowProfile")]
         public double RecDisp_300uRec_HollowProfile { get; set; }

         [Display(Name = "RecDisp_400uRec_TopFrames")]
         public double RecDisp_400uRec_TopFrames { get; set; }

         //-Dispatched-\\

         [Display(Name = "RecDisp_100nDisp_Pallet")]
         public double RecDisp_100nDisp_Pallet { get; set; }

         [Display(Name = "RecDisp_201uDisp_Composite")]
         public double RecDisp_201uDisp_Composite { get; set; }

         [Display(Name = "RecDisp_300uDisp_HollowProfile")]
         public double RecDisp_300uDisp_HollowProfile { get; set; }

         [Display(Name = "RecDisp_400uDisp_TopFrames")]
         public double RecDisp_400uDisp_TopFrames { get; set; }

     }



     public class DocumentStockModel
     {
         public int Id { get; set; }

         [Display(Name = "SortCenter")]
         public string SortCenter { get; set; }
         [Display(Name = "DateRec")]
         public DateTime DateRec { get; set; }


         //-100n / Palette-\\

         [Display(Name = "Stock_100n_Palette_QI")]
         public double Stock_100n_Palette_QI { get; set; }

         [Display(Name = "Stock_100n_Palette_UR")]
         public double Stock_100n_Palette_UR { get; set; }

         [Display(Name = "Stock_100n_Palette_BL")]
         public double Stock_100n_Palette_BL { get; set; }


         //-201u / Compeosite-\\

         [Display(Name = "Stock_201u_Composite_QI")]
         public double Stock_201u_Composite_QI { get; set; }

         [Display(Name = "Stock_201u_Composite_UR")]
         public double Stock_201u_Composite_UR { get; set; }

         [Display(Name = "Stock_201u_Composite_BL")]
         public double Stock_201u_Composite_BL { get; set; }



         //-300U / Hollow Profile-\\

         [Display(Name = "Stock_300u_HollowProfile_QI")]
         public double Stock_300u_HollowProfile_QI { get; set; }

         [Display(Name = "Stock_300u_HollowProfile_UR")]
         public double Stock_300u_HollowProfile_UR { get; set; }

         [Display(Name = "Stock_300u_HollowProfile_BL")]
         public double Stock_300u_HollowProfile_BL { get; set; }

         //-400U / Top Frames-\\

         [Display(Name = "Stock_400u_TopFrames_QI")]
         public double Stock_400u_TopFrames_QI { get; set; }

         [Display(Name = "Stock_400u_TopFrames_UR")]
         public double Stock_400u_TopFrames_UR { get; set; }

         [Display(Name = "Stock_400u_TopFrames_BL")]
         public double Stock_400u_TopFrames_BL { get; set; }


     }



}