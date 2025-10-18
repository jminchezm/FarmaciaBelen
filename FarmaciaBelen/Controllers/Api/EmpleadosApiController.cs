using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using FarmaciaBelen.Models;
using FarmaciaBelen.Models.Dto;

[RoutePrefix("api/empleados")]
public class EmpleadosApiController : ApiController
{
    private readonly DBFARMACIABELENEntities _db = new DBFARMACIABELENEntities();

    // GET api/empleados?pagina=1&tamanioPagina=20&busqueda=ana&estado=Activo&desde=2024-01-01&hasta=2024-12-31
    [HttpGet]
    [Route("")]
    // [Authorize]
    public IHttpActionResult Obtener(
        int pagina = 1,
        int tamanioPagina = 20,
        string busqueda = null,
        string estado = null,
        DateTime? desde = null,
        DateTime? hasta = null)
    {
        if (pagina < 1) pagina = 1;
        if (tamanioPagina < 1 || tamanioPagina > 200) tamanioPagina = 20;

        var consulta = _db.EMPLEADO
            .Include(e => e.PERSONA)
            .Include(e => e.PUESTO)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var q = busqueda.Trim().ToLower();
            consulta = consulta.Where(e =>
                (e.PERSONA.PERSONA_PRIMERNOMBRE + " " +
                 e.PERSONA.PERSONA_SEGUNDONOMBRE + " " +
                 e.PERSONA.PERSONA_PRIMERAPELLIDO + " " +
                 e.PERSONA.PERSONA_SEGUNDOAPELLIDO).ToLower().Contains(q)
                || e.EMPLEADO_ID.Contains(busqueda));
        }

        if (!string.IsNullOrWhiteSpace(estado))
            consulta = consulta.Where(e => e.EMPLEADO_ESTADO == estado);

        if (desde.HasValue)
            consulta = consulta.Where(e => e.EMPLEADO_FECHAINGRESO >= desde.Value);

        if (hasta.HasValue)
            consulta = consulta.Where(e => e.EMPLEADO_FECHAINGRESO <= hasta.Value);

        var total = consulta.Count();

        var elementos = consulta
            .OrderBy(e => e.PERSONA.PERSONA_PRIMERAPELLIDO)
            .ThenBy(e => e.PERSONA.PERSONA_PRIMERNOMBRE)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToList()
            .Select(e => new EmpleadoDto
            {
                id = e.EMPLEADO_ID,
                primerNombre = e.PERSONA.PERSONA_PRIMERNOMBRE,
                segundoNombre = e.PERSONA.PERSONA_SEGUNDONOMBRE,
                primerApellido = e.PERSONA.PERSONA_PRIMERAPELLIDO,
                segundoApellido = e.PERSONA.PERSONA_SEGUNDOAPELLIDO,
                correo = e.PERSONA.PERSONA_CORREO,
                telefonoCasa = e.PERSONA.PERSONA_TELEFONOCASA,
                telefonoMovil = e.PERSONA.PERSONA_TELEFONOMOVIL,
                puesto = e.PUESTO != null ? e.PUESTO.PUESTO_NOMBRE : null,
                estado = e.EMPLEADO_ESTADO,
                nombreCompleto = string.Join(" ", new[]
                {
                    e.PERSONA.PERSONA_PRIMERNOMBRE,
                    e.PERSONA.PERSONA_SEGUNDONOMBRE,
                    e.PERSONA.PERSONA_PRIMERAPELLIDO,
                    e.PERSONA.PERSONA_SEGUNDOAPELLIDO
                }.Where(s => !string.IsNullOrWhiteSpace(s)))
            });

        return Ok(new { pagina, tamanioPagina, total, elementos });
    }

    // GET api/empleados/{id}
    [HttpGet]
    [Route("{id}")]
    // [Authorize]
    public IHttpActionResult ObtenerPorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest("id requerido");

        var e = _db.EMPLEADO
            .Include(x => x.PERSONA)
            .Include(x => x.PUESTO)
            .FirstOrDefault(x => x.EMPLEADO_ID == id);

        if (e == null) return NotFound();

        var dto = new EmpleadoDto
        {
            id = e.EMPLEADO_ID,
            primerNombre = e.PERSONA.PERSONA_PRIMERNOMBRE,
            segundoNombre = e.PERSONA.PERSONA_SEGUNDONOMBRE,
            primerApellido = e.PERSONA.PERSONA_PRIMERAPELLIDO,
            segundoApellido = e.PERSONA.PERSONA_SEGUNDOAPELLIDO,
            correo = e.PERSONA.PERSONA_CORREO,
            telefonoCasa = e.PERSONA.PERSONA_TELEFONOCASA,
            telefonoMovil = e.PERSONA.PERSONA_TELEFONOMOVIL,
            puesto = e.PUESTO != null ? e.PUESTO.PUESTO_NOMBRE : null,
            estado = e.EMPLEADO_ESTADO,
            nombreCompleto = string.Join(" ", new[]
            {
                e.PERSONA.PERSONA_PRIMERNOMBRE,
                e.PERSONA.PERSONA_SEGUNDONOMBRE,
                e.PERSONA.PERSONA_PRIMERAPELLIDO,
                e.PERSONA.PERSONA_SEGUNDOAPELLIDO
            }.Where(s => !string.IsNullOrWhiteSpace(s)))
        };

        return Ok(dto);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}
