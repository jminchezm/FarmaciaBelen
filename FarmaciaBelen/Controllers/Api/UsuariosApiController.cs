using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using FarmaciaBelen.Models;
using FarmaciaBelen.Models.Dto;

[RoutePrefix("api/usuarios")]
public class UsuariosApiController : ApiController
{
    private readonly DBFARMACIABELENEntities _db = new DBFARMACIABELENEntities();

    // POST api/usuarios
    [HttpPost]
    [Route("")]
    // [Authorize]
    public IHttpActionResult CrearUsuario(UsuarioCrearDto dto)
    {
        // ---- Validaciones de entrada
        var errores = Validar(dto);
        if (errores.Any()) return Content(HttpStatusCode.BadRequest, new { errores });

        // ---- Generar ID si no viene
        var usuarioId = string.IsNullOrWhiteSpace(dto.usuarioId)
            ? GenerarNuevoCodigo()
            : dto.usuarioId;

        // ---- Reglas de negocio
        // 1) UsuarioId único
        if (_db.USUARIO.Any(u => u.USUARIO_ID == usuarioId))
            return Content(HttpStatusCode.Conflict, new { errores = new[] { "El código de usuario ya existe." } });

        // 2) Nombre de usuario único (si aplica)
        if (_db.USUARIO.Any(u => u.USUARIO_NOMBRE == dto.usuarioNombre))
            return Content(HttpStatusCode.Conflict, new { errores = new[] { "El nombre de usuario ya existe." } });

        // 3) Rol existe y activo
        var rol = _db.ROL.FirstOrDefault(r => r.ROL_ID == dto.rolId && r.ROL_ESTADO == "Activo");
        if (rol == null) return Content(HttpStatusCode.BadRequest, new { errores = new[] { "El rol no existe o no está activo." } });

        // 4) Empleado existe y activo
        var empleado = _db.EMPLEADO
            .Include(e => e.PERSONA)
            .FirstOrDefault(e => e.EMPLEADO_ID == dto.empleadoId && e.EMPLEADO_ESTADO == "Activo");
        if (empleado == null) return Content(HttpStatusCode.BadRequest, new { errores = new[] { "El empleado no existe o no está activo." } });

        // 5) Empleado NO debe tener ya usuario
        if (_db.USUARIO.Any(u => u.EMPLEADO_ID == dto.empleadoId))
            return Content(HttpStatusCode.BadRequest, new { errores = new[] { "Ese empleado ya tiene un usuario asignado." } });

        // ---- Crear usuario
        var nuevo = new USUARIO
        {
            USUARIO_ID = usuarioId,
            USUARIO_NOMBRE = dto.usuarioNombre,
            USUARIO_CONTRASENA = HashSha256(dto.contrasenaPlano),
            USUARIO_FECHAREGISTRO = DateTime.Now,
            ROL_ID = dto.rolId,
            EMPLEADO_ID = dto.empleadoId,
            USUARIO_ESTADO = string.IsNullOrWhiteSpace(dto.usuarioEstado) ? "Activo" : dto.usuarioEstado
        };

        _db.USUARIO.Add(nuevo);
        _db.SaveChanges();

        var salida = MapearDto(nuevo, rol.ROL_NOMBRE, empleado);
        var url = Url.Link("ObtenerUsuarioPorId", new { id = nuevo.USUARIO_ID });
        return Created(url, salida);
    }

    // GET api/usuarios/{id}
    [HttpGet]
    [Route("{id}", Name = "ObtenerUsuarioPorId")]
    // [Authorize]
    public IHttpActionResult ObtenerPorId(string id)
    {
        var u = _db.USUARIO
            .Include(x => x.ROL)
            .Include(x => x.EMPLEADO.PERSONA)
            .FirstOrDefault(x => x.USUARIO_ID == id);

        if (u == null) return NotFound();

        return Ok(MapearDto(u, u.ROL?.ROL_NOMBRE, u.EMPLEADO));
    }

    // (opcional) GET api/usuarios?empleadoId=...&usuarioNombre=...
    [HttpGet]
    [Route("")]
    public IHttpActionResult Buscar(string empleadoId = null, string usuarioNombre = null)
    {
        var q = _db.USUARIO.Include(x => x.ROL).Include(x => x.EMPLEADO.PERSONA).AsQueryable();
        if (!string.IsNullOrWhiteSpace(empleadoId)) q = q.Where(x => x.EMPLEADO_ID == empleadoId);
        if (!string.IsNullOrWhiteSpace(usuarioNombre)) q = q.Where(x => x.USUARIO_NOMBRE == usuarioNombre);

        var lista = q.Take(50).ToList()
            .Select(u => MapearDto(u, u.ROL?.ROL_NOMBRE, u.EMPLEADO));
        return Ok(lista);
    }

    // ================= helpers =================

    private static byte[] HashSha256(string input)
    {
        using (var sha = SHA256.Create())
            return sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
    }

    private string GenerarNuevoCodigo()
    {
        var ultimo = _db.USUARIO.OrderByDescending(u => u.USUARIO_ID).FirstOrDefault();
        if (ultimo != null && !string.IsNullOrEmpty(ultimo.USUARIO_ID) && ultimo.USUARIO_ID.Length == 10)
        {
            var num = ultimo.USUARIO_ID.Substring(4);
            var sig = int.Parse(num) + 1;
            return "USER" + sig.ToString("D6");
        }
        return "USER000001";
    }

    private static UsuarioDto MapearDto(USUARIO u, string rolNombre, EMPLEADO emp)
    {
        var nombreEmpleado = emp?.PERSONA == null ? null :
            string.Join(" ", new[]
            {
                emp.PERSONA.PERSONA_PRIMERNOMBRE,
                emp.PERSONA.PERSONA_SEGUNDONOMBRE,
                emp.PERSONA.PERSONA_TERCERNOMBRE,
                emp.PERSONA.PERSONA_PRIMERAPELLIDO,
                emp.PERSONA.PERSONA_SEGUNDOAPELLIDO,
                emp.PERSONA.PERSONA_APELLIDOCASADA
            }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return new UsuarioDto
        {
            usuarioId = u.USUARIO_ID,
            usuarioNombre = u.USUARIO_NOMBRE,
            fechaRegistro = u.USUARIO_FECHAREGISTRO,
            rolId = u.ROL_ID,
            rolNombre = rolNombre,
            empleadoId = u.EMPLEADO_ID,
            empleadoNombre = nombreEmpleado,
            estado = u.USUARIO_ESTADO
        };
    }

    private static System.Collections.Generic.List<string> Validar(UsuarioCrearDto dto)
    {
        var errores = new System.Collections.Generic.List<string>();
        if (dto == null) { errores.Add("Cuerpo vacío."); return errores; }
        if (string.IsNullOrWhiteSpace(dto.usuarioNombre)) errores.Add("usuarioNombre es requerido.");
        if (string.IsNullOrWhiteSpace(dto.contrasenaPlano)) errores.Add("contrasenaPlano es requerido.");
        if (string.IsNullOrWhiteSpace(dto.rolId)) errores.Add("rolId es requerido.");
        if (string.IsNullOrWhiteSpace(dto.empleadoId)) errores.Add("empleadoId es requerido.");
        if (!string.IsNullOrWhiteSpace(dto.usuarioEstado) && dto.usuarioEstado != "Activo" && dto.usuarioEstado != "Inactivo")
            errores.Add("usuarioEstado inválido (use 'Activo' o 'Inactivo').");
        return errores;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _db.Dispose();
        base.Dispose(disposing);
    }
}
