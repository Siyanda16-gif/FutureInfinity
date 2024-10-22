using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GROUP_Q.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Service Name")]
        public string ProductName { get; set; }
        [Display(Name = "Service Price")]
        public int ProductPrice { get; set; }
        [Display(Name = "Product Service Picture")]
        public string ProductPicture { get; set; }
        [Display(Name = "Service Description")]
        public string Description { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
    public class Cart
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPic { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPic { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

    }
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public double Total { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set;}
        [Display(Name = "Collection Address")]
        public string DeliveryAddress { get; set; }
        public string ContactNo { get; set; }
        public int HireDays { get; set; }
        public double FinalTotal { get; set; }

    }
}