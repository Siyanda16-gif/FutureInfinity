using GROUP_Q.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;


namespace GROUP_Q.Controllers
{
    public class DriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DriversController()
        {
            _context = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
        }
        [HttpGet]
        public ActionResult ConfirmReturn(int orderid)
        {
            Manifesto manifestoInfo = (from x in db.Manifestos
                                     where x.OrderId == orderid
                                       select x).FirstOrDefault();
            TempData["OrderId"] = orderid;
            return View(manifestoInfo);
        }
        [HttpPost]
        public ActionResult ConfirmReturn(Order order, string signature)
        {
            order.OrderId = int.Parse(TempData["OrderId"].ToString());
            Order Z = db.Orders.Find(order.OrderId);
            Z.Status = "Return process completed";

            Manifesto manifestoInfo = db.Manifestos.FirstOrDefault(x => x.OrderId == Z.OrderId);
            manifestoInfo.Status = "Return process completed";
            db.SaveChanges();

            Manifesto info = db.Manifestos.Find(order.OrderId);
            var GetVehicleStatus = (from x in db.DeliveryVans
                                    where x.NumberPlate == info.NumberPlate
                                    select x.DeliveryVanId).FirstOrDefault();        
            DeliveryVan van = db.DeliveryVans.Find(GetVehicleStatus);
            van.IsAvailable = true;
            db.SaveChanges();

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Docs\\" + Z.OrderId);

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            GetQuery query = new GetQuery();

            string signame = User.Identity.Name + query.Main() + Z.OrderId + "signature.png";
            //check if a file was  uploaded
            if (signature != null)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(signature.Replace("data:image/png;base64,", ""))))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        bm2.Save(Server.MapPath("~/Images/Uploads/Docs/" + Z.OrderId + "/" + signame), System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            return Redirect("/Products/HiredList");
        }

        [HttpPost]
        public JsonResult UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
                var manifesto = db.Manifestos.Find(orderId);

                if (order != null && manifesto != null)
                {
                    order.Status = status;
                    db.SaveChanges();

                    manifesto.Status = status;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Order status updated successfully." });
                }

                return Json(new { success = false, message = "Order not found." });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return Json(new { success = false, message = "Error occurred while updating order status." });
            }
        }


        [HttpGet]
        public ActionResult ConfirmSign(int manifestoInfoId)
        {
            Manifesto manifestoInfo = db.Manifestos.Find(manifestoInfoId);
            return View(manifestoInfo);
        }

        [HttpPost]
        public ActionResult ConfirmSign(int manifestoInfoId, string signature)
        {
            Manifesto info = db.Manifestos.Find(manifestoInfoId);
            Order order = db.Orders.Find(info.OrderId);

            info.Status = "Package collected from customer";
            order.Status = "Package collected from customer";

            var GetVehicleStatus = (from x in db.DeliveryVans
                                    where x.NumberPlate == info.NumberPlate
                                    select x.DeliveryVanId).FirstOrDefault();

            DeliveryVan van = db.DeliveryVans.Find(GetVehicleStatus);

            van.IsAvailable = false;

            db.SaveChanges();

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Gateindocs\\" + order.OrderId.ToString());

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }

            GetQuery query = new GetQuery();

            string signame = User.Identity.Name + query.Main() + order.OrderId + "signature.png";

            //check if a file was uploaded
            if (signature != null)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(signature.Replace("data:image/png;base64,", ""))))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {

                        bm2.Save(Server.MapPath("~/Images/Uploads/Gateindocs/" + order.OrderId + "/" + signame), System.Drawing.Imaging.ImageFormat.Png);

                    }
                }
            }
            return Redirect("/Drivers/ViewDetails?id=" + info.OrderId);
        }
        [HttpPost]
        public ActionResult RequestReturn(int orderId)
        {
            var order = db.Orders.Find(orderId);
            var manifesto = db.Manifestos.Find(orderId);

            if (order != null && manifesto != null)
            {               
                order.Status = "Return Requested";
                db.SaveChanges();

                manifesto.Status = "Return Requested";
                db.SaveChanges();

                return Json(new { success = true, message = "Return requested successfully." });
            }

            return Json(new { success = false, message = "Order not found." });
        }

        [HttpGet]
        public ActionResult CustomerConfirm(int manifestoInfoId)
        {
            Manifesto manifestoInfo = db.Manifestos.Find(manifestoInfoId);
            return View(manifestoInfo);
        }
        [HttpPost]
        public ActionResult CustomerConfirm(Manifesto manifestoInfo, string signature, string customerSignature)
        {
            Manifesto info = db.Manifestos.Find(manifestoInfo.Id);
            info.Status = "Delivered To Customer";

            Order order = db.Orders.Find(info.OrderId);
            order.Status = "Delivered To Customer";

            var GetVehicleStatus = (from x in db.DeliveryVans
                                    where x.NumberPlate == info.NumberPlate
                                    select x.DeliveryVanId).FirstOrDefault();
            DeliveryVan van = db.DeliveryVans.Find(GetVehicleStatus);
            van.IsAvailable = true;
            db.SaveChanges();

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString2 = Path.Combine(originalDirectory.ToString(), "Gateindocs\\" + order.OrderId.ToString());

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }

            GetQuery query = new GetQuery();

            string driverSigName = User.Identity.Name + query.Main() + order.OrderId + "driver_signature.png";

            string customerSigName = User.Identity.Name + query.Main() + order.OrderId + "customer_signature.png";

            // Save driver's signature
            if (!string.IsNullOrEmpty(signature))
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(signature.Replace("data:image/png;base64,", ""))))
                {
                    using (Bitmap bm = new Bitmap(ms))
                    {
                        bm.Save(Server.MapPath("~/Images/Uploads/Gateindocs/" + order.OrderId + "/" + driverSigName), System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }

            // Save customer's signature
            if (!string.IsNullOrEmpty(customerSignature))
            {
                using (MemoryStream mss = new MemoryStream(Convert.FromBase64String(customerSignature.Replace("data:image/png;base64,", ""))))
                {
                    using (Bitmap bmm = new Bitmap(mss))
                    {
                        bmm.Save(Server.MapPath("~/Images/Uploads/Gateindocs/" + order.OrderId + "/" + customerSigName), System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }

            // Send an email notification to customer
            SendDeliveryCompleteEmail(manifestoInfo, driverSigName, customerSigName);


            return Redirect("/Drivers/DeliveryList?OrderId=" + info.OrderId);
        }
        private void SendDeliveryCompleteEmail(Manifesto manifestoInfo, string driverSignature, string customerSignature)
        {
            //var customerEmail = deliveryInfo.CustomerEmail;
            var driverEmail = manifestoInfo.DriverEmail;
            var driverLicensePlate = manifestoInfo.NumberPlate;
            var status = manifestoInfo.Status;
            var destinationAddress = manifestoInfo.DestinationAddress;
            var deliveryDate = manifestoInfo.DeliveryDate.ToString("dddd, MMMM d, yyyy");

            // Compose the email message
            var subject = "Delivery Confirmation: Package Handed Over";
            var body = $"Dear Customer,\n\n" +
                "We are pleased to inform you that your package has been successfully delivered. " +
                "Our driver has handed over the package to you as per the delivery instructions.\n\n" +
                "Delivery Details:\n\n" +
                $"Delivery Date: {deliveryDate}\n" +
                $"Driver Email: {driverEmail}\n" +
                $"Driver License Plate: {driverLicensePlate}\n" +
                $"Status: {status}\n" +
                $"Destination Address: {destinationAddress}\n\n" +
                "Thank you for choosing our delivery service. If you have any further inquiries or require assistance, " +
                "please do not hesitate to contact our customer support team at [support email/phone number].\n\n" +
                "Please find attached the delivery confirmation document, which includes both the driver and customer signatures.\n\n" +
                "Best regards,\n\n" +
                "[Jay_D]\n" +
                "[Future Infinity Minds]\n" +
                "[078 785 7141]";


            // Send the email
            var fromAddress = "thandowonke@gmail.com"; // Replace with your actual email address
            var toAddress = "thandowonke@gmail.com"; // Replace with the customer's email address
            var smtpClient = new SmtpClient("smtp.gmail.com", 587); // Replace with your SMTP server and port number
            smtpClient.EnableSsl = true; // Set to true if your SMTP server requires SSL
            smtpClient.Credentials = new NetworkCredential("thandowonke@gmail.com", "tfakicsyvxweitc"); // Replace with your SMTP credentials
            var mailMessage = new MailMessage(fromAddress, toAddress, subject, body);
            //mailMessage.Attachments.Add(new Attachment(new MemoryStream(pdfContent), pdfFileName, "application/pdf"));
            smtpClient.Send(mailMessage);
        }
        [HttpGet]
        public ActionResult DriverSignOff(int manifestoInfoId)
        {
            Manifesto manifestoInfo = db.Manifestos.Find(manifestoInfoId);
            return View(manifestoInfo);
        }

        [HttpPost]
        public ActionResult DriverSignOff(int manifestoInfoId, string signature)
        {
            Manifesto info = db.Manifestos.Find(manifestoInfoId);
            Order order = db.Orders.Find(info.OrderId);

            info.Status = "Package Has Been Dispatched";
            order.Status = "Package Has Been Dispatched";

            var GetVehicleStatus = (from x in db.DeliveryVans
                                    where x.NumberPlate == info.NumberPlate
                                    select x.DeliveryVanId).FirstOrDefault();

            DeliveryVan van = db.DeliveryVans.Find(GetVehicleStatus);

            van.IsAvailable = false;

            db.SaveChanges();

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Gateindocs\\" + order.OrderId.ToString());

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }

            GetQuery query = new GetQuery();

            string signame = User.Identity.Name + query.Main() + order.OrderId + "signature.png";

            //check if a file was uploaded
            if (signature != null)
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(signature.Replace("data:image/png;base64,", ""))))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {

                        bm2.Save(Server.MapPath("~/Images/Uploads/Gateindocs/" + order.OrderId + "/" + signame), System.Drawing.Imaging.ImageFormat.Png);

                    }
                }
            }
            return Redirect("/Drivers/ViewDetails?id=" + info.OrderId);
        }
        public class GetQuery
        {
            public string Main()
            {
                int length = 20;

                // creating a StringBuilder object()
                StringBuilder str_build = new StringBuilder();
                Random random = new Random();

                char letter;

                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
                return str_build.ToString();
            }

            public string StripHTML(string input)
            {
                return Regex.Replace(input, "<.*?>", " ");
            }
        }
        public ActionResult ViewDetails(int id)
        {
            Manifesto p = db.Manifestos.Find(id);
            return View(p);
        }
        public ActionResult DeliveryList()
        {
            string UserEmail = User.Identity.GetUserName();

            var D = (from x in db.Manifestos
                     where x.DriverEmail == UserEmail
                     select x).ToList();
            return View(D);
        }
        [HttpGet]
        public ActionResult DesignateDriver(int orderid)
        {
            Manifesto M = db.Manifestos.Find(orderid);
            ViewBag.DeliveryVanId = new SelectList(db.DeliveryVans.Where(v => v.IsAvailable == true), "DeliveryVanId", "NumberPlate");
            ViewBag.DriverId = new SelectList(db.Drivers, "DriverId", "Email");
            return View(M);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DesignateDriver([Bind(Include = "Id,DriverEmail,NumberPlate,Status,DestinationAddress,DeliveryDate,CustomerID,OrderId,DeliveryVanId,DriverId,ContactNo,FullName")] Manifesto manifesto)
        {
            Manifesto M = db.Manifestos.Find(manifesto.OrderId);
            Order order = db.Orders.Find(manifesto.OrderId);

            if (M != null && order != null && ModelState.IsValid)
            {
                var GetDriverEmail = (from x in db.Drivers
                                      where x.DriverId == manifesto.DriverId
                                      select x.Email).FirstOrDefault();

                var GetVehicles = (from x in db.DeliveryVans
                                   where x.DeliveryVanId == manifesto.DeliveryVanId
                                   select x).FirstOrDefault();

                if (GetVehicles != null && GetVehicles.IsAvailable == true)
                {
                    GetVehicles.IsAvailable = false;
                }
                else
                {
                    ModelState.AddModelError("", "Selected vehicle is not available.");
                    ViewBag.DeliveryVanId = new SelectList(db.DeliveryVans, "DeliveryVanId", "NumberPlate", M.DeliveryVanId);
                    ViewBag.DriverId = new SelectList(db.Drivers, "DriverId", "Email", M.DriverId);
                    return View(M);
                }

                M.DriverEmail = GetDriverEmail;
                M.NumberPlate = GetVehicles.NumberPlate;
                M.Status = "Order Appointed To Driver";
                M.DeliveryDate = manifesto.DeliveryDate;
                M.DeliveryVanId = manifesto.DeliveryVanId;
                M.DriverId = manifesto.DriverId;
                order.Status = "Order Appointed To Driver";

                db.SaveChanges();

                return RedirectToAction("HiredList", "Products");
            }

            ViewBag.DeliveryVanId = new SelectList(db.DeliveryVans, "DeliveryVanId", "NumberPlate", M.DeliveryVanId);
            ViewBag.DriverId = new SelectList(db.Drivers, "DriverId", "Email", M.DriverId);

            return View(M);
        }
        // GET: Drivers
        public ActionResult Index()
        {
            return View(db.Drivers.ToList());
        }

        // GET: Drivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // GET: Drivers/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateDriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new user account with the role of Driver
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user.Id, "Driver");

                    // Create a new Driver object with the details entered in the form
                    var driver = new Driver
                    {
                        UserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                    };

                    // Add the new Driver object to the database and save changes
                    _context.Drivers.Add(driver);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }

            return View(model);
        }

        // GET: Drivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DriverId,UserId,Email,FirstName,LastName,Phone")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Driver driver = db.Drivers.Find(id);
            db.Drivers.Remove(driver);
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
