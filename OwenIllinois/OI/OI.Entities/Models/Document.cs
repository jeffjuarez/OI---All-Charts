using System;
using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Ef6;

namespace OI.Entities.Models
{
    public class Document : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Filename { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FilePath { get; set; }

        [Required]
        [MaxLength(100)]
        public string SortCenter { get; set; }

        [Required]
        public DateTime ReceivedDateFrom { get; set; }

        [Required]
        public DateTime DateUploaded { get; set; }

        [Required]
        [MaxLength(100)]
        public string PreparedBy { get; set; }


        [Required]
        public int EmployeeId { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }


        //PRODUCTIVITY
        //**100n**////**201u**////**300u**////**400u**//
        [Required]
        public double Productivity_100n { get; set; }
        [Required]
        public double Productivity_201u { get; set; }
        [Required]
        public double Productivity_300u { get; set; }
        [Required]
        public double Productivity_400u { get; set; }


        // UR/SC RATIO
        [Required]
        public double URSC_BL_Pallet { get; set; }
        [Required]
        public double URSC_UR_Pallet { get; set; }
        [Required]
        public double URSC_SC_Pallet { get; set; }
        [Required]
        public double URSC_BL_Composite { get; set; }
        [Required]
        public double URSC_UR_Composite { get; set; }
        [Required]
        public double URSC_SC_Composite { get; set; }
        [Required]
        public double URSC_BL_HollowProfile { get; set; }
        [Required]
        public double URSC_UR_HollowProfile { get; set; }
        [Required]
        public double URSC_SC_HollowProfile { get; set; }
        [Required]
        public double URSC_BL_TopFrames { get; set; }
        [Required]
        public double URSC_UR_TopFrames { get; set; }
        [Required]
        public double URSC_SC_TopFrames { get; set; }


        //TRUCK MOVEMENT RATIO
        [Required]
        public double Truck_Movement_IN { get; set; }
        [Required]
        public double Truck_Movement_OUT { get; set; }
        [Required]
        public double Truck_Movement_Ratio { get; set; }

       


        //PACKED ITEM RECEIVED
        //**100n**////**201u**////**300u**////**400u**//
        [Required]
        public double PackedItemsRec_100n_Pallet_TotalRec { get; set; }
        [Required]
        public double PackedItemsRec_201u_Composite_TotalRec { get; set; }
        [Required]
        public double PackedItemsRec_300u_BlackHollowProfile_TotalRec { get; set; }
        [Required]
        public double PackedItemsRec_400u_TopFrames_TotalRec { get; set; }
   

        //PACKED ITEM DISPATCHED
        //**100n**////**201u**////**300u**////**400u**//
        [Required]
        public double PackedItemsDisp_100n_Pallet_TotalDisp { get; set; }
        [Required]
        public double PackedItemsDisp_201u_Composite_TotalDisp { get; set; }
        [Required]
        public double PackedItemsDisp_300u_BlackHollowProfile_TotalDisp { get; set; }
        [Required]
        public double PackedItemsDisp_400u_TopFrames_TotalDisp { get; set; }

        //STOCK ON HAND
        //**100n-pallete**////**201u-Composite**////**300u-HollowProfile**////**400u-TopFrame**//
        [Required]
        public double Stock_100n_Pallet_QI { get; set; }
        [Required]
        public double Stock_201u_Composite_QI { get; set; }
        [Required]
        public double Stock_300u_HollowProfile_QI { get; set; }
        [Required]
        public double Stock_400u_TopFrame_QI { get; set; }

        [Required]
        public double Stock_100n_Pallet_UR { get; set; }
        [Required]
        public double Stock_201u_Composite_UR { get; set; }
        [Required]
        public double Stock_300u_HollowProfile_UR { get; set; }
        [Required]
        public double Stock_400u_TopFrame_UR { get; set; }

        [Required]
        public double Stock_100n_Pallet_BL { get; set; }
        [Required]
        public double Stock_201u_Composite_BL { get; set; }
        [Required]
        public double Stock_300u_HollowProfile_BL { get; set; }
        [Required]
        public double Stock_400u_TopFrame_BL { get; set; }
      
    }
}
