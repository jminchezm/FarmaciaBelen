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
    public class SubCategoriaProductosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: SubCategoriaProductos
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var subcategorias = db.SUBCATEGORIAPRODUCTO.Include(s => s.CATEGORIAPRODUCTO).AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                subcategorias = subcategorias.Where(s => s.SUBCATEGORIAPRODUCTO_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                subcategorias = subcategorias.Where(s => s.SUBCATEGORIAPRODUCTO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                subcategorias = subcategorias.Where(s => s.SUBCATEGORIAPRODUCTO_ESTADO == estado);

            return View(subcategorias.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SUBCATEGORIAPRODUCTO sCAT = db.SUBCATEGORIAPRODUCTO.Find(id);
            if (sCAT == null)
            {
                return HttpNotFound();
            }
            return View(sCAT);
        }

        // GET: SubCategoriaProductos/Create
        public ActionResult Create()
        {
            // Obtener el último ID
            var ultima = db.SUBCATEGORIAPRODUCTO
                .OrderByDescending(s => s.SUBCATEGORIAPRODUCTO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultima != null && ultima.SUBCATEGORIAPRODUCTO_ID.Length == 10)
            {
                string numero = ultima.SUBCATEGORIAPRODUCTO_ID.Substring(4); // "000001"
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "SUBC" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "SUBC000001";
            }

            var nuevaSub = new SUBCATEGORIAPRODUCTO
            {
                SUBCATEGORIAPRODUCTO_ID = nuevoCodigo
                /*SUBCATEGORIAPRODUCTO_ESTADO = "Activo"*/
            };

            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO.Where(c => c.CATEGORIAPRODUCTO_ESTADO == "Activo"), "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE");
            ViewBag.Creado = TempData["Creado"];

            return View(nuevaSub);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SUBCATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_NOMBRE,SUBCATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_ESTADO")] SUBCATEGORIAPRODUCTO s)
        {
            if (ModelState.IsValid)
            {
                db.SUBCATEGORIAPRODUCTO.Add(s);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO.Where(c => c.CATEGORIAPRODUCTO_ESTADO == "Activo"), "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", s.CATEGORIAPRODUCTO_ID);
            return View(s);
        }

        // GET: SubCategoriaProductos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sub = db.SUBCATEGORIAPRODUCTO.Find(id);
            if (sub == null) return HttpNotFound();

            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO.Where(c => c.CATEGORIAPRODUCTO_ESTADO == "Activo"), "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", sub.CATEGORIAPRODUCTO_ID);
            ViewBag.Editado = TempData["Editado"];

            return View(sub);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SUBCATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_NOMBRE,SUBCATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_ESTADO")] SUBCATEGORIAPRODUCTO s)
        {
            if (ModelState.IsValid)
            {
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = s.SUBCATEGORIAPRODUCTO_ID });
            }

            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO.Where(c => c.CATEGORIAPRODUCTO_ESTADO == "Activo"), "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", s.CATEGORIAPRODUCTO_ID);
            return View(s);
        }

        // GET: SubCategoriaProductos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sub = db.SUBCATEGORIAPRODUCTO.Find(id);
            if (sub == null) return HttpNotFound();

            ViewBag.Desactivado = TempData["Desactivado"];
            return View(sub);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var sub = db.SUBCATEGORIAPRODUCTO.Find(id);
            if (sub == null) return HttpNotFound();

            sub.SUBCATEGORIAPRODUCTO_ESTADO = "Inactivo";
            db.Entry(sub).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true;
            return RedirectToAction("Delete", new { id = sub.SUBCATEGORIAPRODUCTO_ID });
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







































//namespace FarmaciaBelen.Controllers
//{
//    public class SubCategoriaProductosController : Controller
//    {
//        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

//        // GET: SubCategoriaProductos
//        public ActionResult Index()
//        {
//            var sUBCATEGORIAPRODUCTO = db.SUBCATEGORIAPRODUCTO.Include(s => s.CATEGORIAPRODUCTO);
//            return View(sUBCATEGORIAPRODUCTO.ToList());
//        }

//        // GET: SubCategoriaProductos/Details/5
//        public ActionResult Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO = db.SUBCATEGORIAPRODUCTO.Find(id);
//            if (sUBCATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(sUBCATEGORIAPRODUCTO);
//        }

//        // GET: SubCategoriaProductos/Create
//        public ActionResult Create()
//        {
//            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO, "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE");
//            return View();
//        }

//        // POST: SubCategoriaProductos/Create
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
//        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "SUBCATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_NOMBRE,SUBCATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_ESTADO")] SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO)
//        {
//            if (ModelState.IsValid)
//            {
//                db.SUBCATEGORIAPRODUCTO.Add(sUBCATEGORIAPRODUCTO);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO, "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", sUBCATEGORIAPRODUCTO.CATEGORIAPRODUCTO_ID);
//            return View(sUBCATEGORIAPRODUCTO);
//        }

//        // GET: SubCategoriaProductos/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO = db.SUBCATEGORIAPRODUCTO.Find(id);
//            if (sUBCATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO, "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", sUBCATEGORIAPRODUCTO.CATEGORIAPRODUCTO_ID);
//            return View(sUBCATEGORIAPRODUCTO);
//        }

//        // POST: SubCategoriaProductos/Edit/5
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
//        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "SUBCATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_NOMBRE,SUBCATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ID,SUBCATEGORIAPRODUCTO_ESTADO")] SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(sUBCATEGORIAPRODUCTO).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.CATEGORIAPRODUCTO_ID = new SelectList(db.CATEGORIAPRODUCTO, "CATEGORIAPRODUCTO_ID", "CATEGORIAPRODUCTO_NOMBRE", sUBCATEGORIAPRODUCTO.CATEGORIAPRODUCTO_ID);
//            return View(sUBCATEGORIAPRODUCTO);
//        }

//        // GET: SubCategoriaProductos/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO = db.SUBCATEGORIAPRODUCTO.Find(id);
//            if (sUBCATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(sUBCATEGORIAPRODUCTO);
//        }

//        // POST: SubCategoriaProductos/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            SUBCATEGORIAPRODUCTO sUBCATEGORIAPRODUCTO = db.SUBCATEGORIAPRODUCTO.Find(id);
//            db.SUBCATEGORIAPRODUCTO.Remove(sUBCATEGORIAPRODUCTO);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
