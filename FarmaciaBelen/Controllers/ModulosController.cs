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
    public class ModulosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Modulos
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var modulo = db.MODULO.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                modulo = modulo.Where(m => m.MODULO_ID.Contains(codigo)); // ← usa Contains para búsquedas parciales

            if (!string.IsNullOrEmpty(nombre))
                modulo = modulo.Where(m => m.MODULO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                modulo = modulo.Where(m => m.MODULO_ESTADO == estado);

            return View(modulo.ToList());
        }
        //public ActionResult Index()
        //{
        //    return View(db.MODULO.ToList());
        //}

        // GET: Modulos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MODULO mODULO = db.MODULO.Find(id);
            if (mODULO == null)
            {
                return HttpNotFound();
            }
            return View(mODULO);
        }

        // GET: Modulos/Create
        public ActionResult Create()
        {
            // Obtener el último código existente
            var ultimoModulo = db.MODULO
                .OrderByDescending(m => m.MODULO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoModulo != null && ultimoModulo.MODULO_ID.Length == 10)
            {
                // Extraer el número del código (últimos 6 caracteres)
                string numero = ultimoModulo.MODULO_ID.Substring(4); // "000004"
                int siguienteNumero = int.Parse(numero) + 1;

                // Formatear el nuevo código con ceros y prefijo
                nuevoCodigo = "MODU" + siguienteNumero.ToString("D6");
            }
            else
            {
                // Si no hay registros o el formato es incorrecto, empezar con el primero
                nuevoCodigo = "MODU000001";
            }

            var nuevoModulo = new MODULO
            {
                MODULO_ID = nuevoCodigo,
                /*AREA_ESTADO = "Activo"*/ // Estado por defecto
            };

            ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal
            return View(nuevoModulo);
        }
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: Modulos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MODULO_ID,MODULO_NOMBRE,MODULO_DESCRIPCION,MODULO_ESTADO")] MODULO mODULO)
        {
            if (ModelState.IsValid)
            {
                db.MODULO.Add(mODULO);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            return View(mODULO);
            //if (ModelState.IsValid)
            //{
            //    db.MODULO.Add(mODULO);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //return View(mODULO);
        }

        // GET: Modulos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MODULO mODULO = db.MODULO.Find(id);
            if (mODULO == null)
            {
                return HttpNotFound();
            }
            return View(mODULO);
        }

        // POST: Modulos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MODULO_ID,MODULO_NOMBRE,MODULO_DESCRIPCION,MODULO_ESTADO")] MODULO mODULO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mODULO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mODULO);
        }

        // GET: Modulos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MODULO modulo = db.MODULO.Find(id);
            if (modulo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Desactivado = TempData["Desactivado"]; //se pasa a la vista
            return View(modulo);
        }
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MODULO mODULO = db.MODULO.Find(id);
        //    if (mODULO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mODULO);
        //}

        // POST: Modulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MODULO modulo = db.MODULO.Find(id);
            if (modulo == null)
            {
                return HttpNotFound();
            }

            // Eliminación lógica: cambiar estado a "Inactivo"
            modulo.MODULO_ESTADO = "Inactivo";
            db.Entry(modulo).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true; // 👈 activa el modal
            return RedirectToAction("Delete", new { id = modulo.MODULO_ID }); // redirige a la misma vista

            //MODULO mODULO = db.MODULO.Find(id);
            //db.MODULO.Remove(mODULO);
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
