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
    public class RolesController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Roles
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var roles = db.ROL.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                roles = roles.Where(a => a.ROL_ID.Contains(codigo)); // ← usa Contains para búsquedas parciales

            if (!string.IsNullOrEmpty(nombre))
                roles = roles.Where(a => a.ROL_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                roles = roles.Where(a => a.ROL_ESTADO == estado);

            return View(roles.ToList());
            //return View(db.ROL.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROL rOL = db.ROL.Find(id);
            if (rOL == null)
            {
                return HttpNotFound();
            }
            return View(rOL);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            // Obtener el último código existente
            var ultimoRol = db.ROL
                .OrderByDescending(r => r.ROL_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoRol != null && ultimoRol.ROL_ID.Length == 9)
            {
                // Extraer el número del código (últimos 6 caracteres)
                string numero = ultimoRol.ROL_ID.Substring(4); // "0000004"
                int siguienteNumero = int.Parse(numero) + 1;

                // Formatear el nuevo código con ceros y prefijo
                nuevoCodigo = "ROL" + siguienteNumero.ToString("D6");
            }
            else
            {
                // Si no hay registros o el formato es incorrecto, empezar con el primero
                nuevoCodigo = "ROL000001";
            }

            var nuevoPuesto = new ROL
            {
                ROL_ID = nuevoCodigo,
                ROL_ESTADO = "Activo" // Estado por defecto
            };

            //IMPORTANTE: Asignar la lista de áreas
            //ViewBag.AREA_ID = new SelectList(db.AREA, "AREA_ID", "AREA_NOMBRE");

            ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal
            return View(nuevoPuesto);
            //return View();
        }

        // POST: Roles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ROL_ID,ROL_NOMBRE,ROL_DESCRIPCION,ROL_ESTADO")] ROL rol)
        {
            // Validar correo duplicado, solo si se ingresó algo.
            if (!string.IsNullOrWhiteSpace(rol.ROL_NOMBRE))
            {
                bool nombreExistente = db.ROL.Any(r => r.ROL_NOMBRE== rol.ROL_NOMBRE);
                if (nombreExistente)
                    ModelState.AddModelError("ROL_NOMBRE", "Este nombre ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                db.ROL.Add(rol);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
                //return RedirectToAction("Index");
            }

            return View(rol);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROL rOL = db.ROL.Find(id);
            if (rOL == null)
            {
                return HttpNotFound();
            }
            ViewBag.Editado = TempData["Editado"];
            return View(rOL);
            //return View(rOL);
        }

        // POST: Roles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ROL_ID,ROL_NOMBRE,ROL_DESCRIPCION,ROL_ESTADO")] ROL rOL)
        {
            // Validar correo duplicado solo si se ingresó algo
            if (!string.IsNullOrWhiteSpace(rOL.ROL_NOMBRE))
            {
                bool correoExistente = db.ROL.Any(r =>
                    r.ROL_NOMBRE== rOL.ROL_NOMBRE&&
                    r.ROL_ID!= rOL.ROL_ID
                );

                if (correoExistente)
                    ModelState.AddModelError("ROL_NOMBRE", "Este nombre ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(rOL).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Editado"] = true; // <-- se activa el modal
                return RedirectToAction("Edit", new { id = rOL.ROL_ID}); // redirige a la misma vista con el id
                //return RedirectToAction("Index");
            }
            return View(rOL);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ROL rOL = db.ROL.Find(id);
            if (rOL == null)
            {
                return HttpNotFound();
            }

            ViewBag.Desactivado = TempData["Desactivado"]; //se pasa a la vista
            return View(rOL);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            ROL rol= db.ROL.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }

            // Eliminación lógica: cambiar estado a "Inactivo"
            rol.ROL_ESTADO = "Inactivo";
            db.Entry(rol).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true; // 👈 activa el modal
            return RedirectToAction("Delete", new { id = rol.ROL_ID }); // redirige a la misma vista

            //ROL rOL = db.ROL.Find(id);
            //db.ROL.Remove(rOL);
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
