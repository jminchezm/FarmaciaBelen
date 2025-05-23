using FarmaciaBelen.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace FarmaciaBelen.Controllers
{
    public class InventarioProductosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: InventarioProductos
        public ActionResult Index(string nombre, int? stok, string codigoProducto, decimal? precioMin, decimal? precioMax, string filtroStock)
        {

            var inventario = db.INVENTARIO_PRODUCTO.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
                inventario = inventario.Where(c => c.PRODUCTO.PRODUCTO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(codigoProducto))
                inventario = inventario.Where(c => c.PRODUCTO.PRODUCTO_ID.Contains(codigoProducto));

            if (stok > 0)
                inventario = inventario.Where(c => c.STOCK_ACTUAL == stok);


            // Filtro por rango de precio
            if (precioMin.HasValue)
                inventario = inventario.Where(c =>
                    c.PRECIO_UNITARIO >= precioMin.Value);

            if (precioMax.HasValue)
                inventario = inventario.Where(c =>
                    c.PRECIO_UNITARIO <= precioMax.Value);



            // Filtro por botones
            if (!string.IsNullOrEmpty(filtroStock))
            {
                if (filtroStock == "Bajo")
                {
                    inventario = inventario.Where(c => c.STOCK_ACTUAL <= 10);
                }
                else if (filtroStock == "Alto")
                {
                    inventario = inventario.Where(c => c.STOCK_ACTUAL >= 100);
                }
            }




            return View(inventario.ToList());
        }

        // GET: InventarioProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO = db.INVENTARIO_PRODUCTO.Find(id);
            if (iNVENTARIO_PRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(iNVENTARIO_PRODUCTO);
        }

        // GET: InventarioProductos/Create
        public ActionResult Create()
        {
            ViewBag.PRODUCTO_ID = new SelectList(db.PRODUCTO, "PRODUCTO_ID", "PRODUCTO_NOMBRE");
            return View();
        }

        // POST: InventarioProductos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "INVENTARIO_ID,PRODUCTO_ID,STOCK_ACTUAL,PRECIO_UNITARIO")] INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO)
        {
            if (ModelState.IsValid)
            {
                db.INVENTARIO_PRODUCTO.Add(iNVENTARIO_PRODUCTO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PRODUCTO_ID = new SelectList(db.PRODUCTO, "PRODUCTO_ID", "PRODUCTO_NOMBRE", iNVENTARIO_PRODUCTO.PRODUCTO_ID);
            return View(iNVENTARIO_PRODUCTO);
        }

        // GET: InventarioProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO = db.INVENTARIO_PRODUCTO.Find(id);
            if (iNVENTARIO_PRODUCTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.PRODUCTO_ID = new SelectList(db.PRODUCTO, "PRODUCTO_ID", "PRODUCTO_NOMBRE", iNVENTARIO_PRODUCTO.PRODUCTO_ID);
            return View(iNVENTARIO_PRODUCTO);
        }

        // POST: InventarioProductos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "INVENTARIO_ID,PRODUCTO_ID,STOCK_ACTUAL,PRECIO_UNITARIO")] INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iNVENTARIO_PRODUCTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PRODUCTO_ID = new SelectList(db.PRODUCTO, "PRODUCTO_ID", "PRODUCTO_NOMBRE", iNVENTARIO_PRODUCTO.PRODUCTO_ID);
            return View(iNVENTARIO_PRODUCTO);
        }

        // GET: InventarioProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO = db.INVENTARIO_PRODUCTO.Find(id);
            if (iNVENTARIO_PRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(iNVENTARIO_PRODUCTO);
        }

        // POST: InventarioProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            INVENTARIO_PRODUCTO iNVENTARIO_PRODUCTO = db.INVENTARIO_PRODUCTO.Find(id);
            db.INVENTARIO_PRODUCTO.Remove(iNVENTARIO_PRODUCTO);
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
