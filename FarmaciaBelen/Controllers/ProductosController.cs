using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class ProductosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Productos
        public ActionResult Index(string codigo, string nombre, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var productos = db.PRODUCTO
                .Include(p => p.FORMAFARMACEUTICA)
                .Include(p => p.SUBCATEGORIAPRODUCTO)
                .Include(p => p.VIAADMINISTRACION)
                .AsQueryable();
            //var producto = db.PRODUCTO.AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                productos = productos.Where(p => p.PRODUCTO_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
                productos = productos.Where(p => p.PRODUCTO_NOMBRE.Contains(nombre));

            if (!string.IsNullOrEmpty(estado))
                productos = productos.Where(p => p.PRODUCTO_ESTADO.Contains(estado));

            if (fechaInicio.HasValue)
                productos = productos.Where(p => p.PRODUCTO_FECHACREACION >= fechaInicio.Value);

            if (fechaFin.HasValue)
                productos = productos.Where(p => p.PRODUCTO_FECHACREACION <= fechaFin.Value);

            ViewBag.Productos = db.PRODUCTO
                .Select(r => new SelectListItem
                {
                    Value = r.PRODUCTO_ID,
                    Text = r.PRODUCTO_NOMBRE
                }).ToList();

            return View(productos.ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCTO);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            ViewBag.FORMAFARMACEUTICA_ID = new SelectList(db.FORMAFARMACEUTICA.Where(f => f.FORMAFARMACEUTICA_ESTADO == "Activo"), "FORMAFARMACEUTICA_ID", "FARMAFARMACEUTICA_NOMBRE");
            ViewBag.SUBCATEGORIAPRODUCTO_ID = new SelectList(db.SUBCATEGORIAPRODUCTO.Where(sc => sc.SUBCATEGORIAPRODUCTO_ESTADO == "Activo"), "SUBCATEGORIAPRODUCTO_ID", "SUBCATEGORIAPRODUCTO_NOMBRE");
            ViewBag.VIAADMINISTRACION_ID = new SelectList(db.VIAADMINISTRACION.Where(va => va.VIAADMINISTRACION_ESTADO == "Activo"), "VIAADMINISTRACION_ID", "VIAADMINISTRATIVA_NOMBRE");

            var ultimoproducto = db.PRODUCTO
              .OrderByDescending(p => p.PRODUCTO_ID)
              .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoproducto != null && ultimoproducto.PRODUCTO_ID.Length == 10)
            {
                string numero = ultimoproducto.PRODUCTO_ID.Substring(4);
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "PROD" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "PROD000001";
            }

            var nuvoProducto = new PRODUCTO
            {
                PRODUCTO_ID = nuevoCodigo
                //PROVEEDOR_ESTADO = "Activo"
            };

            ViewBag.Creado = TempData["Creado"];
            return View(nuvoProducto);


        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PRODUCTO_ID,PRODUCTO_NOMBRE,PRODUCTO_DESCRIPCION,FORMAFARMACEUTICA_ID,VIAADMINISTRACION_ID,PRODUCTO_CASAMEDICA,PRODUCTO_ESTADO,SUBCATEGORIAPRODUCTO_ID")] PRODUCTO producto, HttpPostedFileBase imagen)
        {
            if (!string.IsNullOrWhiteSpace(producto.PRODUCTO_NOMBRE))
            {
                bool existe = db.PRODUCTO.Any(p => p.PRODUCTO_NOMBRE == producto.PRODUCTO_NOMBRE);
                if (existe)
                    ModelState.AddModelError("PRODUCTO_NOMBRE", "Este nombre ya está en uso.");
            }

            producto.PRODUCTO_FECHACREACION = DateTime.Today;

            // Subida de imagen
            if (imagen != null && imagen.ContentLength > 0)
            {
                string extension = Path.GetExtension(imagen.FileName).ToLower();
                string[] permitidas = { ".jpg", ".jpeg", ".png", ".gif" };

                if (permitidas.Contains(extension))
                {
                    string nombreUnico = Guid.NewGuid().ToString() + extension;
                    string rutaCarpeta = Server.MapPath("~/Content/ImagenesProductos");
                    string rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

                    if (!Directory.Exists(rutaCarpeta))
                        Directory.CreateDirectory(rutaCarpeta);

                    imagen.SaveAs(rutaCompleta);
                    producto.PRODUCTO_IMG = "/Content/ImagenesProductos/" + nombreUnico;
                }
                else
                {
                    producto.PRODUCTO_IMG = "~/Content/ImagenesProductos/default.png";
                    ModelState.AddModelError("imagen", "Solo se permiten imágenes .jpg, .jpeg, .png, .gif.");

                    return View(producto);
                }
            }

            if (ModelState.IsValid)
            {
                db.PRODUCTO.Add(producto);
                db.SaveChanges();
                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            ViewBag.FORMAFARMACEUTICA_ID = new SelectList(db.FORMAFARMACEUTICA.Where(f => f.FORMAFARMACEUTICA_ESTADO == "Activo"), "FORMAFARMACEUTICA_ID", "FARMAFARMACEUTICA_NOMBRE", producto.FORMAFARMACEUTICA_ID);
            ViewBag.SUBCATEGORIAPRODUCTO_ID = new SelectList(db.SUBCATEGORIAPRODUCTO.Where(sc => sc.SUBCATEGORIAPRODUCTO_ESTADO == "Activo"), "SUBCATEGORIAPRODUCTO_ID", "SUBCATEGORIAPRODUCTO_NOMBRE", producto.SUBCATEGORIAPRODUCTO_ID);
            ViewBag.VIAADMINISTRACION_ID = new SelectList(db.VIAADMINISTRACION.Where(va => va.VIAADMINISTRACION_ESTADO == "Activo"), "VIAADMINISTRACION_ID", "VIAADMINISTRATIVA_NOMBRE", producto.VIAADMINISTRACION_ID);
            return View(producto);
        }


        // GET: Productos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.FORMAFARMACEUTICA_ID = new SelectList(db.FORMAFARMACEUTICA.Where(f => f.FORMAFARMACEUTICA_ESTADO == "Activo"), "FORMAFARMACEUTICA_ID", "FARMAFARMACEUTICA_NOMBRE", pRODUCTO.FORMAFARMACEUTICA_ID);
            ViewBag.SUBCATEGORIAPRODUCTO_ID = new SelectList(db.SUBCATEGORIAPRODUCTO.Where(sc => sc.SUBCATEGORIAPRODUCTO_ESTADO == "Activo"), "SUBCATEGORIAPRODUCTO_ID", "SUBCATEGORIAPRODUCTO_NOMBRE", pRODUCTO.SUBCATEGORIAPRODUCTO_ID);
            ViewBag.VIAADMINISTRACION_ID = new SelectList(db.VIAADMINISTRACION.Where(va => va.VIAADMINISTRACION_ESTADO == "Activo"), "VIAADMINISTRACION_ID", "VIAADMINISTRATIVA_NOMBRE", pRODUCTO.VIAADMINISTRACION_ID);

            ViewBag.Editado = TempData["Editado"];
            return View(pRODUCTO);
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PRODUCTO_ID,PRODUCTO_NOMBRE,PRODUCTO_DESCRIPCION,FORMAFARMACEUTICA_ID,VIAADMINISTRACION_ID,PRODUCTO_CASAMEDICA,PRODUCTO_IMG,PRODUCTO_ESTADO,SUBCATEGORIAPRODUCTO_ID")] PRODUCTO producto, HttpPostedFileBase imagen)
        {

            var productoOriginal = db.PRODUCTO.AsNoTracking().FirstOrDefault(p => p.PRODUCTO_ID == producto.PRODUCTO_ID);

            producto.PRODUCTO_FECHACREACION = DateTime.Today;


            if (imagen != null && imagen.ContentLength > 0)
            {
                string nombreArchivo = Path.GetFileName(imagen.FileName);
                string ruta = Path.Combine(Server.MapPath("~/Content/ImagenesProductos"), nombreArchivo);
                imagen.SaveAs(ruta);
                producto.PRODUCTO_IMG = "/Content/ImagenesProductos/" + nombreArchivo;
            }
            else
            {
                // Conservar imagen existente
                producto.PRODUCTO_IMG = productoOriginal.PRODUCTO_IMG;
            }

            productoOriginal.PRODUCTO_NOMBRE = producto.PRODUCTO_NOMBRE;
            productoOriginal.PRODUCTO_DESCRIPCION = producto.PRODUCTO_DESCRIPCION;
            productoOriginal.PRODUCTO_CASAMEDICA = producto.PRODUCTO_CASAMEDICA;
            productoOriginal.PRODUCTO_IMG = producto.PRODUCTO_IMG;
            productoOriginal.PRODUCTO_ESTADO = producto.PRODUCTO_ESTADO;
            productoOriginal.FORMAFARMACEUTICA_ID = producto.FORMAFARMACEUTICA_ID;
            productoOriginal.SUBCATEGORIAPRODUCTO_ID = producto.SUBCATEGORIAPRODUCTO_ID;
            productoOriginal.VIAADMINISTRACION_ID = producto.VIAADMINISTRACION_ID;


            if (!string.IsNullOrWhiteSpace(producto.PRODUCTO_NOMBRE))
            {
                bool existe = db.PRODUCTO.Any(p =>
                    p.PRODUCTO_NOMBRE == producto.PRODUCTO_NOMBRE &&
                    p.PRODUCTO_ID != producto.PRODUCTO_ID
                );

                if (existe)
                    ModelState.AddModelError("PRODUCTO_NOMBRE", "Este nombre ya está en uso.");
            }


            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = producto.PRODUCTO_ID });
            }

            ViewBag.FORMAFARMACEUTICA_ID = new SelectList(db.FORMAFARMACEUTICA.Where(f => f.FORMAFARMACEUTICA_ESTADO == "Activo"), "FORMAFARMACEUTICA_ID", "FARMAFARMACEUTICA_NOMBRE", producto.FORMAFARMACEUTICA_ID);
            ViewBag.SUBCATEGORIAPRODUCTO_ID = new SelectList(db.SUBCATEGORIAPRODUCTO.Where(sc => sc.SUBCATEGORIAPRODUCTO_ESTADO == "Activo"), "SUBCATEGORIAPRODUCTO_ID", "SUBCATEGORIAPRODUCTO_NOMBRE", producto.SUBCATEGORIAPRODUCTO_ID);
            ViewBag.VIAADMINISTRACION_ID = new SelectList(db.VIAADMINISTRACION.Where(va => va.VIAADMINISTRACION_ESTADO == "Activo"), "VIAADMINISTRACION_ID", "VIAADMINISTRATIVA_NOMBRE", producto.VIAADMINISTRACION_ID);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var producto = db.PRODUCTO.Find(id);
            if (producto == null)
                return HttpNotFound();

            ViewBag.Desactivado = TempData["Desactivado"];
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var producto = db.PRODUCTO.Find(id);
            if (producto == null)
                return HttpNotFound();

            producto.PRODUCTO_ESTADO = "Inactivo";
            db.Entry(producto).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Desactivado"] = true;
            return RedirectToAction("Delete", new { id = producto.PRODUCTO_ID });
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
