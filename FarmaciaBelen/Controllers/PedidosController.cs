using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FarmaciaBelen.Models;
using FarmaciaBelen.Models.ViewModel;
//using FarmaciaBelen.ViewModels;

namespace FarmaciaBelen.Controllers
{
    public class PedidosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Pedidos
        public ActionResult Index(string codigo, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var pedidos = db.PEDIDO.AsQueryable();
            if (!string.IsNullOrEmpty(codigo))
                pedidos = pedidos.Where(p => p.PEDIDO_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(estado))
                pedidos = pedidos.Where(p => p.ESTADO_PEDIDO.Contains(estado));

            if (fechaInicio.HasValue)
                pedidos = pedidos.Where(p => p.FECHA_PEDIDO >= fechaInicio.Value);

            if (fechaFin.HasValue)
                pedidos = pedidos.Where(p => p.FECHA_PEDIDO <= fechaFin.Value);
            return View(pedidos.ToList());
        }

        // GET: Pedidos/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pedido = db.PEDIDO
                .Include(p => p.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
                .Include(p => p.PROVEEDOR)
                .FirstOrDefault(p => p.PEDIDO_ID == id);

            if (pedido == null)
                return HttpNotFound();

            var viewModel = new PedidoViewModel
            {
                PEDIDO_ID = pedido.PEDIDO_ID,
                FECHA_PEDIDO = pedido.FECHA_PEDIDO,
                ESTADO_PEDIDO = pedido.ESTADO_PEDIDO,
                FECHA_ENTREGA_ESTIMADA = pedido.FECHA_ENTREGA_ESTIMADA,
                OBSERVACIONES = pedido.OBSERVACIONES,
                PROVEEDOR_ID = pedido.PROVEEDOR_ID,
                PROVEEDOR_NOMBRE = pedido.PROVEEDOR?.PROVEEDOR_NOMBRE ?? "No asignado",
                Detalles = pedido.DETALLE_PEDIDO.Select(d => new DetallePedidoViewModel
                {
                    DETALLEPEDIDO_ID = d.DETALLEPEDIDO_ID,
                    PRODUCTO_ID = d.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = d.PRODUCTO.PRODUCTO_NOMBRE,
                    PRODUCTO_IMG = d.PRODUCTO.PRODUCTO_IMG,
                    DETALLEPEDIDO_CANTIDAD = d.DETALLEPEDIDO_CANTIDAD,
                    PRECIO_UNITARIO = d.PRECIO_UNITARIO
                }).ToList()
            };

            ViewBag.Confirmado = TempData["Confirmado"];
            ViewBag.ErrorConfirmacion = TempData["ErrorConfirmacion"];

            return View(viewModel);
        }

        // GET: Pedidos/Create
        public ActionResult Create()
        {
            var utimoPedido = db.PEDIDO
                .OrderByDescending(a => a.PEDIDO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (utimoPedido != null && utimoPedido.PEDIDO_ID.Length == 10)
            {
                string numero = utimoPedido.PEDIDO_ID.Substring(4);
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "PEDD" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "PEDD000001";
            }

            ViewBag.Proveedores = new SelectList(
                db.PROVEEDOR.Where(p => p.PROVEEDOR_ESTADO == "Activo"),
                "PROVEEDOR_ID",
                "PROVEEDOR_NOMBRE"
            );

            ViewBag.Productos = db.PRODUCTO.Where(p => p.PRODUCTO_ESTADO == "Activo").Select(p => new
            {
                p.PRODUCTO_ID,
                p.PRODUCTO_NOMBRE,
                IMAGEN_URL = p.PRODUCTO_IMG ?? "/Content/ImagenesProductos/default.png"
            }).ToList();

            // Aquí asignas el código generado al ViewModel
            var viewModel = new PedidoViewModel
            {
                PEDIDO_ID = nuevoCodigo,
                ESTADO_PEDIDO = "Pendiente",
            };

            ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal

            return View(viewModel);
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoViewModel viewModel)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

            if (viewModel.Detalles == null || !viewModel.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto al pedido.");
            }



            if (ModelState.IsValid)
            {
                var pedido = new PEDIDO
                {
                    PEDIDO_ID = viewModel.PEDIDO_ID,
                    FECHA_PEDIDO = DateTime.Now,
                    ESTADO_PEDIDO = viewModel.ESTADO_PEDIDO,
                    FECHA_ENTREGA_ESTIMADA = viewModel.FECHA_ENTREGA_ESTIMADA,
                    OBSERVACIONES = viewModel.OBSERVACIONES,
                    PROVEEDOR_ID = viewModel.PROVEEDOR_ID,
                    DETALLE_PEDIDO = viewModel.Detalles.Select(d => new DETALLE_PEDIDO
                    {
                        PRODUCTO_ID = d.PRODUCTO_ID,
                        DETALLEPEDIDO_CANTIDAD = d.DETALLEPEDIDO_CANTIDAD,
                        PRECIO_UNITARIO = d.PRECIO_UNITARIO ?? 0,
                        SUBTOTAL = (d.PRECIO_UNITARIO ?? 0) * d.DETALLEPEDIDO_CANTIDAD
                    }).ToList()
                };

                db.PEDIDO.Add(pedido);
                db.SaveChanges();

                TempData["Creado"] = true;
                return RedirectToAction("Create");
            }

            ViewBag.Proveedores = new SelectList(db.PROVEEDOR.Where(p => p.PROVEEDOR_ESTADO == "Activo"), "PROVEEDOR_ID", "PROVEEDOR_NOMBRE", viewModel.PROVEEDOR_ID);

            ViewBag.Productos = db.PRODUCTO.Where(p => p.PRODUCTO_ESTADO == "Activo").Select(p => new ProductoViewModel
            {
                PRODUCTO_ID = p.PRODUCTO_ID,
                PRODUCTO_NOMBRE = p.PRODUCTO_NOMBRE,
                IMAGEN_URL = p.PRODUCTO_IMG ?? "/Content/ImagenesProductos/default.png"
            }).ToList();


            return View(viewModel);
        }

