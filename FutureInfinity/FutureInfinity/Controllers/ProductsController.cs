using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GROUP_Q.Models;
using Microsoft.AspNet.Identity;

namespace GROUP_Q.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        public ActionResult HiredList()
        {
            return View(db.Orders.ToList());
        }
        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Orders()
        {
            string userId = User.Identity.GetUserId();
            var userOrders = db.Orders.Where(o => o.UserId == userId).ToList();
            return View(userOrders);
        }
        [HttpPost]
        public ActionResult PlaceOrder(Cart cart, DateTime? FromDate,DateTime? ToDate, string DeliveryAddress, string ContactNo)
        {
            Session["ContactViewModel"] = cart;

            var Cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            double Total = 0,FinalTotal = 0;
            TimeSpan HireDays = ToDate.Value - FromDate.Value;
            int differenceInDays = HireDays.Days;

            foreach (var item in Cart)
            {
                Total += item.Price * item.Price;
            }
            FinalTotal = Total * differenceInDays;

            Order order = new Order();
            order.UserId = User.Identity.GetUserId();
            order.OrderDate = DateTime.Now;
            order.FromDate = FromDate;
            order.ToDate = ToDate;
            order.DeliveryAddress = DeliveryAddress;
            order.ContactNo = ContactNo;
            order.Total = Total;
            order.HireDays = differenceInDays;
            order.FinalTotal = FinalTotal;
            order.Status = "Payment Recieved";
            db.Orders.Add(order);
            db.SaveChanges();

            foreach(var item in Cart)
            {
                OrderDetail orderDetail = new OrderDetail();              
                orderDetail.ProductId = item.ProductId;
                orderDetail.ProductName = item.ProductName;
                orderDetail.ProductPic = item.ProductPic;
                orderDetail.Description = item.Description;
                orderDetail.Price = item.Price;
                orderDetail.Quantity = item.Quantity;
                orderDetail.OrderId = order.OrderId;
                db.OrderDetails.Add(orderDetail); 
                db.SaveChanges();
            }
            SaveToManifesto(order, db);
            Session["Cart"] = null;

            TempData["OrderPlaced"] = "Thank For Hiring Your Stuff With Book Rental, We Will Complete Your Delivery Soon";
            return RedirectToAction("Payment", "Products");
        }
        private void SaveToManifesto(Order order, ApplicationDbContext dbContext)
        {
            var contactViewModel = Session["ContactViewModel"] as Cart;

            if (contactViewModel != null)
            {
                var manifesto = new Manifesto
                {
                    FullName = db.Users.Where(x => x.Id == order.UserId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
                    DestinationAddress = order.DeliveryAddress,
                    ContactNo = order.ContactNo,
                    DeliveryDate = DateTime.Now,
                    Status = "Waiting For Driver To Be Assigned",
                    OrderId = order.OrderId
                };

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbContext.Manifestos.Add(manifesto);
                        dbContext.SaveChanges();

                        Session["ContactViewModel"] = null;
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        [HttpGet]
        public ActionResult Cart()
        {
            var Cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            return View(Cart);
        }
        [HttpPost]
        public ActionResult AddToCart(int ProductId,string Description, string Name, double Price, string Picture, int Qty)
        {
            var Cart = Session["Cart"] as List<Cart> ?? new List<Cart>();

            var productInCart = Cart.FirstOrDefault(x => x.ProductId == ProductId);

            if (productInCart != null)
            {
                productInCart.Quantity += Qty;
            }
            else
            {
                var cart = new Cart
                {
                    ProductId = ProductId,
                    ProductName = Name,
                    ProductPic = Picture,
                    Description= Description,
                    Quantity = Qty,
                    Price = Price,
                };

                Cart.Add(cart);

            }
            Session["Cart"] = Cart;

            return RedirectToAction("Cart");
        }
        [HttpPost]
        public ActionResult RemoveItemFromCart(int ProductId)
        {
            var Cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var RemoveItem = Cart.Where(x => x.ProductId == ProductId).FirstOrDefault();

            Cart.Remove(RemoveItem);
            return RedirectToAction("Cart");
        }
        [HttpPost]
        public ActionResult AddQty(int ProductId,int Quantity)
        {
            var Cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            var AddQty = Cart.Where(x => x.ProductId == ProductId).FirstOrDefault();
            AddQty.Quantity = Quantity;

            Session["Cart"] = Cart;
            return RedirectToAction("Cart");
        }
        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductName,ProductPrice,ProductPicture,Quantity")] Product product, HttpPostedFileBase ProductPicture)
        {
            if (ModelState.IsValid)
            {
                if (ProductPicture != null && ProductPicture.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(ProductPicture.InputStream))
                    {
                        product.ProductPicture = Convert.ToBase64String(reader.ReadBytes(ProductPicture.ContentLength));
                    }
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }              
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductName,ProductPrice,ProductPicture,Quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
