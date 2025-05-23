using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FarmaciaBelen.Models;
using FarmaciaBelen.Models.ViewModel;

namespace FarmaciaBelen.Controllers
{
    public class VentasController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Ventas
        //public ActionResult Index()
        //{
        //    var ventas = db.VENTA.Include(v => v.CLIENTE).Include(v => v.USUARIO).ToList();
        //    return View(ventas);
        //}

        public ActionResult Index(string codigo, string cliente, string usuario, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var ventas = db.VENTA.Include(v => v.CLIENTE.PERSONA).Include(v => v.USUARIO).AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                ventas = ventas.Where(v => v.VENTA_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(cliente))
                ventas = ventas.Where(v => (v.CLIENTE.PERSONA.PERSONA_PRIMERNOMBRE + " " + v.CLIENTE.PERSONA.PERSONA_PRIMERAPELLIDO).Contains(cliente));

            if (!string.IsNullOrEmpty(usuario))
                ventas = ventas.Where(v => v.USUARIO.USUARIO_NOMBRE.Contains(usuario));

            if (fechaInicio.HasValue)
                ventas = ventas.Where(v => v.VENTA_FECHA >= fechaInicio);

            if (fechaFin.HasValue)
                ventas = ventas.Where(v => v.VENTA_FECHA <= fechaFin);

            return View(ventas.ToList());
        }


