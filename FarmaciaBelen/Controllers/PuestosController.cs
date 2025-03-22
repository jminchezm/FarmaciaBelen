using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class PuestosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Puestos
        public ActionResult Index()
        {
            var pUESTO = db.PUESTO.Include(p => p.AREA);
            return View(pUESTO.ToList());
        }

        // GET: Puestos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PUESTO pUESTO = db.PUESTO.Find(id);
            if (pUESTO == null)
            {
                return HttpNotFound();
            }
            return View(pUESTO);
        }

        // GET: Puestos/Create
        public ActionResult Create()
        {
            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE");
            return View();
        }

        // POST: Puestos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PUESTO_ID,PUESTO_NOMBRE,PUESTO_DESCRIPCION,PUESTO_ESTADO,AREA_ID")] PUESTO pUESTO)
        {
            if (ModelState.IsValid)
            {
                db.PUESTO.Add(pUESTO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE", pUESTO.AREA_ID);
            return View(pUESTO);
        }

        // GET: Puestos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PUESTO pUESTO = db.PUESTO.Find(id);
            if (pUESTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE", pUESTO.AREA_ID);
            return View(pUESTO);
        }

        // POST: Puestos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PUESTO_ID,PUESTO_NOMBRE,PUESTO_DESCRIPCION,PUESTO_ESTADO,AREA_ID")] PUESTO pUESTO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pUESTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE", pUESTO.AREA_ID);
            return View(pUESTO);
        }

        // GET: Puestos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PUESTO pUESTO = db.PUESTO.Find(id);
            if (pUESTO == null)
            {
                return HttpNotFound();
            }
            return View(pUESTO);
        }

        // POST: Puestos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PUESTO pUESTO = db.PUESTO.Find(id);
            db.PUESTO.Remove(pUESTO);
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
