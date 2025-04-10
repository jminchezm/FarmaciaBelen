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
    public class EmpleadosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Empleados

        public ActionResult Index(string codigo, string nombre, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var empleados = db.EMPLEADO.Include(e => e.PERSONA).Include(e => e.PUESTO).AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                empleados = empleados.Where(e => e.EMPLEADO_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
            {
                empleados = empleados.Where(e =>
                    (e.PERSONA.PERSONA_PRIMERNOMBRE + " " +
                     e.PERSONA.PERSONA_SEGUNDONOMBRE + " " +
                     e.PERSONA.PERSONA_PRIMERAPELLIDO + " " +
                     e.PERSONA.PERSONA_SEGUNDOAPELLIDO)
                     .Trim()
                     .ToLower()
                     .Contains(nombre.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(estado))
                empleados = empleados.Where(e => e.EMPLEADO_ESTADO == estado);

            if (fechaInicio.HasValue)
                empleados = empleados.Where(e => e.EMPLEADO_FECHAINGRESO >= fechaInicio.Value);

            if (fechaFin.HasValue)
                empleados = empleados.Where(e => e.EMPLEADO_FECHAINGRESO <= fechaFin.Value);

            return View(empleados.ToList());
        }

        //public ActionResult Index(string codigo, string nombre, string estado)
        //{
        //    var empleados = db.EMPLEADO.AsQueryable();

        //    if (!string.IsNullOrEmpty(codigo))
        //        empleados = empleados.Where(e => e.EMPLEADO_ID.Contains(codigo)); // ← usa Contains para búsquedas parciales

        //    if (!string.IsNullOrEmpty(nombre))
        //        empleados = empleados.Where(e => e.PERSONA.PERSONA_PRIMERNOMBRE.Contains(nombre));

        //    if (!string.IsNullOrEmpty(estado))
        //        empleados = empleados.Where(e => e.EMPLEADO_ESTADO == estado);

        //    return View(empleados.ToList());
        //    //var eMPLEADO = db.EMPLEADO.Include(e => e.PERSONA).Include(e => e.PUESTO);
        //    //return View(eMPLEADO.ToList());
        //}

        // GET: Empleados/Details/5

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Buscar el empleado e incluir la relación con persona y puesto
            var empleado = db.EMPLEADO
                .Include(e => e.PERSONA)
                .Include(e => e.PUESTO)
                .FirstOrDefault(e => e.EMPLEADO_ID == id);

            if (empleado == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EmpleadoViewModel
            {
                Persona = empleado.PERSONA,
                Empleado = empleado
            };

            return View(viewModel);
        }

        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
        //    if (eMPLEADO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(eMPLEADO);
        //}

        // GET: Empleados/Create
        public ActionResult Create()
        {
            // Obtener el último código existente en PERSONA (que también sirve para EMPLEADO_ID)
            var ultimoEmpleado = db.EMPLEADO
                .OrderByDescending(e => e.PERSONA.PERSONA_ID)
                .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoEmpleado != null && ultimoEmpleado.PERSONA.PERSONA_ID.Length == 10)
            {
                string numero = ultimoEmpleado.PERSONA.PERSONA_ID.Substring(4);
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "PERS" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "PERS000001";
            }

            // Construir el ViewModel con el nuevo código
            var viewModel = new EmpleadoViewModel
            {
                Persona = new PERSONA
                {
                    PERSONA_ID = nuevoCodigo
                },
                Empleado = new EMPLEADO
                {
                    EMPLEADO_ID = nuevoCodigo,
                    //EMPLEADO_ESTADO = "Activo"
                }
            };

            // SelectList para el dropdown de puestos
            ViewBag.Puestos = new SelectList(db.PUESTO.Where(p=> p.PUESTO_ESTADO == "Activo").ToList(), "PUESTO_ID", "PUESTO_NOMBRE");
            //ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE");
            ViewBag.Creado = TempData["Creado"];

            return View(viewModel);
        }

        //public ActionResult Create()
        //{
        //    // Obtener el último código existente
        //    var ultimoEmpleado = db.EMPLEADO
        //        .OrderByDescending(e => e.PERSONA.PERSONA_ID)
        //        .FirstOrDefault();

        //    string nuevoCodigo;

        //    if (ultimoEmpleado != null && ultimoEmpleado.PERSONA.PERSONA_ID.Length == 10)
        //    {
        //        // Extraer el número del código (últimos 6 caracteres)
        //        string numero = ultimoEmpleado.PERSONA.PERSONA_ID.Substring(4); // "000004"
        //        int siguienteNumero = int.Parse(numero) + 1;

        //        // Formatear el nuevo código con ceros y prefijo
        //        nuevoCodigo = "PERS" + siguienteNumero.ToString("D6");
        //    }
        //    else
        //    {
        //        // Si no hay registros o el formato es incorrecto, empezar con el primero
        //        nuevoCodigo = "PERS000001";
        //    }

        //    var nuevoEmpleado = new EMPLEADO
        //    {
        //        EMPLEADO_ID = nuevoCodigo,
        //        EMPLEADO_ESTADO = "Activo" // Estado por defecto
        //    };

        //    ViewBag.EMPLEADO_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE");
        //    ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE");

        //    ViewBag.Creado = TempData["Creado"]; //esto permite mostrar el modal
        //    return View(nuevoEmpleado);
        //    //ViewBag.EMPLEADO_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE");
        //    //ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE");
        //    //return View();
        //}

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmpleadoViewModel viewModel)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                System.Diagnostics.Debug.WriteLine("ID generado: " + viewModel.Persona.PERSONA_ID);
            }

            // VALIDACIONES DE NIT
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_NIT))
            {
                bool nitExiste = db.PERSONA.Any(p => p.PERSONA_NIT == viewModel.Persona.PERSONA_NIT);
                if (nitExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_NIT", "El NIT ya está registrado.");
                }
            }

            // VALIDACIONES DE CUI
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_CUI))
            {
                bool cuiExiste = db.PERSONA.Any(p => p.PERSONA_CUI == viewModel.Persona.PERSONA_CUI);
                if (cuiExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_CUI", "El CUI ya está registrado.");
                }
            }

            // VALIDACIÓN DE CORREO (ignorar si es nulo o vacío)
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_CORREO))
            {
                bool correoExiste = db.PERSONA.Any(p => p.PERSONA_CORREO == viewModel.Persona.PERSONA_CORREO);
                if (correoExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_CORREO", "El correo ya está registrado.");
                }
            }

            // VALIDACIÓN DE EDAD MÍNIMA (15 años)
            if (viewModel.Empleado.EMPLEADO_FECHANACIMIENTO != null)
            {
                var fechaNacimiento = viewModel.Empleado.EMPLEADO_FECHANACIMIENTO;
                var edad = DateTime.Today.Year - fechaNacimiento.Year;
                if (fechaNacimiento > DateTime.Today.AddYears(-edad)) edad--; // corregir si aún no ha cumplido años este año

                if (edad < 15)
                {
                    ModelState.AddModelError("Empleado.EMPLEADO_FECHANACIMIENTO", "El empleado debe tener al menos 15 años.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Si alguna validación falló, retornar la vista
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Puestos = new SelectList(db.PUESTO.Where(p => p.PUESTO_ESTADO == "Activo").ToList(), "PUESTO_ID", "PUESTO_NOMBRE", viewModel.Empleado.PUESTO_ID);
                        return View(viewModel);
                    }

                    // Asignar el mismo ID a persona y empleado
                    string idGenerado = viewModel.Persona.PERSONA_ID;
                    viewModel.Empleado.EMPLEADO_ID = idGenerado;
                    viewModel.Empleado.PERSONA = viewModel.Persona;
                    //viewModel.Persona.PERSONA_FECHAREGISTRO = DateTime.Now; //Fecha de registro al crear.
                    // Guardar persona
                    db.PERSONA.Add(viewModel.Persona);
                    db.SaveChanges();

                    // Guardar empleado
                    db.EMPLEADO.Add(viewModel.Empleado);
                    db.SaveChanges();

                    TempData["Creado"] = true;
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.InnerException?.Message;
                    System.Diagnostics.Debug.WriteLine("ERROR INTERNO: " + innerMessage);
                    ModelState.AddModelError("", "Error al guardar el empleado: " + (innerMessage ?? ex.Message));
                }
            }

            // Si hubo errores de validación o excepciones
            ViewBag.Puestos = new SelectList(db.PUESTO.Where(p => p.PUESTO_ESTADO == "Activo").ToList(), "PUESTO_ID", "PUESTO_NOMBRE", viewModel.Empleado.PUESTO_ID);
            return View(viewModel);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "EMPLEADO_ID,EMPLEADO_FECHANACIMIENTO,EMPLEADO_FECHAINGRESO,EMPLEADO_GENERO,EMPLEADO_ESTADO,PUESTO_ID")] EMPLEADO eMPLEADO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.EMPLEADO.Add(eMPLEADO);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EMPLEADO_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE", eMPLEADO.EMPLEADO_ID);
        //    ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", eMPLEADO.PUESTO_ID);
        //    return View(eMPLEADO);
        //}

        // GET: Empleados/Edit/5

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            var empleado = db.EMPLEADO.Include("PERSONA").FirstOrDefault(e => e.EMPLEADO_ID == id);
            if (empleado == null) return HttpNotFound();

            var viewModel = new EmpleadoViewModel
            {
                Persona = empleado.PERSONA,
                Empleado = empleado
            };

            ViewBag.Puestos = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", empleado.PUESTO_ID);
            ViewBag.Editado = TempData["Editado"];

            return View(viewModel);
        }


        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
        //    if (eMPLEADO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.EMPLEADO_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE", eMPLEADO.EMPLEADO_ID);
        //    ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", eMPLEADO.PUESTO_ID);
        //    return View(eMPLEADO);
        //}

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmpleadoViewModel viewModel)
        {
            // VALIDACIONES DE NIT
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_NIT))
            {
                bool nitExiste = db.PERSONA.Any(p =>
                    p.PERSONA_NIT == viewModel.Persona.PERSONA_NIT &&
                    p.PERSONA_ID != viewModel.Persona.PERSONA_ID); // excluir al mismo empleado

                if (nitExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_NIT", "El NIT ya está registrado por otro empleado.");
                }
            }

            // VALIDACIONES DE CUI
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_CUI))
            {
                bool cuiExiste = db.PERSONA.Any(p =>
                    p.PERSONA_CUI == viewModel.Persona.PERSONA_CUI &&
                    p.PERSONA_ID != viewModel.Persona.PERSONA_ID);

                if (cuiExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_CUI", "El CUI ya está registrado por otro empleado.");
                }
            }

            // VALIDACIÓN DE CORREO
            if (!string.IsNullOrWhiteSpace(viewModel.Persona.PERSONA_CORREO))
            {
                bool correoExiste = db.PERSONA.Any(p =>
                    p.PERSONA_CORREO == viewModel.Persona.PERSONA_CORREO &&
                    p.PERSONA_ID != viewModel.Persona.PERSONA_ID);

                if (correoExiste)
                {
                    ModelState.AddModelError("Persona.PERSONA_CORREO", "El correo ya está registrado por otro empleado.");
                }
            }

            // VALIDACIÓN DE EDAD MÍNIMA (15 años)
            if (viewModel.Empleado.EMPLEADO_FECHANACIMIENTO != null)
            {
                var fechaNacimiento = viewModel.Empleado.EMPLEADO_FECHANACIMIENTO;
                var edad = DateTime.Today.Year - fechaNacimiento.Year;
                if (fechaNacimiento > DateTime.Today.AddYears(-edad)) edad--; // corregir si aún no ha cumplido años este año

                if (edad < 15)
                {
                    ModelState.AddModelError("Empleado.EMPLEADO_FECHANACIMIENTO", "El empleado debe tener al menos 15 años.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener los datos actuales desde la base de datos
                    var personaBD = db.PERSONA.Find(viewModel.Persona.PERSONA_ID);
                    var empleadoBD = db.EMPLEADO.Find(viewModel.Empleado.EMPLEADO_ID);

                    if (personaBD != null && empleadoBD != null)
                    {
                        // Actualizar manualmente solo los campos permitidos de Persona
                        personaBD.PERSONA_PRIMERNOMBRE = viewModel.Persona.PERSONA_PRIMERNOMBRE;
                        personaBD.PERSONA_SEGUNDONOMBRE = viewModel.Persona.PERSONA_SEGUNDONOMBRE;
                        personaBD.PERSONA_TERCERNOMBRE = viewModel.Persona.PERSONA_TERCERNOMBRE;
                        personaBD.PERSONA_PRIMERAPELLIDO = viewModel.Persona.PERSONA_PRIMERAPELLIDO;
                        personaBD.PERSONA_SEGUNDOAPELLIDO = viewModel.Persona.PERSONA_SEGUNDOAPELLIDO;
                        personaBD.PERSONA_APELLIDOCASADA = viewModel.Persona.PERSONA_APELLIDOCASADA;
                        personaBD.PERSONA_DIRECCION = viewModel.Persona.PERSONA_DIRECCION;
                        personaBD.PERSONA_NIT = viewModel.Persona.PERSONA_NIT;
                        personaBD.PERSONA_CUI = viewModel.Persona.PERSONA_CUI;
                        personaBD.PERSONA_TELEFONOCASA = viewModel.Persona.PERSONA_TELEFONOCASA;
                        personaBD.PERSONA_TELEFONOMOVIL = viewModel.Persona.PERSONA_TELEFONOMOVIL;
                        personaBD.PERSONA_CORREO = viewModel.Persona.PERSONA_CORREO;
                        // NO actualizar PERSONA_FECHAREGISTRO

                        // Actualizar manualmente los campos del empleado
                        empleadoBD.EMPLEADO_FECHANACIMIENTO = viewModel.Empleado.EMPLEADO_FECHANACIMIENTO;
                        empleadoBD.EMPLEADO_FECHAINGRESO = viewModel.Empleado.EMPLEADO_FECHAINGRESO;
                        empleadoBD.EMPLEADO_GENERO = viewModel.Empleado.EMPLEADO_GENERO;
                        empleadoBD.EMPLEADO_ESTADO = viewModel.Empleado.EMPLEADO_ESTADO;
                        empleadoBD.PUESTO_ID = viewModel.Empleado.PUESTO_ID;

                        db.SaveChanges();

                        TempData["Editado"] = true;
                        return RedirectToAction("Edit", new { id = viewModel.Empleado.EMPLEADO_ID });
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se encontró al empleado o persona.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
                }
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        db.Entry(viewModel.Persona).State = EntityState.Modified;
            //        db.Entry(viewModel.Empleado).State = EntityState.Modified;

            //        db.SaveChanges();

            //        TempData["Editado"] = true;
            //        return RedirectToAction("Edit", new { id = viewModel.Empleado.EMPLEADO_ID });
            //        //return RedirectToAction("Index");
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            //    }
            //}

            ViewBag.Puestos = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", viewModel.Empleado.PUESTO_ID);
            return View(viewModel);
        }
        //public ActionResult Edit([Bind(Include = "EMPLEADO_ID,EMPLEADO_FECHANACIMIENTO,EMPLEADO_FECHAINGRESO,EMPLEADO_GENERO,EMPLEADO_ESTADO,PUESTO_ID")] EMPLEADO eMPLEADO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(eMPLEADO).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.EMPLEADO_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE", eMPLEADO.EMPLEADO_ID);
        //    ViewBag.PUESTO_ID = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", eMPLEADO.PUESTO_ID);
        //    return View(eMPLEADO);
        //}

        // GET: Empleados/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var empleado = db.EMPLEADO
                .Include(e => e.PERSONA)
                .Include(e => e.PUESTO)
                .FirstOrDefault(e => e.EMPLEADO_ID == id);

            if (empleado == null)
                return HttpNotFound();

            var viewModel = new EmpleadoViewModel
            {
                Persona = empleado.PERSONA,
                Empleado = empleado
            };

            ViewBag.Desactivado = TempData["Desactivado"]; // importante para el modal
            return View(viewModel);
        }


        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
        //    if (eMPLEADO == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(eMPLEADO);
        //}

        // POST: Empleados/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(EmpleadoViewModel viewModel)
        {
            var empleado = db.EMPLEADO
                .Include(e => e.PERSONA)
                .Include(e => e.PUESTO)
                .FirstOrDefault(e => e.EMPLEADO_ID == viewModel.Empleado.EMPLEADO_ID);

            if (empleado == null)
                return HttpNotFound();

            try
            {
                // Cambiar estado a Inactivo
                empleado.EMPLEADO_ESTADO = "Inactivo";
                db.Entry(empleado).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Desactivado"] = true;

                // Redirigir a la misma vista Delete para mostrar el modal
                return RedirectToAction("Delete", new { id = empleado.EMPLEADO_ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al desactivar el empleado: " + ex.Message);
            }

            // Si ocurre un error, recarga la vista con los datos del empleado
            var persona = db.PERSONA.Find(viewModel.Empleado.EMPLEADO_ID);
            viewModel.Persona = persona;
            viewModel.Empleado = empleado;

            return View(viewModel);
        }


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    EMPLEADO eMPLEADO = db.EMPLEADO.Find(id);
        //    db.EMPLEADO.Remove(eMPLEADO);
        //    db.SaveChanges();
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
