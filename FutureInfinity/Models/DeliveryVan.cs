using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GROUP_Q.Models
{
    public class DeliveryVan
    {
        [Display(Name = "Tow Truck Id")]
        public int DeliveryVanId { get; set; }

        public string NumberPlate { get; set; }

        public bool IsAvailable { get; set; }

        public string DriverSignature { get; set; }

        public int StatusNum { get; set; }
    }
}