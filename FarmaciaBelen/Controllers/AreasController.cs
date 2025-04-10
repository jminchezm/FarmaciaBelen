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
            // Obtener el último código existente
            var ultimaArea = db.AREA
                .OrderByDescending(a => a.AREA_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimaArea != null && ultimaArea.AREA_ID.Length == 10)
            {
                // Extraer el número del código (últimos 6 caracteres)
                string numero = ultimaArea.AREA_ID.Substring(4); // "000004"
                int siguienteNumero = int.Parse(numero) + 1;

                // Formatear el nuevo código con ceros y prefijo
                nuevoCodigo = "AREA" + siguienteNumero.ToString("D6");
            }
            else
            {
                // Si no hay registros o el formato es incorrecto, empezar con el primero
                nuevoCodigo = "AREA000001";
            }

            var nuevaArea = new AREA
            {
                AREA_ID = nuevoCodigo,
                /*AREA_ESTADO = "Activo"*/ // Estado por defecto
            };

            ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal
            return View(nuevaArea);
        }

        // POST: Areas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AREA_ID,AREA_NOMBRE,AREA_DESCRIPCION,AREA_EXTENSION,AREA_CORREO,AREA_ESTADO")] AREA aREA)
        {
            // Validar correo duplicado, solo si se ingresó algo.
            if (!string.IsNullOrWhiteSpace(aREA.AREA_CORREO))
            {
                bool correoExistente = db.AREA.Any(a => a.AREA_CORREO == aREA.AREA_CORREO);
                if (correoExistente)
                    ModelState.AddModelError("AREA_CORREO", "Este correo ya está en uso.");
            }

            // Validar extensión duplicada
            bool extensionExistente = db.AREA.Any(a => a.AREA_EXTENSION == aREA.AREA_EXTENSION);
            if (extensionExistente)
                ModelState.AddModelError("AREA_EXTENSION", "Esta extensión ya está en uso.");

            if (ModelState.IsValid)
            {
                db.AREA.Add(aREA);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
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

            ViewBag.Editado = TempData["Editado"]; // para mostrar el modal si se acaba de editar

            return View(aREA);
        }

        // POST: Areas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AREA_ID,AREA_NOMBRE,AREA_DESCRIPCION,AREA_EXTENSION,AREA_CORREO,AREA_ESTADO")] AREA aREA)
        {
            // Validar correo duplicado solo si se ingresó algo
            if (!string.IsNullOrWhiteSpace(aREA.AREA_CORREO))
            {
                bool correoExistente = db.AREA.Any(a =>
                    a.AREA_CORREO == aREA.AREA_CORREO &&
                    a.AREA_ID != aREA.AREA_ID
                );

                if (correoExistente)
                    ModelState.AddModelError("AREA_CORREO", "Este correo ya está en uso.");
            }

            // Validar extensión duplicada, excluyendo el mismo área
            bool extensionExistente = db.AREA.Any(a =>
                a.AREA_EXTENSION == aREA.AREA_EXTENSION &&
                a.AREA_ID != aREA.AREA_ID
            );

            if (extensionExistente)
                ModelState.AddModelError("AREA_EXTENSION", "Esta extensión ya está en uso.");

            if (ModelState.IsValid)
            {
                db.Entry(aREA).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Editado"] = true; // <-- se activa el modal
                return RedirectToAction("Edit", new { id = aREA.AREA_ID }); // redirige a la misma vista con el id
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
            ViewBag.Desactivado = TempData["Desactivado"]; //se pasa a la vista
            return View(aREA);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AREA aREA = db.AREA.Find(id);
            if (aREA == null)
            {
                return HttpNotFound();
            }

            // Eliminación lógica: cambiar estado a "Inactivo"
            aREA.AREA_ESTADO = "Inactivo";
            db.Entry(aREA).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true; // activa el modal
            return RedirectToAction("Delete", new { id = aREA.AREA_ID }); // redirige a la misma vista
        }

        // POST: Areas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    AREA aREA = db.AREA.Find(id);
        //    db.AREA.Remove(aREA);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
