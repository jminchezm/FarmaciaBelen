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
    public class ViaAdministracionesController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: ViaAdministraciones
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var ViaAdministracion = db.VIAADMINISTRACION.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                ViaAdministracion = ViaAdministracion.Where(v => v.VIAADMINISTRACION_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                ViaAdministracion = ViaAdministracion.Where(v => v.VIAADMINISTRATIVA_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                ViaAdministracion = ViaAdministracion.Where(v => v.VIAADMINISTRACION_ESTADO == estado);

            return View(ViaAdministracion.ToList());
        }

        // GET: ViaAdministraciones/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VIAADMINISTRACION vIAADMINISTRACION = db.VIAADMINISTRACION.Find(id);
            if (vIAADMINISTRACION == null)
            {
                return HttpNotFound();
            }
            return View(vIAADMINISTRACION);
        }

        // GET: ViaAdministraciones/Create
        public ActionResult Create()
        {
            var ultimoViaAdministracion = db.VIAADMINISTRACION
                .OrderByDescending(v => v.VIAADMINISTRACION_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoViaAdministracion != null && ultimoViaAdministracion.VIAADMINISTRACION_ID.Length == 10)
            {
                string numero = ultimoViaAdministracion.VIAADMINISTRACION_ID.Substring(4);
                int siguiente = int.Parse(numero) + 1;
                nuevoCodigo = "VIAD" + siguiente.ToString("D6");
            }
            else
            {
                nuevoCodigo = "VIAD000001";
            }

            var nuevoViaAdministracion = new VIAADMINISTRACION
            {
                VIAADMINISTRACION_ID = nuevoCodigo
                //PROVEEDOR_ESTADO = "Activo"
            };

            ViewBag.Creado = TempData["Creado"];
            return View(nuevoViaAdministracion);
        }

        // POST: ViaAdministraciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VIAADMINISTRACION_ID,VIAADMINISTRATIVA_NOMBRE,VIAADMINISTRACION_DESCRIPCION,VIAADMINISTRACION_ESTADO")] VIAADMINISTRACION viaadministracion)
        {


            if (!string.IsNullOrWhiteSpace(viaadministracion.VIAADMINISTRATIVA_NOMBRE))
            {
                bool existe = db.VIAADMINISTRACION.Any(v => v.VIAADMINISTRATIVA_NOMBRE == viaadministracion.VIAADMINISTRATIVA_NOMBRE);
                if (existe)
                    ModelState.AddModelError("PROVEEDOR_NOMBRE", "Este nombre ya está en uso.");
            }


            if (ModelState.IsValid)
            {
                db.VIAADMINISTRACION.Add(viaadministracion);
                db.SaveChanges();
                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            return View(viaadministracion);
        }

        // GET: ViaAdministraciones/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VIAADMINISTRACION vIAADMINISTRACION = db.VIAADMINISTRACION.Find(id);
            if (vIAADMINISTRACION == null)
            {
                return HttpNotFound();
            }
            ViewBag.Editado = TempData["Editado"];
            return View(vIAADMINISTRACION);
        }

        // POST: ViaAdministraciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VIAADMINISTRACION_ID,VIAADMINISTRATIVA_NOMBRE,VIAADMINISTRACION_DESCRIPCION,VIAADMINISTRACION_ESTADO")] VIAADMINISTRACION viaadministracion)
        {
            if (!string.IsNullOrWhiteSpace(viaadministracion.VIAADMINISTRATIVA_NOMBRE))
            {
                bool existe = db.VIAADMINISTRACION.Any(v =>
                    v.VIAADMINISTRATIVA_NOMBRE == viaadministracion.VIAADMINISTRATIVA_NOMBRE &&
                    v.VIAADMINISTRACION_ID != viaadministracion.VIAADMINISTRACION_ID
                );

                if (existe)
                    ModelState.AddModelError("VIAADMINISTRATIVA_NOMBRE", "Este nombre ya está en uso.");
            }


            if (ModelState.IsValid)
            {
                db.Entry(viaadministracion).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = viaadministracion.VIAADMINISTRACION_ID });
            }
            return View(viaadministracion);
        }

        // GET: ViaAdministraciones/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VIAADMINISTRACION vIAADMINISTRACION = db.VIAADMINISTRACION.Find(id);
            if (vIAADMINISTRACION == null)
            {
                return HttpNotFound();
            }

            ViewBag.Desactivado = TempData["Desactivado"];
            return View(vIAADMINISTRACION);
        }

        // POST: ViaAdministraciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var viaAdministracion = db.VIAADMINISTRACION.Find(id);
            if (viaAdministracion == null)
                return HttpNotFound();

            viaAdministracion.VIAADMINISTRACION_ESTADO = "Inactivo";
            db.Entry(viaAdministracion).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true;
            return RedirectToAction("Delete", new { id = viaAdministracion.VIAADMINISTRACION_ID });
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
