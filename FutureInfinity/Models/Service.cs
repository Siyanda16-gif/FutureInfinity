using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GROUP_Q.Models
{
    public class Service
    {
        [Key]
        public string Service_Id { get; set; }
        [DisplayName("Make of Vehicle")]
        public string Veh_Make { get; set; }
        [DisplayName("What Services do you need?")]
        public virtual int si_Id { get; set; }
        public ServiceInfo ServiceInfo { get; set; }
        [DisplayName("Model/Engine Capacity of the vehicle")]
        public string Model { get; set; }
        [DisplayName("Current mileage")]
        public string Year { get; set; }
        [DisplayName("VIN number")]
        public string Vin { get; set; }
    }
}