        // GET: Ventas/Create
        public ActionResult Create()
        {
            string nuevoCodigo;
            var ultimaVenta = db.VENTA.OrderByDescending(v => v.VENTA_ID).FirstOrDefault();
            if (ultimaVenta != null && ultimaVenta.VENTA_ID.Length == 10)
            {
                string numero = ultimaVenta.VENTA_ID.Substring(3);
                int siguiente = int.Parse(numero) + 1;
                nuevoCodigo = "VEN" + siguiente.ToString("D7");
            }
            else
            {
                nuevoCodigo = "VEN0000001";
            }

            // ✅ Obtener usuario actual desde sesión (ajusta según tu sistema)
            string usuarioID = Session["UsuarioID"] as string;

            var usuario = db.USUARIO.FirstOrDefault(u => u.USUARIO_ID == usuarioID);
            string usuarioNombre = usuario?.USUARIO_NOMBRE ?? "Desconocido";

            ViewBag.Clientes = new SelectList(
                db.CLIENTE
                    .Include(c => c.PERSONA)
                    .Where(c => c.CLIENTE_ESTADO == "Activo")
                    .ToList()
                    .Select(c => new
                    {
                        c.CLIENTE_ID,
                        CLIENTE_NOMBRE = c.PERSONA.PERSONA_PRIMERNOMBRE + " " + c.PERSONA.PERSONA_PRIMERAPELLIDO
                    }),
                "CLIENTE_ID",
                "CLIENTE_NOMBRE"
            );



            ViewBag.Productos = db.INVENTARIO_PRODUCTO.Where(i => i.STOCK_ACTUAL > 0)
                .Select(p => new
                {
                    p.PRODUCTO_ID,
                    p.PRODUCTO.PRODUCTO_NOMBRE,
                    p.PRODUCTO.PRODUCTO_IMG,
                    p.PRECIO_UNITARIO,
                    p.STOCK_ACTUAL,
                }).ToList();

            var viewModel = new VentaViewModel
            {
                VENTA_ID = nuevoCodigo,
                VENTA_FECHA = DateTime.Now,
                USUARIO_ID = usuarioID,
                USUARIO_NOMBRE = usuarioNombre,
                Detalles = new List<DetalleVentaViewModel>()
            };

            return View(viewModel);
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VentaViewModel viewModel)
        {
            if (!ModelState.IsValid || viewModel.Detalles == null || !viewModel.Detalles.Any())
            {
                if (string.IsNullOrEmpty(viewModel.CLIENTE_ID))
                {
                    ModelState.AddModelError("CLIENTE_ID", "Debe seleccionar un cliente.");
                }
                ModelState.AddModelError("", "Debe agregar al menos un producto.");
                ViewBag.Clientes = new SelectList(
                    db.CLIENTE
                        .Include(c => c.PERSONA)
                        .Where(c => c.CLIENTE_ESTADO == "Activo")
                        .ToList()
                        .Select(c => new
                        {
                            c.CLIENTE_ID,
                            CLIENTE_NOMBRE = c.PERSONA.PERSONA_PRIMERNOMBRE + " " + c.PERSONA.PERSONA_PRIMERAPELLIDO
                        }),
                    "CLIENTE_ID",
                    "CLIENTE_NOMBRE",
                    viewModel.CLIENTE_ID
                );

                ViewBag.Usuarios = new SelectList(db.USUARIO, "USUARIO_ID", "USUARIO_NOMBRE", viewModel.USUARIO_ID);
                ViewBag.Productos = db.INVENTARIO_PRODUCTO.Where(i => i.STOCK_ACTUAL > 0)
                    .Select(p => new
                    {
                        p.PRODUCTO_ID,
                        p.PRODUCTO.PRODUCTO_NOMBRE,
                        p.PRODUCTO.PRODUCTO_IMG,
                        p.PRECIO_UNITARIO,
                        p.STOCK_ACTUAL,
                    }).ToList();

                if (!string.IsNullOrEmpty(viewModel.USUARIO_ID))
                {
                    var usuario = db.USUARIO.FirstOrDefault(u => u.USUARIO_ID == viewModel.USUARIO_ID);
                    viewModel.USUARIO_NOMBRE = usuario?.USUARIO_NOMBRE ?? "Desconocido";
                }

                return View(viewModel);
            }

            var venta = new VENTA
            {
                VENTA_ID = viewModel.VENTA_ID,
                VENTA_FECHA = DateTime.Now,
                CLIENTE_ID = viewModel.CLIENTE_ID,
                USUARIO_ID = viewModel.USUARIO_ID,
                VENTA_TOTAL = viewModel.Detalles.Sum(d => d.DETALLEVENTA_SUBTOTAL),
                DETALLEVENTA = new List<DETALLEVENTA>()
            };

            foreach (var item in viewModel.Detalles)
            {
                var inventario = db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.PRODUCTO_ID == item.PRODUCTO_ID);

                if (inventario != null && inventario.STOCK_ACTUAL >= item.DETALLEVENTA_CANTIDADVENDIDA)
                {
                    inventario.STOCK_ACTUAL -= item.DETALLEVENTA_CANTIDADVENDIDA;

                    venta.DETALLEVENTA.Add(new DETALLEVENTA
                    {
                        PRODUCTO_ID = item.PRODUCTO_ID,
                        INVENTARIO_ID = inventario.INVENTARIO_ID,
                        DETALLEVENTA_CANTIDADVENDIDA = item.DETALLEVENTA_CANTIDADVENDIDA,
                        DETALLEVENTA_PRECIOUNITARIO = item.DETALLEVENTA_PRECIOUNITARIO,
                        DETALLEVENTA_SUBTOTAL = item.DETALLEVENTA_SUBTOTAL
                    });
                }
                else
                {
                    ModelState.AddModelError("", $"Stock insuficiente para el producto {item.PRODUCTO_ID}.");
                    return View(viewModel);
                }
            }

