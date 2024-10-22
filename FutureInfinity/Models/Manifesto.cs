using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GROUP_Q.Models
{
    public class Manifesto
    {
        [Key]
        public int Id { get; set; }
        public string CustomerID { get; set; }

        [Display(Name = "Deliver To")]
        public string DestinationAddress { get; set; }
        public string Status { get; set; }
        public string DriverEmail { get; set; }
        public string NumberPlate { get; set; }
        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Phone")]
        public string ContactNo { get; set; }
        public int? DeliveryVanId { get; set; }
        [ForeignKey("DeliveryVanId")]
        public virtual DeliveryVan DeliveryVan { get; set; }
        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual Driver Drivers { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}