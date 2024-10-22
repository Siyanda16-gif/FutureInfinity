using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GROUP_Q.Models
{
    public class ServicingContext: DbContext
    {
        public ServicingContext(): base("QueDb")
        {

        }
        public virtual DbSet<ServiceInfo> ServiceInfo { get; set; }
        public virtual DbSet<Service> Service { get; set; }
    }
}