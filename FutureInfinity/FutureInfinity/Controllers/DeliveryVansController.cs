using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GROUP_Q.Models;

namespace GROUP_Q.Controllers
{
    public class DeliveryVansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DeliveryVans
        public ActionResult Index()
        {
            return View(db.DeliveryVans.ToList());
        }

        // GET: DeliveryVans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryVan deliveryVan = db.DeliveryVans.Find(id);
            if (deliveryVan == null)
            {
                return HttpNotFound();
            }
            return View(deliveryVan);
        }

        // GET: DeliveryVans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeliveryVans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeliveryVanId,NumberPlate,IsAvailable,DriverSignature,StatusNum")] DeliveryVan deliveryVan)
        {
            if (ModelState.IsValid)
            {
                db.DeliveryVans.Add(deliveryVan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deliveryVan);
        }


        // GET: DeliveryVans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryVan deliveryVan = db.DeliveryVans.Find(id);
            if (deliveryVan == null)
            {
                return HttpNotFound();
            }
            return View(deliveryVan);
        }

        // POST: DeliveryVans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryVanId,NumberPlate,Status,DriverSignature,StatusNum")] DeliveryVan deliveryVan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryVan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deliveryVan);
        }

        // GET: DeliveryVans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryVan deliveryVan = db.DeliveryVans.Find(id);
            if (deliveryVan == null)
            {
                return HttpNotFound();
            }
            return View(deliveryVan);
        }

        // POST: DeliveryVans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeliveryVan deliveryVan = db.DeliveryVans.Find(id);
            db.DeliveryVans.Remove(deliveryVan);
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
