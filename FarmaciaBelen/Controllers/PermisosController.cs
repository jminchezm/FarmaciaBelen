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
    public class PermisosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Permisos
        public ActionResult Index()
        {
            var pERMISOS = db.PERMISOS.Include(p => p.MODULO).Include(p => p.ROL);
            return View(pERMISOS.ToList());
        }

        // GET: Permisos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERMISOS pERMISOS = db.PERMISOS.Find(id);
            if (pERMISOS == null)
            {
                return HttpNotFound();
            }
            return View(pERMISOS);
        }

        // GET: Permisos/Create
        public ActionResult Create()
        {
            ViewBag.MODULO_ID = new SelectList(db.MODULO, "MODULO_ID", "MODULO_NOMBRE");
            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE");
            return View();
        }

        // POST: Permisos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PERMISOS_ID,ROL_ID,MODULO_ID,PERMISOS_PUEDEACCEDER")] PERMISOS pERMISOS)
        {
            if (ModelState.IsValid)
            {
                db.PERMISOS.Add(pERMISOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MODULO_ID = new SelectList(db.MODULO, "MODULO_ID", "MODULO_NOMBRE", pERMISOS.MODULO_ID);
            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", pERMISOS.ROL_ID);
            return View(pERMISOS);
        }

        // GET: Permisos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERMISOS pERMISOS = db.PERMISOS.Find(id);
            if (pERMISOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.MODULO_ID = new SelectList(db.MODULO, "MODULO_ID", "MODULO_NOMBRE", pERMISOS.MODULO_ID);
            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", pERMISOS.ROL_ID);
            return View(pERMISOS);
        }

        // POST: Permisos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PERMISOS_ID,ROL_ID,MODULO_ID,PERMISOS_PUEDEACCEDER")] PERMISOS pERMISOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pERMISOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MODULO_ID = new SelectList(db.MODULO, "MODULO_ID", "MODULO_NOMBRE", pERMISOS.MODULO_ID);
            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", pERMISOS.ROL_ID);
            return View(pERMISOS);
        }

        // GET: Permisos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PERMISOS pERMISOS = db.PERMISOS.Find(id);
            if (pERMISOS == null)
            {
                return HttpNotFound();
            }
            return View(pERMISOS);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PERMISOS pERMISOS = db.PERMISOS.Find(id);
            db.PERMISOS.Remove(pERMISOS);
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
