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
    public class ProveedoresController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Proveedores
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var proveedores = db.PROVEEDOR.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                proveedores = proveedores.Where(p => p.PROVEEDOR_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                proveedores = proveedores.Where(p => p.PROVEEDOR_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                proveedores = proveedores.Where(p => p.PROVEEDOR_ESTADO == estado);

            return View(proveedores.ToList());

        }

        // GET: Proveedores/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROVEEDOR pROVEEDOR = db.PROVEEDOR.Find(id);
            if (pROVEEDOR == null)
            {
                return HttpNotFound();
            }
            return View(pROVEEDOR);
        }






        // GET: Proveedores/Create
        public ActionResult Create()
        {
            var ultimaProveedor = db.PROVEEDOR
                .OrderByDescending(p => p.PROVEEDOR_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimaProveedor != null && ultimaProveedor.PROVEEDOR_ID.Length == 10)
            {
                string numero = ultimaProveedor.PROVEEDOR_ID.Substring(4);
                int siguiente = int.Parse(numero) + 1;
                nuevoCodigo = "PROV" + siguiente.ToString("D6");
            }
            else
            {
                nuevoCodigo = "PROV000001";
            }

            var nuevaProveedor = new PROVEEDOR
            {
                PROVEEDOR_ID = nuevoCodigo
                //PROVEEDOR_ESTADO = "Activo"
            };

            ViewBag.Creado = TempData["Creado"];
            return View(nuevaProveedor);
        }

        // POST: Proveedores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PROVEEDOR_ID,PROVEEDOR_NOMBRE,PROVEEDOR_TELEFONO,PROVEEDOR_CELULAR,PROVEEDOR_CORREO,PROVEEDOR_DIRECCION,PROVEEDOR_ESTADO")] PROVEEDOR proveedor)
        {

            if (!string.IsNullOrWhiteSpace(proveedor.PROVEEDOR_NOMBRE))
            {
                bool existe = db.PROVEEDOR.Any(p => p.PROVEEDOR_NOMBRE == proveedor.PROVEEDOR_NOMBRE);
                if (existe)
                    ModelState.AddModelError("PROVEEDOR_NOMBRE", "Este nombre ya está en uso.");
            }


            if (ModelState.IsValid)
            {
                db.PROVEEDOR.Add(proveedor);
                db.SaveChanges();
                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROVEEDOR pROVEEDOR = db.PROVEEDOR.Find(id);
            if (pROVEEDOR == null)
            {
                return HttpNotFound();
            }

            ViewBag.Editado = TempData["Editado"];
            return View(pROVEEDOR);
        }

        // POST: Proveedores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PROVEEDOR_ID,PROVEEDOR_NOMBRE,PROVEEDOR_TELEFONO,PROVEEDOR_CELULAR,PROVEEDOR_CORREO,PROVEEDOR_DIRECCION,PROVEEDOR_ESTADO")] PROVEEDOR proveedor)
        {

            if (!string.IsNullOrWhiteSpace(proveedor.PROVEEDOR_NOMBRE))
            {
                bool existe = db.PROVEEDOR.Any(p =>
                    p.PROVEEDOR_NOMBRE == proveedor.PROVEEDOR_NOMBRE &&
                    p.PROVEEDOR_ID != proveedor.PROVEEDOR_ID
                );

                if (existe)
                    ModelState.AddModelError("CATEGORIAPRODUCTO_NOMBRE", "Este nombre ya está en uso.");
            }


            if (ModelState.IsValid)
            {
                db.Entry(proveedor).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = proveedor.PROVEEDOR_ID });
            }
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }



            var proveedor = db.PROVEEDOR.Find(id);
            if (proveedor == null)
                return HttpNotFound();

            ViewBag.Desactivado = TempData["Desactivado"];
            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {


            var proveedor = db.PROVEEDOR.Find(id);
            if (proveedor == null)
                return HttpNotFound();

            proveedor.PROVEEDOR_ESTADO = "Inactivo";
            db.Entry(proveedor).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true;
            return RedirectToAction("Delete", new { id = proveedor.PROVEEDOR_ID });
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