        // GET: Pedidos/Edit/5

        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pedido = db.PEDIDO
                .Include(p => p.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
                .Include(p => p.PROVEEDOR) // <- ¡NECESARIO!
                .FirstOrDefault(p => p.PEDIDO_ID == id);

            if (pedido == null)
                return HttpNotFound();

            var viewModel = new PedidoViewModel
            {
                PEDIDO_ID = pedido.PEDIDO_ID,
                FECHA_PEDIDO = pedido.FECHA_PEDIDO,
                ESTADO_PEDIDO = pedido.ESTADO_PEDIDO,
                FECHA_ENTREGA_ESTIMADA = pedido.FECHA_ENTREGA_ESTIMADA,
                OBSERVACIONES = pedido.OBSERVACIONES,
                PROVEEDOR_ID = pedido.PROVEEDOR_ID,
                PROVEEDOR_NOMBRE = pedido.PROVEEDOR?.PROVEEDOR_NOMBRE,
                //PROVEEDOR_NOMBRE = pedido.PROVEEDOR.PROVEEDOR_NOMBRE,
                Detalles = pedido.DETALLE_PEDIDO.Select(d => new DetallePedidoViewModel
                {
                    DETALLEPEDIDO_ID = d.DETALLEPEDIDO_ID,
                    PRODUCTO_ID = d.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = d.PRODUCTO.PRODUCTO_NOMBRE,
                    PRODUCTO_IMG = d.PRODUCTO.PRODUCTO_IMG,
                    DETALLEPEDIDO_CANTIDAD = d.DETALLEPEDIDO_CANTIDAD,
                    PRECIO_UNITARIO = d.PRECIO_UNITARIO
                }).ToList()
            };

            ViewBag.Proveedores = new SelectList(
                db.PROVEEDOR.Where(p => p.PROVEEDOR_ESTADO == "Activo"),
                "PROVEEDOR_ID", "PROVEEDOR_NOMBRE", viewModel.PROVEEDOR_ID
            );

            ViewBag.Productos = db.PRODUCTO
                .Where(p => p.PRODUCTO_ESTADO == "Activo")
                .Select(p => new ProductoViewModel
                {
                    PRODUCTO_ID = p.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = p.PRODUCTO_NOMBRE,
                    IMAGEN_URL = p.PRODUCTO_IMG ?? "/Content/ImagenesProductos/default.png"
                }).ToList();

            ViewBag.Editado = TempData["Editado"];

            return View(viewModel);
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PedidoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var pedido = db.PEDIDO
                    .Include(p => p.DETALLE_PEDIDO)
                    .FirstOrDefault(p => p.PEDIDO_ID == viewModel.PEDIDO_ID);

                if (pedido == null)
                    return HttpNotFound();

                // Actualizar pedido
                pedido.ESTADO_PEDIDO = viewModel.ESTADO_PEDIDO;
                pedido.FECHA_ENTREGA_ESTIMADA = viewModel.FECHA_ENTREGA_ESTIMADA;
                pedido.OBSERVACIONES = viewModel.OBSERVACIONES;
                pedido.PROVEEDOR_ID = viewModel.PROVEEDOR_ID;

                // Eliminar detalles anteriores
                db.DETALLE_PEDIDO.RemoveRange(pedido.DETALLE_PEDIDO);

                // Agregar nuevos detalles
                foreach (var d in viewModel.Detalles)
                {
                    pedido.DETALLE_PEDIDO.Add(new DETALLE_PEDIDO
                    {
                        PRODUCTO_ID = d.PRODUCTO_ID,
                        DETALLEPEDIDO_CANTIDAD = d.DETALLEPEDIDO_CANTIDAD,
                        PRECIO_UNITARIO = d.PRECIO_UNITARIO ?? 0,
                        SUBTOTAL = (d.PRECIO_UNITARIO ?? 0) * d.DETALLEPEDIDO_CANTIDAD
                    });
                }

                db.SaveChanges();
                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = viewModel.PEDIDO_ID });
            }

            // Si llega aquí, hubo error: volver a cargar SelectList y productos
            ViewBag.Proveedores = new SelectList(
                db.PROVEEDOR.Where(p => p.PROVEEDOR_ESTADO == "Activo"),
                "PROVEEDOR_ID", "PROVEEDOR_NOMBRE", viewModel.PROVEEDOR_ID
            );

            ViewBag.Productos = db.PRODUCTO
                .Where(p => p.PRODUCTO_ESTADO == "Activo")
                .Select(p => new ProductoViewModel
                {
                    PRODUCTO_ID = p.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = p.PRODUCTO_NOMBRE,
                    IMAGEN_URL = p.PRODUCTO_IMG ?? "/Content/ImagenesProductos/default.png"
                }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirmar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pedido = db.PEDIDO
                .Include(p => p.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
                .FirstOrDefault(p => p.PEDIDO_ID == id);

            if (pedido == null)
                return HttpNotFound();

            // Validar estado
            if (!pedido.ESTADO_PEDIDO.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorConfirmacion"] = "Solo se pueden confirmar pedidos que estén en estado 'Pendiente'.";
                return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
            }

            // Validar proveedor
            if (string.IsNullOrEmpty(pedido.PROVEEDOR_ID))
            {
                TempData["ErrorConfirmacion"] = "No se puede confirmar el pedido porque no tiene un proveedor asignado.";
                return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
            }

            // Validar precios > 0
            if (pedido.DETALLE_PEDIDO.Any(d => d.PRECIO_UNITARIO <= 0))
            {
                TempData["ErrorConfirmacion"] = "No se puede confirmar el pedido porque uno o más productos tienen precio unitario igual o menor a 0.";
                return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
            }

            // ✅ Actualizar inventario usando precio promedio ponderado
            foreach (var detalle in pedido.DETALLE_PEDIDO)
            {
                var inventario = db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.PRODUCTO_ID == detalle.PRODUCTO_ID);

                if (inventario != null)
                {
                    int stockActual = inventario.STOCK_ACTUAL;
                    decimal precioActual = inventario.PRECIO_UNITARIO;
                    int cantidadNueva = detalle.DETALLEPEDIDO_CANTIDAD;
                    decimal precioNuevo = detalle.PRECIO_UNITARIO;

                    int nuevoStock = stockActual + cantidadNueva;

                    // Cálculo del precio promedio ponderado
                    decimal nuevoPrecio = ((precioActual * stockActual) + (precioNuevo * cantidadNueva)) / nuevoStock;

                    inventario.STOCK_ACTUAL = nuevoStock;
                    inventario.PRECIO_UNITARIO = nuevoPrecio;
                }
                else
                {
                    db.INVENTARIO_PRODUCTO.Add(new INVENTARIO_PRODUCTO
                    {
                        PRODUCTO_ID = detalle.PRODUCTO_ID,
                        STOCK_ACTUAL = detalle.DETALLEPEDIDO_CANTIDAD,
                        PRECIO_UNITARIO = detalle.PRECIO_UNITARIO
                    });
                }
            }

            // Confirmar el pedido
            pedido.ESTADO_PEDIDO = "Confirmado";

            db.SaveChanges();

            TempData["Confirmado"] = true;
            return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Confirmar(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var pedido = db.PEDIDO
        //        .Include(p => p.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
        //        .FirstOrDefault(p => p.PEDIDO_ID == id);

        //    if (pedido == null)
        //        return HttpNotFound();

        //    // Validar que el pedido esté en estado "Pendiente"
        //    if (!pedido.ESTADO_PEDIDO.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
        //    {
        //        TempData["ErrorConfirmacion"] = "Solo se pueden confirmar pedidos que estén en estado 'Pendiente'.";
        //        return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
        //    }

        //    // ✅ Validar que el pedido tenga un proveedor asignado
        //    if (string.IsNullOrEmpty(pedido.PROVEEDOR_ID))
        //    {
        //        TempData["ErrorConfirmacion"] = "No se puede confirmar el pedido porque no tiene un proveedor asignado.";
        //        return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
        //    }

        //    // Validar precios > 0
        //    if (pedido.DETALLE_PEDIDO.Any(d => d.PRECIO_UNITARIO <= 0))
        //    {
        //        TempData["ErrorConfirmacion"] = "No se puede confirmar el pedido porque uno o más productos tienen precio unitario igual o menor a 0.";
        //        return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
        //    }

        //    // Actualizar inventario
        //    foreach (var detalle in pedido.DETALLE_PEDIDO)
        //    {
        //        var inventario = db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.PRODUCTO_ID == detalle.PRODUCTO_ID);

        //        if (inventario != null)
        //        {
        //            inventario.STOCK_ACTUAL += detalle.DETALLEPEDIDO_CANTIDAD;
        //            inventario.PRECIO_UNITARIO = detalle.PRECIO_UNITARIO;
        //        }
        //        else
        //        {
        //            db.INVENTARIO_PRODUCTO.Add(new INVENTARIO_PRODUCTO
        //            {
        //                PRODUCTO_ID = detalle.PRODUCTO_ID,
        //                STOCK_ACTUAL = detalle.DETALLEPEDIDO_CANTIDAD,
        //                PRECIO_UNITARIO = detalle.PRECIO_UNITARIO
        //            });
        //        }
        //    }

        //    // Cambiar estado
        //    pedido.ESTADO_PEDIDO = "Confirmado";

        //    db.SaveChanges();

        //    TempData["Confirmado"] = true;
        //    return RedirectToAction("Details", new { id = pedido.PEDIDO_ID });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string PEDIDO_ID)
        {
            var pedido = db.PEDIDO
                .Include(p => p.DETALLE_PEDIDO)
                .FirstOrDefault(p => p.PEDIDO_ID == PEDIDO_ID);

            if (pedido == null)
                return HttpNotFound();

            if (pedido.ESTADO_PEDIDO == "Confirmado")
            {
                TempData["ErrorEliminar"] = true;
                return RedirectToAction("Delete", new { id = pedido.PEDIDO_ID });
            }

            db.DETALLE_PEDIDO.RemoveRange(pedido.DETALLE_PEDIDO);
            db.PEDIDO.Remove(pedido);
            db.SaveChanges();

            TempData["Eliminado"] = true;
            return RedirectToAction("Index"); // ✅ Ya no redirige a Delete
        }


        // GET: Pedidos/Delete/5

        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pedido = db.PEDIDO
                .Include(p => p.DETALLE_PEDIDO.Select(d => d.PRODUCTO))
                .Include(p => p.PROVEEDOR)
                .FirstOrDefault(p => p.PEDIDO_ID == id);

            if (pedido == null)
                return HttpNotFound();

            var viewModel = new PedidoViewModel
            {
                PEDIDO_ID = pedido.PEDIDO_ID,
                ESTADO_PEDIDO = pedido.ESTADO_PEDIDO,
                FECHA_ENTREGA_ESTIMADA = pedido.FECHA_ENTREGA_ESTIMADA,
                OBSERVACIONES = pedido.OBSERVACIONES,
                PROVEEDOR_NOMBRE = pedido.PROVEEDOR?.PROVEEDOR_NOMBRE ?? "No asignado",
                Detalles = pedido.DETALLE_PEDIDO.Select(d => new DetallePedidoViewModel
                {
                    PRODUCTO_ID = d.PRODUCTO_ID,
                    PRODUCTO_NOMBRE = d.PRODUCTO.PRODUCTO_NOMBRE,
                    DETALLEPEDIDO_CANTIDAD = d.DETALLEPEDIDO_CANTIDAD,
                    PRECIO_UNITARIO = d.PRECIO_UNITARIO
                }).ToList()
            };

            ViewBag.Eliminado = TempData["Eliminado"];
            ViewBag.ErrorEliminar = TempData["ErrorEliminar"];

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
