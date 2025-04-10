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
    public class CategoriaProductosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: CategoriaProducto
        public ActionResult Index(string codigo, string nombre, string estado)
        {
            var categorias = db.CATEGORIAPRODUCTO.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                categorias = categorias.Where(c => c.CATEGORIAPRODUCTO_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                categorias = categorias.Where(c => c.CATEGORIAPRODUCTO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                categorias = categorias.Where(c => c.CATEGORIAPRODUCTO_ESTADO == estado);

            return View(categorias.ToList());
        }

        // GET: CategoriaProducto/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.CATEGORIAPRODUCTO.Find(id);
            if (categoria == null)
                return HttpNotFound();

            return View(categoria);
        }

        // GET: CategoriaProducto/Create
        public ActionResult Create()
        {
            var ultimaCategoria = db.CATEGORIAPRODUCTO
                .OrderByDescending(c => c.CATEGORIAPRODUCTO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimaCategoria != null && ultimaCategoria.CATEGORIAPRODUCTO_ID.Length == 10)
            {
                string numero = ultimaCategoria.CATEGORIAPRODUCTO_ID.Substring(4);
                int siguiente = int.Parse(numero) + 1;
                nuevoCodigo = "CATP" + siguiente.ToString("D6");
            }
            else
            {
                nuevoCodigo = "CATP000001";
            }

            var nuevaCategoria = new CATEGORIAPRODUCTO
            {
                CATEGORIAPRODUCTO_ID = nuevoCodigo
                //CATEGORIAPRODUCTO_ESTADO = "Activo"
            };

            ViewBag.Creado = TempData["Creado"];
            return View(nuevaCategoria);
        }

        // POST: CategoriaProducto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CATEGORIAPRODUCTO_ID,CATEGORIAPRODUCTO_NOMBRE,CATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ESTADO")] CATEGORIAPRODUCTO categoria)
        {
            if (!string.IsNullOrWhiteSpace(categoria.CATEGORIAPRODUCTO_NOMBRE))
            {
                bool existe = db.CATEGORIAPRODUCTO.Any(c => c.CATEGORIAPRODUCTO_NOMBRE == categoria.CATEGORIAPRODUCTO_NOMBRE);
                if (existe)
                    ModelState.AddModelError("CATEGORIAPRODUCTO_NOMBRE", "Este nombre ya está en uso.");
            }



            if (ModelState.IsValid)
            {
                db.CATEGORIAPRODUCTO.Add(categoria);
                db.SaveChanges();
                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            return View(categoria);
        }

        // GET: CategoriaProducto/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.CATEGORIAPRODUCTO.Find(id);
            if (categoria == null)
                return HttpNotFound();

            ViewBag.Editado = TempData["Editado"];
            return View(categoria);
        }

        // POST: CategoriaProducto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CATEGORIAPRODUCTO_ID,CATEGORIAPRODUCTO_NOMBRE,CATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ESTADO")] CATEGORIAPRODUCTO categoria)
        {
            if (!string.IsNullOrWhiteSpace(categoria.CATEGORIAPRODUCTO_NOMBRE))
            {
                bool existe = db.CATEGORIAPRODUCTO.Any(c =>
                    c.CATEGORIAPRODUCTO_NOMBRE == categoria.CATEGORIAPRODUCTO_NOMBRE &&
                    c.CATEGORIAPRODUCTO_ID != categoria.CATEGORIAPRODUCTO_ID
                );

                if (existe)
                    ModelState.AddModelError("CATEGORIAPRODUCTO_NOMBRE", "Este nombre ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(categoria).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = categoria.CATEGORIAPRODUCTO_ID });
            }

            return View(categoria);
        }

        // GET: CategoriaProducto/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var categoria = db.CATEGORIAPRODUCTO.Find(id);
            if (categoria == null)
                return HttpNotFound();

            ViewBag.Desactivado = TempData["Desactivado"];
            return View(categoria);
        }

        // POST: CategoriaProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var categoria = db.CATEGORIAPRODUCTO.Find(id);
            if (categoria == null)
                return HttpNotFound();

            categoria.CATEGORIAPRODUCTO_ESTADO = "Inactivo";
            db.Entry(categoria).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true;
            return RedirectToAction("Delete", new { id = categoria.CATEGORIAPRODUCTO_ID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}


























//namespace FarmaciaBelen.Controllers
//{
//    public class CategoriaProductoController : Controller
//    {
//        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

//        // GET: CategoriaProducto
//        public ActionResult Index()
//        {
//            return View(db.CATEGORIAPRODUCTO.ToList());
//        }

//        // GET: CategoriaProducto/Details/5
//        public ActionResult Details(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            CATEGORIAPRODUCTO cATEGORIAPRODUCTO = db.CATEGORIAPRODUCTO.Find(id);
//            if (cATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(cATEGORIAPRODUCTO);
//        }

//        // GET: CategoriaProducto/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: CategoriaProducto/Create
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
//        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "CATEGORIAPRODUCTO_ID,CATEGORIAPRODUCTO_NOMBRE,CATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ESTADO")] CATEGORIAPRODUCTO cATEGORIAPRODUCTO)
//        {
//            if (ModelState.IsValid)
//            {
//                db.CATEGORIAPRODUCTO.Add(cATEGORIAPRODUCTO);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(cATEGORIAPRODUCTO);
//        }

//        // GET: CategoriaProducto/Edit/5
//        public ActionResult Edit(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            CATEGORIAPRODUCTO cATEGORIAPRODUCTO = db.CATEGORIAPRODUCTO.Find(id);
//            if (cATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(cATEGORIAPRODUCTO);
//        }

//        // POST: CategoriaProducto/Edit/5
//        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
//        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "CATEGORIAPRODUCTO_ID,CATEGORIAPRODUCTO_NOMBRE,CATEGORIAPRODUCTO_DESCRIPCION,CATEGORIAPRODUCTO_ESTADO")] CATEGORIAPRODUCTO cATEGORIAPRODUCTO)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(cATEGORIAPRODUCTO).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(cATEGORIAPRODUCTO);
//        }

//        // GET: CategoriaProducto/Delete/5
//        public ActionResult Delete(string id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            CATEGORIAPRODUCTO cATEGORIAPRODUCTO = db.CATEGORIAPRODUCTO.Find(id);
//            if (cATEGORIAPRODUCTO == null)
//            {
//                return HttpNotFound();
//            }
//            return View(cATEGORIAPRODUCTO);
//        }

//        // POST: CategoriaProducto/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            CATEGORIAPRODUCTO cATEGORIAPRODUCTO = db.CATEGORIAPRODUCTO.Find(id);
//            db.CATEGORIAPRODUCTO.Remove(cATEGORIAPRODUCTO);
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
