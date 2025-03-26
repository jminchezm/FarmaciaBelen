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
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var puestos = db.PUESTO.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                puestos = puestos.Where(a => a.PUESTO_ID.Contains(codigo)); // ← usa Contains para búsquedas parciales

            if (!string.IsNullOrEmpty(nombre))
                puestos = puestos.Where(a => a.PUESTO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                puestos = puestos.Where(a => a.PUESTO_ESTADO == estado);

            return View(puestos.ToList());
            //var pUESTO = db.PUESTO.Include(p => p.AREA);
            //return View(pUESTO.ToList());
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
            // Obtener el último código existente
            var ultimoPuesto = db.PUESTO
                .OrderByDescending(p => p.PUESTO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoPuesto != null && ultimoPuesto.PUESTO_ID.Length == 10)
            {
                // Extraer el número del código (últimos 6 caracteres)
                string numero = ultimoPuesto.PUESTO_ID.Substring(4); // "000004"
                int siguienteNumero = int.Parse(numero) + 1;

                // Formatear el nuevo código con ceros y prefijo
                nuevoCodigo = "PSTO" + siguienteNumero.ToString("D6");
            }
            else
            {
                // Si no hay registros o el formato es incorrecto, empezar con el primero
                nuevoCodigo = "PSTO000001";
            }

            var nuevoPuesto = new PUESTO
            {
                PUESTO_ID = nuevoCodigo,
                PUESTO_ESTADO = "Activo" // Estado por defecto
            };

            //IMPORTANTE: Asignar la lista de áreas
            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE");

            ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal
            return View(nuevoPuesto);
            //ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE");
            //return View();
        }

        // POST: Puestos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PUESTO_ID,PUESTO_NOMBRE,PUESTO_DESCRIPCION,PUESTO_ESTADO,AREA_ID")] PUESTO puesto)
        {
            if (ModelState.IsValid)
            {
                db.PUESTO.Add(puesto);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            //IMPORTANTE: Asignar la lista de áreas
            ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE");

            return View(puesto);
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

            ViewBag.Editado = TempData["Editado"];
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

                TempData["Editado"] = true; // <-- se activa el modal
                return RedirectToAction("Edit", new { id = pUESTO.PUESTO_ID}); // redirige a la misma vista con el id
                //return RedirectToAction("Index");
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

            ViewBag.Desactivado = TempData["Desactivado"]; //se pasa a la vista
            return View(pUESTO);
        }

        // POST: Puestos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PUESTO area = db.PUESTO.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }

            // Eliminación lógica: cambiar estado a "Inactivo"
            area.PUESTO_ESTADO = "Inactivo";
            db.Entry(area).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true; // 👈 activa el modal
            return RedirectToAction("Delete", new { id = area.PUESTO_ID }); // redirige a la misma vista
            //PUESTO pUESTO = db.PUESTO.Find(id);
            //db.PUESTO.Remove(pUESTO);
            //db.SaveChanges();
            //return RedirectToAction("Index");
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
