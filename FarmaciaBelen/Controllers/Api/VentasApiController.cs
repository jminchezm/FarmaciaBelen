using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using FarmaciaBelen.Models;
using FarmaciaBelen.Models.Dto;

[RoutePrefix("api/ventas")]
public class VentasApiController : ApiController
{
    private readonly DBFARMACIABELENEntities _db = new DBFARMACIABELENEntities();

    // POST api/ventas/validar  -> valida entrada/stock SIN guardar
    [HttpPost]
    [Route("validar")]
    // [Authorize] // si usas JWT
    public IHttpActionResult ValidarVenta(VentaCrearDto dto)
    {
        var errores = ValidarEntradaYStock(dto, soloValidar: true);
        if (errores.Any()) return Content(HttpStatusCode.BadRequest, new { errores });
        return Ok(new { mensaje = "Venta válida", total = CalcularTotal(dto) });
    }

    // POST api/ventas  -> crea la venta
    [HttpPost]
    [Route("")]
    // [Authorize]
    public IHttpActionResult CrearVenta(VentaCrearDto dto)
    {
        var errores = ValidarEntradaYStock(dto, soloValidar: false);
        if (errores.Any()) return Content(HttpStatusCode.BadRequest, new { errores });

        using (var tx = _db.Database.BeginTransaction())
        {
            try
            {
                // Generar código como en tu MVC
                var ventaId = GenerarNuevoCodigo();

                var venta = new VENTA
                {
                    VENTA_ID = ventaId,
                    VENTA_FECHA = dto.fecha ?? DateTime.Now,
                    CLIENTE_ID = dto.clienteId,
                    USUARIO_ID = dto.usuarioId,
                    VENTA_TOTAL = 0m,
                    DETALLEVENTA = new List<DETALLEVENTA>()
                };

                foreach (var d in dto.detalles)
                {
                    // elegir inventario: si viene inventarioId lo usamos; si no, el primero con stock
                    var inventario = d.inventarioId.HasValue
                        ? _db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.INVENTARIO_ID == d.inventarioId.Value)
                        : _db.INVENTARIO_PRODUCTO.FirstOrDefault(i => i.PRODUCTO_ID == d.productoId);

                    if (inventario == null)
                        return Content(HttpStatusCode.BadRequest, new { errores = new[] { $"No existe inventario para el producto {d.productoId}." } });

                    if (inventario.STOCK_ACTUAL < d.cantidad)
                        return Content(HttpStatusCode.BadRequest, new { errores = new[] { $"Stock insuficiente para el producto {d.productoId}." } });

                    inventario.STOCK_ACTUAL -= d.cantidad;

                    var subtotal = d.cantidad * d.precioUnitario;

                    venta.DETALLEVENTA.Add(new DETALLEVENTA
                    {
                        VENTA_ID = ventaId,
                        INVENTARIO_ID = inventario.INVENTARIO_ID,
                        PRODUCTO_ID = d.productoId,
                        DETALLEVENTA_CANTIDADVENDIDA = d.cantidad,
                        DETALLEVENTA_PRECIOUNITARIO = d.precioUnitario,
                        DETALLEVENTA_SUBTOTAL = subtotal
                    });

                    venta.VENTA_TOTAL += subtotal;
                }

                _db.VENTA.Add(venta);
                _db.SaveChanges();
                tx.Commit();

                var salida = MapearVenta(venta);
                var url = Url.Link("ObtenerVentaPorId", new { id = ventaId });
                return Created(url, salida);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return Content(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
    }

    // GET api/ventas/{id}
    [HttpGet]
    [Route("{id}", Name = "ObtenerVentaPorId")]
    // [Authorize]
    public IHttpActionResult ObtenerPorId(string id)
    {
        var v = _db.VENTA
            .Include(x => x.CLIENTE.PERSONA)
            .Include(x => x.USUARIO)
            .Include(x => x.DETALLEVENTA.Select(d => d.INVENTARIO_PRODUCTO.PRODUCTO))
            .FirstOrDefault(x => x.VENTA_ID == id);

        if (v == null) return NotFound();
        return Ok(MapearVenta(v));
    }

    // ====================== helpers ======================

    private List<string> ValidarEntradaYStock(VentaCrearDto dto, bool soloValidar)
    {
        var errores = new List<string>();

        if (dto == null) { errores.Add("Cuerpo de la solicitud vacío."); return errores; }
        if (string.IsNullOrWhiteSpace(dto.clienteId)) errores.Add("clienteId es requerido.");
        if (string.IsNullOrWhiteSpace(dto.usuarioId)) errores.Add("usuarioId es requerido.");
        if (dto.detalles == null || !dto.detalles.Any()) errores.Add("Debe incluir al menos un detalle.");

        if (errores.Any()) return errores;

        // existencia de cliente/usuario
        if (!_db.CLIENTE.Any(c => c.CLIENTE_ID == dto.clienteId && c.CLIENTE_ESTADO == "Activo"))
            errores.Add("El cliente no existe o está inactivo.");

        if (!_db.USUARIO.Any(u => u.USUARIO_ID == dto.usuarioId && u.USUARIO_ESTADO == "Activo"))
            errores.Add("El usuario no existe o está inactivo.");

        // detalles
        foreach (var d in dto.detalles)
        {
            if (string.IsNullOrWhiteSpace(d.productoId)) { errores.Add("productoId es requerido en cada detalle."); continue; }
            if (d.cantidad <= 0) errores.Add($"cantidad inválida para producto {d.productoId}.");
            if (d.precioUnitario <= 0) errores.Add($"precioUnitario inválido para producto {d.productoId}.");

            var invQuery = _db.INVENTARIO_PRODUCTO.AsQueryable();
            if (d.inventarioId.HasValue) invQuery = invQuery.Where(i => i.INVENTARIO_ID == d.inventarioId.Value);
            var inv = invQuery.FirstOrDefault(i => i.PRODUCTO_ID == d.productoId);

            if (inv == null) { errores.Add($"No hay inventario para producto {d.productoId}."); continue; }
            if (inv.STOCK_ACTUAL < d.cantidad) errores.Add($"Stock insuficiente para producto {d.productoId}. Disponible: {inv.STOCK_ACTUAL}.");
        }

        return errores;
    }

    private decimal CalcularTotal(VentaCrearDto dto)
        => dto?.detalles?.Sum(d => d.cantidad * d.precioUnitario) ?? 0m;

    private string GenerarNuevoCodigo()
    {
        var ultima = _db.VENTA.OrderByDescending(v => v.VENTA_ID).FirstOrDefault();
        if (ultima != null && !string.IsNullOrEmpty(ultima.VENTA_ID) && ultima.VENTA_ID.Length == 10)
        {
            var numero = ultima.VENTA_ID.Substring(3);
            var siguiente = int.Parse(numero) + 1;
            return "VEN" + siguiente.ToString("D7");
        }
        return "VEN0000001";
    }

    private VentaDto MapearVenta(VENTA v)
    {
        return new VentaDto
        {
            ventaId = v.VENTA_ID,
            fecha = v.VENTA_FECHA,
            total = v.VENTA_TOTAL ?? 0m,
            clienteId = v.CLIENTE_ID,
            clienteNombre = v.CLIENTE?.PERSONA != null
                ? (v.CLIENTE.PERSONA.PERSONA_PRIMERNOMBRE + " " + v.CLIENTE.PERSONA.PERSONA_PRIMERAPELLIDO)
                : null,
            usuarioId = v.USUARIO_ID,
            usuarioNombre = v.USUARIO?.USUARIO_NOMBRE,
            detalles = v.DETALLEVENTA.Select(d => new VentaDto.DetalleDto
            {
                productoId = d.PRODUCTO_ID,
                productoNombre = d.INVENTARIO_PRODUCTO?.PRODUCTO?.PRODUCTO_NOMBRE,
                cantidad = d.DETALLEVENTA_CANTIDADVENDIDA,
                precioUnitario = d.DETALLEVENTA_PRECIOUNITARIO,
                subtotal = d.DETALLEVENTA_SUBTOTAL
            }).ToList()
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}
