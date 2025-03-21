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
    public class AreasController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Areas
        //public ActionResult Index()
        //{
        //    return View(db.AREA.ToList());
        //}

        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var areas = db.AREA.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                areas = areas.Where(a => a.AREA_ID.Contains(codigo)); // ← usa Contains para búsquedas parciales

            if (!string.IsNullOrEmpty(nombre))
                areas = areas.Where(a => a.AREA_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                areas = areas.Where(a => a.AREA_ESTADO == estado);

            return View(areas.ToList());
        }



        // GET: Areas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AREA aREA = db.AREA.Find(id);
            if (aREA == null)
            {
                return HttpNotFound();
            }
            return View(aREA);
        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AREA_ID,AREA_NOMBRE,AREA_DESCRIPCION,AREA_EXTENSION,AREA_CORREO,AREA_ESTADO")] AREA aREA)
        {
            if (ModelState.IsValid)
            {
                db.AREA.Add(aREA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aREA);
        }

        // GET: Areas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AREA aREA = db.AREA.Find(id);
            if (aREA == null)
            {
                return HttpNotFound();
            }
            return View(aREA);
        }

        // POST: Areas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AREA_ID,AREA_NOMBRE,AREA_DESCRIPCION,AREA_EXTENSION,AREA_CORREO,AREA_ESTADO")] AREA aREA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aREA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aREA);
        }

        // GET: Areas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AREA aREA = db.AREA.Find(id);
            if (aREA == null)
            {
                return HttpNotFound();
            }
            return View(aREA);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AREA aREA = db.AREA.Find(id);
            db.AREA.Remove(aREA);
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