            db.VENTA.Add(venta);
            db.SaveChanges();
            TempData["Creado"] = true;
            return RedirectToAction("Create");
        }

        // GET: Ventas/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var venta = db.VENTA
                .Include(v => v.DETALLEVENTA.Select(d => d.INVENTARIO_PRODUCTO.PRODUCTO))
                .Include(v => v.CLIENTE.PERSONA)
                .Include(v => v.USUARIO)
                .FirstOrDefault(v => v.VENTA_ID == id);

            if (venta == null)
                return HttpNotFound();

            var viewModel = new VentaViewModel
            {
                VENTA_ID = venta.VENTA_ID,
                VENTA_FECHA = venta.VENTA_FECHA,
                VENTA_TOTAL = venta.VENTA_TOTAL,
                CLIENTE_ID = venta.CLIENTE_ID,
                CLIENTE_NOMBRE = venta.CLIENTE.PERSONA.PERSONA_PRIMERNOMBRE + " " + venta.CLIENTE.PERSONA.PERSONA_PRIMERAPELLIDO,
                USUARIO_ID = venta.USUARIO_ID,
                USUARIO_NOMBRE = venta.USUARIO.USUARIO_NOMBRE,
                Detalles = venta.DETALLEVENTA.Select(d => new DetalleVentaViewModel
                {
                    PRODUCTO_ID = d.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_NOMBRE,
                    PRODUCTO_IMG = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_IMG,
                    DETALLEVENTA_CANTIDADVENDIDA = d.DETALLEVENTA_CANTIDADVENDIDA,
                    DETALLEVENTA_PRECIOUNITARIO = d.DETALLEVENTA_PRECIOUNITARIO
                }).ToList()
            };

            return View(viewModel);
        }

        //public ActionResult Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var venta = db.VENTA
        //        .Include(v => v.DETALLEVENTA.Select(d => d.INVENTARIO_PRODUCTO.PRODUCTO))
        //        .Include(v => v.CLIENTE)
        //        .Include(v => v.USUARIO)
        //        .FirstOrDefault(v => v.VENTA_ID == id);

        //    if (venta == null)
        //        return HttpNotFound();

        //    var viewModel = new VentaViewModel
        //    {
        //        VENTA_ID = venta.VENTA_ID,
        //        VENTA_FECHA = venta.VENTA_FECHA,
        //        VENTA_TOTAL = venta.VENTA_TOTAL,
        //        CLIENTE_ID = venta.CLIENTE_ID,
        //        CLIENTE_NOMBRE = venta.CLIENTE?.PERSONA.PERSONA_PRIMERNOMBRE,
        //        USUARIO_ID = venta.USUARIO_ID,
        //        USUARIO_NOMBRE = venta.USUARIO?.USUARIO_NOMBRE,
        //        Detalles = venta.DETALLEVENTA.Select(d => new DetalleVentaViewModel
        //        {
        //            PRODUCTO_ID = d.PRODUCTO_ID,
        //            PRODUCTO_NOMBRE = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_NOMBRE,
        //            PRODUCTO_IMG = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_IMG,
        //            DETALLEVENTA_CANTIDADVENDIDA = d.DETALLEVENTA_CANTIDADVENDIDA,
        //            DETALLEVENTA_PRECIOUNITARIO = d.DETALLEVENTA_PRECIOUNITARIO
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}

        //GET
        //public ActionResult Edit(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var venta = db.VENTA
        //        .Include(v => v.CLIENTE.PERSONA)
        //        .Include(v => v.USUARIO)
        //        .Include(v => v.DETALLEVENTA.Select(d => d.INVENTARIO_PRODUCTO.PRODUCTO))
        //        .FirstOrDefault(v => v.VENTA_ID == id);

        //    if (venta == null)
        //        return HttpNotFound();

        //    var viewModel = new VentaViewModel
        //    {
        //        VENTA_ID = venta.VENTA_ID,
        //        VENTA_FECHA = venta.VENTA_FECHA,
        //        VENTA_TOTAL = venta.VENTA_TOTAL,
        //        CLIENTE_ID = venta.CLIENTE_ID,
        //        CLIENTE_NOMBRE = venta.CLIENTE?.PERSONA.PERSONA_PRIMERNOMBRE + " " + venta.CLIENTE?.PERSONA.PERSONA_PRIMERAPELLIDO,
        //        USUARIO_ID = venta.USUARIO_ID,
        //        USUARIO_NOMBRE = venta.USUARIO?.USUARIO_NOMBRE,
        //        Detalles = venta.DETALLEVENTA.Select(d => new DetalleVentaViewModel
        //        {
        //            DETALLEVENTA_ID = d.DETALLEVENTA_ID,
        //            PRODUCTO_ID = d.PRODUCTO_ID,
        //            PRODUCTO_NOMBRE = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_NOMBRE,
        //            PRODUCTO_IMG = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_IMG,
        //            DETALLEVENTA_CANTIDADVENDIDA = d.DETALLEVENTA_CANTIDADVENDIDA,
        //            DETALLEVENTA_PRECIOUNITARIO = d.DETALLEVENTA_PRECIOUNITARIO
        //        }).ToList()
        //    };

        //    ViewBag.Clientes = new SelectList(
        //        db.CLIENTE.Include(c => c.PERSONA)
        //                  .Where(c => c.CLIENTE_ESTADO == "Activo")
        //                  .ToList()
        //                  .Select(c => new {
        //                      c.CLIENTE_ID,
        //                      CLIENTE_NOMBRE = c.PERSONA.PERSONA_PRIMERNOMBRE + " " + c.PERSONA.PERSONA_PRIMERAPELLIDO
        //                  }),
        //        "CLIENTE_ID", "CLIENTE_NOMBRE", viewModel.CLIENTE_ID
        //    );

        //    //ViewBag.Productos = db.INVENTARIO_PRODUCTO.Where(i => i.STOCK_ACTUAL > 0)
        //    //    .Select(p => new
        //    //    {
        //    //        p.PRODUCTO_ID,
        //    //        p.PRODUCTO.PRODUCTO_NOMBRE,
        //    //        p.PRODUCTO.PRODUCTO_IMG,
        //    //        p.PRECIO_UNITARIO
        //    //    }).ToList();

        //    ViewBag.Productos = db.INVENTARIO_PRODUCTO
        //    .Where(i => i.STOCK_ACTUAL > 0)
        //    .Select(p => new ProductoSelectViewModel
        //    {
        //        PRODUCTO_ID = p.PRODUCTO_ID,
        //        PRODUCTO_NOMBRE = p.PRODUCTO.PRODUCTO_NOMBRE,
        //        PRODUCTO_IMG = p.PRODUCTO.PRODUCTO_IMG,
        //        PRECIO_UNITARIO = p.PRECIO_UNITARIO
        //    }).ToList();


        //    return View(viewModel);
        //}

        ////POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(VentaViewModel viewModel)
        //{
        //    if (!ModelState.IsValid || viewModel.Detalles == null || !viewModel.Detalles.Any())
        //    {
        //        ModelState.AddModelError("", "Debe agregar al menos un producto.");

        //        ViewBag.Clientes = new SelectList(
        //            db.CLIENTE.Include(c => c.PERSONA)
        //                      .Where(c => c.CLIENTE_ESTADO == "Activo")
        //                      .ToList()
        //                      .Select(c => new {
        //                          c.CLIENTE_ID,
        //                          CLIENTE_NOMBRE = c.PERSONA.PERSONA_PRIMERNOMBRE + " " + c.PERSONA.PERSONA_PRIMERAPELLIDO
        //                      }),
        //            "CLIENTE_ID", "CLIENTE_NOMBRE", viewModel.CLIENTE_ID
        //        );

        //        //ViewBag.Productos = db.INVENTARIO_PRODUCTO.Where(i => i.STOCK_ACTUAL > 0)
        //        //    .Select(p => new
        //        //    {
        //        //        p.PRODUCTO_ID,
        //        //        p.PRODUCTO.PRODUCTO_NOMBRE,
        //        //        p.PRODUCTO.PRODUCTO_IMG,
        //        //        p.PRECIO_UNITARIO
        //        //    }).ToList();
        //        ViewBag.Productos = db.INVENTARIO_PRODUCTO
        //        .Where(i => i.STOCK_ACTUAL > 0)
        //        .Select(p => new ProductoSelectViewModel
        //        {
        //            PRODUCTO_ID = p.PRODUCTO_ID,
        //            PRODUCTO_NOMBRE = p.PRODUCTO.PRODUCTO_NOMBRE,
        //            PRODUCTO_IMG = p.PRODUCTO.PRODUCTO_IMG,
        //            PRECIO_UNITARIO = p.PRECIO_UNITARIO
        //        }).ToList();


        //        return View(viewModel);
        //    }

        //    var venta = db.VENTA.Include(v => v.DETALLEVENTA).FirstOrDefault(v => v.VENTA_ID == viewModel.VENTA_ID);

        //    if (venta == null)
        //        return HttpNotFound();

        //    // Restaurar stock de los productos antes de editar
        //    foreach (var detalle in venta.DETALLEVENTA)
        //    {
        //        var inventario = db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.INVENTARIO_ID == detalle.INVENTARIO_ID);
        //        if (inventario != null)
        //        {
        //            inventario.STOCK_ACTUAL += detalle.DETALLEVENTA_CANTIDADVENDIDA;
        //        }
        //    }

        //    // Eliminar detalle anterior
        //    db.DETALLEVENTA.RemoveRange(venta.DETALLEVENTA);

        //    // Actualizar datos generales
        //    //venta.CLIENTE_ID = viewModel.CLIENTE_ID;
        //    //venta.VENTA_TOTAL = viewModel.Detalles.Sum(d => d.DETALLEVENTA_SUBTOTAL);
        //    // Actualizar datos generales
        //    venta.CLIENTE_ID = viewModel.CLIENTE_ID;
        //    venta.VENTA_TOTAL = viewModel.Detalles.Sum(d => d.DETALLEVENTA_SUBTOTAL);
        //    venta.VENTA_FECHA = DateTime.Now;

        //    venta.DETALLEVENTA = new List<DETALLEVENTA>();

        //    foreach (var item in viewModel.Detalles)
        //    {
        //        var inventario = db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.PRODUCTO_ID == item.PRODUCTO_ID);
        //        if (inventario != null && inventario.STOCK_ACTUAL >= item.DETALLEVENTA_CANTIDADVENDIDA)
        //        {
        //            inventario.STOCK_ACTUAL -= item.DETALLEVENTA_CANTIDADVENDIDA;

        //            venta.DETALLEVENTA.Add(new DETALLEVENTA
        //            {
        //                PRODUCTO_ID = item.PRODUCTO_ID,
        //                INVENTARIO_ID = inventario.INVENTARIO_ID,
        //                DETALLEVENTA_CANTIDADVENDIDA = item.DETALLEVENTA_CANTIDADVENDIDA,
        //                DETALLEVENTA_PRECIOUNITARIO = item.DETALLEVENTA_PRECIOUNITARIO,
        //                DETALLEVENTA_SUBTOTAL = item.DETALLEVENTA_SUBTOTAL
        //            });
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", $"Stock insuficiente para el producto {item.PRODUCTO_ID}.");
        //            return View(viewModel);
        //        }
        //    }

        //    db.SaveChanges();
        //    TempData["Editado"] = true;
        //    return RedirectToAction("Edit", new { id = venta.VENTA_ID });
        //}


        //// GET: Ventas/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var venta = db.VENTA.Include(v => v.DETALLEVENTA.Select(d => d.INVENTARIO_PRODUCTO.PRODUCTO))
        //                         .Include(v => v.CLIENTE).FirstOrDefault(v => v.VENTA_ID == id);

        //    if (venta == null)
        //        return HttpNotFound();

        //    var viewModel = new VentaViewModel
        //    {
        //        VENTA_ID = venta.VENTA_ID,
        //        VENTA_FECHA = venta.VENTA_FECHA,
        //        CLIENTE_NOMBRE = venta.CLIENTE.PERSONA.PERSONA_PRIMERNOMBRE,
        //        Detalles = venta.DETALLEVENTA.Select(d => new DetalleVentaViewModel
        //        {
        //            PRODUCTO_NOMBRE = d.INVENTARIO_PRODUCTO.PRODUCTO.PRODUCTO_NOMBRE,
        //            DETALLEVENTA_CANTIDADVENDIDA = d.DETALLEVENTA_CANTIDADVENDIDA,
        //            DETALLEVENTA_PRECIOUNITARIO = d.DETALLEVENTA_PRECIOUNITARIO
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}

        //// POST: Ventas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    var venta = db.VENTA.Include(v => v.DETALLEVENTA).FirstOrDefault(v => v.VENTA_ID == id);

        //    if (venta == null)
        //        return HttpNotFound();

        //    db.DETALLEVENTA.RemoveRange(venta.DETALLEVENTA);
        //    db.VENTA.Remove(venta);
        //    db.SaveChanges();
        //    TempData["Eliminado"] = true;

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
