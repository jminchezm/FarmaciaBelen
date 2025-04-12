using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class FormaFarmaceuticaController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: FormaFarmaceutica
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var formaFa = db.FORMAFARMACEUTICA.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                formaFa = formaFa.Where(f => f.FORMAFARMACEUTICA_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                formaFa = formaFa.Where(f => f.FARMAFARMACEUTICA_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                formaFa = formaFa.Where(f => f.FORMAFARMACEUTICA_ESTADO == estado);

            return View(formaFa.ToList());
        }

        // GET: FormaFarmaceutica/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FORMAFARMACEUTICA fORMAFARMACEUTICA = db.FORMAFARMACEUTICA.Find(id);
            if (fORMAFARMACEUTICA == null)
            {
                return HttpNotFound();
            }
            return View(fORMAFARMACEUTICA);
        }

        // GET: FormaFarmaceutica/Create
        public ActionResult Create()
        {
            {
                var ultimaForma = db.FORMAFARMACEUTICA
                    .OrderByDescending(f => f.FORMAFARMACEUTICA_ID)
                    .FirstOrDefault();

                string nuevoCodigo;

                if (ultimaForma != null && ultimaForma.FORMAFARMACEUTICA_ID.Length == 10)
                {
                    string numero = ultimaForma.FORMAFARMACEUTICA_ID.Substring(4);
                    int siguiente = int.Parse(numero) + 1;
                    nuevoCodigo = "FORF" + siguiente.ToString("D6");
                }
                else
                {
                    nuevoCodigo = "FORF000001";
                }

                var nuevaFormula = new FORMAFARMACEUTICA
                {
                    FORMAFARMACEUTICA_ID = nuevoCodigo
                    //FORMAFARMACEUTICA_ESTADO = "Activo"
                };

                ViewBag.Creado = TempData["Creado"];
                return View(nuevaFormula);
            }
        }

        // POST: FormaFarmaceutica/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FORMAFARMACEUTICA_ID,FARMAFARMACEUTICA_NOMBRE,FORMAFARMACEUTICA_DESCRIPCION,FORMAFARMACEUTICA_ESTADO")] FORMAFARMACEUTICA formafarmaceutica)
        {

            if (!string.IsNullOrWhiteSpace(formafarmaceutica.FARMAFARMACEUTICA_NOMBRE))
            {
                bool nombreExistente = db.FORMAFARMACEUTICA.Any(r => r.FARMAFARMACEUTICA_NOMBRE == formafarmaceutica.FARMAFARMACEUTICA_NOMBRE);
                if (nombreExistente)
                    ModelState.AddModelError("FARMAFARMACEUTICA_NOMBRE", "Este nombre ya está en uso.");
            }




            if (ModelState.IsValid)
            {
                db.FORMAFARMACEUTICA.Add(formafarmaceutica);
                db.SaveChanges();
                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            return View(formafarmaceutica);
        }

        // GET: FormaFarmaceutica/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FORMAFARMACEUTICA fORMAFARMACEUTICA = db.FORMAFARMACEUTICA.Find(id);


            if (fORMAFARMACEUTICA == null)
            {
                return HttpNotFound();
            }
            ViewBag.Editado = TempData["Editado"];
            return View(fORMAFARMACEUTICA);
        }

        // POST: FormaFarmaceutica/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FORMAFARMACEUTICA_ID,FARMAFARMACEUTICA_NOMBRE,FORMAFARMACEUTICA_DESCRIPCION,FORMAFARMACEUTICA_ESTADO")] FORMAFARMACEUTICA fORMAFARMACEUTICA)
        {


            // Validar correo duplicado solo si se ingresó algo
            if (!string.IsNullOrWhiteSpace(fORMAFARMACEUTICA.FARMAFARMACEUTICA_NOMBRE))
            {
                bool correoExistente = db.FORMAFARMACEUTICA.Any(r =>
                    r.FARMAFARMACEUTICA_NOMBRE == fORMAFARMACEUTICA.FARMAFARMACEUTICA_NOMBRE &&
                    r.FORMAFARMACEUTICA_ID != fORMAFARMACEUTICA.FORMAFARMACEUTICA_ID
                );

                if (correoExistente)
                    ModelState.AddModelError("FARMAFARMACEUTICA_NOMBRE", "Este nombre ya está en uso.");
            }



            if (ModelState.IsValid)
            {
                db.Entry(fORMAFARMACEUTICA).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Editado"] = true; // <-- se activa el modal
                return RedirectToAction("Edit", new { id = fORMAFARMACEUTICA.FORMAFARMACEUTICA_ID }); // redirige a la misma vista con el id
                //return RedirectToAction("Index");
            }
            return View(fORMAFARMACEUTICA);
        }

        // GET: FormaFarmaceutica/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FORMAFARMACEUTICA formaFarmaceutica = db.FORMAFARMACEUTICA.Find(id);
            if (formaFarmaceutica == null)
            {
                return HttpNotFound();
            }
            ViewBag.Desactivado = TempData["Desactivado"]; //se pasa a la vista
            return View(formaFarmaceutica);
        }

        // POST: FormaFarmaceutica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            FORMAFARMACEUTICA formaFarmaceutica = db.FORMAFARMACEUTICA.Find(id);
            if (formaFarmaceutica == null)
            {
                return HttpNotFound();
            }

            // Eliminación lógica: cambiar estado a "Inactivo"
            formaFarmaceutica.FORMAFARMACEUTICA_ESTADO = "Inactivo";
            db.Entry(formaFarmaceutica).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true; // activa el modal
            return RedirectToAction("Delete", new { id = formaFarmaceutica.FORMAFARMACEUTICA_ID }); // redirige a la misma vista
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
