using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GROUP_Q.Models
{
    public class DeliveryVan
    {
        public int DeliveryVanId { get; set; }

        public string NumberPlate { get; set; }

        public bool IsAvailable { get; set; }

        public string DriverSignature { get; set; }

        public int StatusNum { get; set; }
    }
}