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
    public class ServiceInfoesController : Controller
    {
        private ServicingContext db = new ServicingContext();

        // GET: ServiceInfoes
        public ActionResult Index()
        {
            return View(db.ServiceInfo.ToList());
        }

        // GET: ServiceInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceInfo serviceInfo = db.ServiceInfo.Find(id);
            if (serviceInfo == null)
            {
                return HttpNotFound();
            }
            return View(serviceInfo);
        }

        // GET: ServiceInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServiceInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "si_Id,service_Type")] ServiceInfo serviceInfo)
        {
            if (ModelState.IsValid)
            {
                db.ServiceInfo.Add(serviceInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviceInfo);
        }

        // GET: ServiceInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceInfo serviceInfo = db.ServiceInfo.Find(id);
            if (serviceInfo == null)
            {
                return HttpNotFound();
            }
            return View(serviceInfo);
        }

        // POST: ServiceInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "si_Id,service_Type")] ServiceInfo serviceInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceInfo);
        }

        // GET: ServiceInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceInfo serviceInfo = db.ServiceInfo.Find(id);
            if (serviceInfo == null)
            {
                return HttpNotFound();
            }
            return View(serviceInfo);
        }

        // POST: ServiceInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceInfo serviceInfo = db.ServiceInfo.Find(id);
            db.ServiceInfo.Remove(serviceInfo);
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
