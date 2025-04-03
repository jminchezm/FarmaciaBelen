using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class UsuariosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Usuarios

        public ActionResult Index(string codigo, string nombre, string rol, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var usuarios = db.USUARIO
                .Include(u => u.EMPLEADO.PERSONA)
                .Include(u => u.ROL)
                .AsQueryable();

            // Buscar por nombre de usuario (usuario_id o nombre)
            if (!string.IsNullOrEmpty(codigo))
                usuarios = usuarios.Where(u => u.USUARIO_ID.Contains(codigo) || u.USUARIO_NOMBRE.Contains(codigo));

            // Buscar por nombre completo de la persona relacionada al empleado
            if (!string.IsNullOrEmpty(nombre))
            {
                usuarios = usuarios.Where(u =>
                    (u.EMPLEADO.PERSONA.PERSONA_PRIMERNOMBRE + " " +
                     u.EMPLEADO.PERSONA.PERSONA_SEGUNDONOMBRE + " " +
                     u.EMPLEADO.PERSONA.PERSONA_TERCERNOMBRE + " " +
                     u.EMPLEADO.PERSONA.PERSONA_PRIMERAPELLIDO + " " +
                     u.EMPLEADO.PERSONA.PERSONA_SEGUNDOAPELLIDO + " " +
                     u.EMPLEADO.PERSONA.PERSONA_APELLIDOCASADA)
                    .Trim()
                    .ToLower()
                    .Contains(nombre.Trim().ToLower()));
            }

            // Filtrar por rol del empleado
            if (!string.IsNullOrEmpty(rol))
                usuarios = usuarios.Where(u => u.ROL_ID == rol);

            //Filtrar el estado del empleado
            if (!string.IsNullOrEmpty(estado))
                usuarios = usuarios.Where(u => u.USUARIO_ESTADO == estado);

            // Rango de fechas basado en la fecha de ingreso del empleado
            if (fechaInicio.HasValue)
                usuarios = usuarios.Where(u => u.USUARIO_FECHAREGISTRO>= fechaInicio.Value);

            if (fechaFin.HasValue)
                usuarios = usuarios.Where(u => u.USUARIO_FECHAREGISTRO <= fechaFin.Value);

            // Para llenar el dropdown de roles
            ViewBag.Roles = db.ROL
                .Select(r => new SelectListItem
                {
                    Value = r.ROL_ID,
                    Text = r.ROL_NOMBRE
                }).ToList();

            return View(usuarios.ToList());
        }


        //public ActionResult Index()
        //{
        //    var uSUARIO = db.USUARIO.Include(u => u.EMPLEADO).Include(u => u.ROL);
        //    return View(uSUARIO.ToList());
        //}

        // GET: Usuarios/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO uSUARIO = db.USUARIO.Find(id);
            if (uSUARIO == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIO);
        }

        private byte[] EncriptarSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                return sha256.ComputeHash(inputBytes);
            }
        }

        private void CargarListas(UsuarioViewModel viewModel)
        {
            // Obtener IDs de empleados que ya están registrados como usuarios
            var empleadosConUsuario = db.USUARIO.Select(u => u.EMPLEADO_ID).ToList();

            // Filtrar empleados que NO están en la lista anterior
            var empleadosDisponibles = db.EMPLEADO
                .Include(e => e.PERSONA)
                .Where(e => !empleadosConUsuario.Contains(e.EMPLEADO_ID))
                .ToList();

            // Crear el SelectList para el combo
            ViewBag.EMPLEADO_ID = new SelectList(
                empleadosDisponibles.Select(e => new
                {
                    e.EMPLEADO_ID,
                    NombreCompleto = e.PERSONA.PERSONA_PRIMERNOMBRE + " " + e.PERSONA.PERSONA_PRIMERAPELLIDO
                }),
                "EMPLEADO_ID", "NombreCompleto", viewModel.EMPLEADO_ID
            );

            ViewBag.ROL_ID = new SelectList(db.ROL.ToList(), "ROL_ID", "ROL_NOMBRE", viewModel.ROL_ID);
        }




        // GET: Usuarios/Create

        public ActionResult Create()
        {
            var ultimoUsuario = db.USUARIO
                .OrderByDescending(u => u.USUARIO_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoUsuario != null && ultimoUsuario.USUARIO_ID.Length == 10)
            {
                string numero = ultimoUsuario.USUARIO_ID.Substring(4);
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "USER" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "USER000001";
            }

            var viewModel = new UsuarioViewModel
            {
                USUARIO_ID = nuevoCodigo
            };

            CargarListas(viewModel);

            ViewBag.Creado = TempData["Creado"];
            return View(viewModel);
        }




        //public ActionResult Create()
        //{
        //    // Obtener el último código existente en PERSONA (que también sirve para EMPLEADO_ID)
        //    var ultimoUsuario = db.USUARIO
        //        .OrderByDescending(e => e.USUARIO_ID)
        //        .FirstOrDefault();

        //    string nuevoCodigo;

        //    if (ultimoUsuario != null && ultimoUsuario.USUARIO_ID.Length == 10)
        //    {
        //        string numero = ultimoUsuario.USUARIO_ID.Substring(4);
        //        int siguienteNumero = int.Parse(numero) + 1;
        //        nuevoCodigo = "USER" + siguienteNumero.ToString("D6");
        //    }
        //    else
        //    {
        //        nuevoCodigo = "USER000001";
        //    }

        //    // Construir el ViewModel con el nuevo código
        //    var viewModel = new UsuarioViewModel
        //    {
        //        Usuario = new PERSONA
        //        {
        //            PERSONA_ID = nuevoCodigo
        //        },
        //        Empleado = new EMPLEADO
        //        {
        //            EMPLEADO_ID = nuevoCodigo,
        //            EMPLEADO_ESTADO = "Activo"
        //        }
        //    };

        //    // SelectList para el dropdown de puestos
        //    ViewBag.Puestos = new SelectList(db.PUESTO.ToList(), "PUESTO_ID", "PUESTO_NOMBRE");
        //    //ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE");
        //    ViewBag.Creado = TempData["Creado"];

        //    return View(viewModel);
        //    //ViewBag.EMPLEADO_ID = new SelectList(db.EMPLEADO, "EMPLEADO_ID", "EMPLEADO_GENERO");
        //    //ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE");
        //    //return View();
        //}

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validación extra por seguridad
                    if (string.IsNullOrWhiteSpace(viewModel.NuevaContrasena) || string.IsNullOrWhiteSpace(viewModel.ConfirmarContrasena))
                    {
                        ModelState.AddModelError("", "Debe ingresar y confirmar la contraseña.");
                        CargarListas(viewModel);
                        return View(viewModel);
                    }

                    if (viewModel.NuevaContrasena != viewModel.ConfirmarContrasena)
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden.");
                        CargarListas(viewModel);
                        return View(viewModel);
                    }

                    // Encriptar la contraseña usando SHA256
                    byte[] passwordHash = EncriptarSHA256(viewModel.NuevaContrasena);

                    // Crear el nuevo usuario
                    USUARIO nuevoUsuario = new USUARIO
                    {
                        USUARIO_ID = viewModel.USUARIO_ID,
                        USUARIO_NOMBRE = viewModel.USUARIO_NOMBRE,
                        USUARIO_CONTRASENA = passwordHash,
                        USUARIO_FECHAREGISTRO = DateTime.Now,
                        EMPLEADO_ID = viewModel.EMPLEADO_ID,
                        ROL_ID = viewModel.ROL_ID,
                        USUARIO_ESTADO = viewModel.USUARIO_ESTADO
                    };

                    db.USUARIO.Add(nuevoUsuario);
                    db.SaveChanges();

                    TempData["Creado"] = true;
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar usuario: " + ex.Message);
                }
            }

            CargarListas(viewModel);
            return View(viewModel);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "USUARIO_ID,USUARIO_NOMBRE,USUARIO_CONTRASENA,USUARIO_FECHAREGISTRO,ROL_ID,EMPLEADO_ID,TOKEN_RECOVERY")] USUARIO uSUARIO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.USUARIO.Add(uSUARIO);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EMPLEADO_ID = new SelectList(db.EMPLEADO, "EMPLEADO_ID", "EMPLEADO_GENERO", uSUARIO.EMPLEADO_ID);
        //    ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", uSUARIO.ROL_ID);
        //    return View(uSUARIO);
        //}

        private byte[] HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(string id)
        {
            var usuario = db.USUARIO.Find(id);
            if (usuario == null)
                return HttpNotFound();

            var viewModel = new UsuarioViewModel
            {
                USUARIO_ID = usuario.USUARIO_ID,
                USUARIO_NOMBRE = usuario.USUARIO_NOMBRE,
                EMPLEADO_ID = usuario.EMPLEADO_ID,
                ROL_ID = usuario.ROL_ID,
                EMPLEADO_NOMBRE = string.Join(" ", new[] {
                    usuario.EMPLEADO.PERSONA.PERSONA_PRIMERNOMBRE,
                    usuario.EMPLEADO.PERSONA.PERSONA_SEGUNDONOMBRE,
                    usuario.EMPLEADO.PERSONA.PERSONA_TERCERNOMBRE,
                    usuario.EMPLEADO.PERSONA.PERSONA_PRIMERAPELLIDO,
                    usuario.EMPLEADO.PERSONA.PERSONA_SEGUNDOAPELLIDO,
                    usuario.EMPLEADO.PERSONA.PERSONA_APELLIDOCASADA
                }.Where(s => !string.IsNullOrWhiteSpace(s))),
                USUARIO_FECHAREGISTRO = (DateTime)usuario.USUARIO_FECHAREGISTRO
            };

            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", usuario.ROL_ID);
            //ViewBag.EMPLEADO_ID = new SelectList(db.EMPLEADO, "EMPLEADO_ID", "EMPLEADO_ID", usuario.EMPLEADO_ID);
            ViewBag.Editado = TempData["Editado"];

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = db.USUARIO.Find(viewModel.USUARIO_ID);
                if (usuario == null)
                    return HttpNotFound();

                usuario.USUARIO_NOMBRE = viewModel.USUARIO_NOMBRE;
                usuario.ROL_ID = viewModel.ROL_ID;
                //usuario.EMPLEADO_ID = viewModel.EMPLEADO_ID;
                usuario.USUARIO_ESTADO = viewModel.USUARIO_ESTADO;

                // Si hay nueva contraseña
                if (!string.IsNullOrEmpty(viewModel.NuevaContrasena))
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var bytes = Encoding.UTF8.GetBytes(viewModel.NuevaContrasena);
                        var hash = sha256.ComputeHash(bytes);
                        usuario.USUARIO_CONTRASENA = hash;
                    }
                }

                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Editado"] = true;
                return RedirectToAction("Edit", new { id = viewModel.USUARIO_ID });
            }

            ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", viewModel.ROL_ID);
            ViewBag.EMPLEADO_ID = new SelectList(db.EMPLEADO, "EMPLEADO_ID", "EMPLEADO_ID", viewModel.EMPLEADO_ID);
            return View(viewModel);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "USUARIO_ID,USUARIO_NOMBRE,USUARIO_CONTRASENA,USUARIO_FECHAREGISTRO,ROL_ID,EMPLEADO_ID,TOKEN_RECOVERY")] USUARIO uSUARIO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(uSUARIO).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.EMPLEADO_ID = new SelectList(db.EMPLEADO, "EMPLEADO_ID", "EMPLEADO_GENERO", uSUARIO.EMPLEADO_ID);
        //    ViewBag.ROL_ID = new SelectList(db.ROL, "ROL_ID", "ROL_NOMBRE", uSUARIO.ROL_ID);
        //    return View(uSUARIO);
        //}

        // GET: Usuarios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIO uSUARIO = db.USUARIO.Find(id);
            if (uSUARIO == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIO);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            USUARIO uSUARIO = db.USUARIO.Find(id);
            db.USUARIO.Remove(uSUARIO);
            db.SaveChanges();
            return RedirectToAction("Index");
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
