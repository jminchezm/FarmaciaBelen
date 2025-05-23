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
    public class ClientesController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Clientes
        public ActionResult Index(string nombre, string codigo, string estado)
        {
            var clientes = db.CLIENTE.Include(e => e.PERSONA).AsQueryable();

            if (!string.IsNullOrEmpty(codigo))
                clientes = clientes.Where(e => e.CLIENTE_ID.Contains(codigo));

            if (!string.IsNullOrEmpty(nombre))
            {
                clientes = clientes.Where(e =>
                    (e.PERSONA.PERSONA_PRIMERNOMBRE + " " +
                     e.PERSONA.PERSONA_SEGUNDONOMBRE + " " +
                     e.PERSONA.PERSONA_PRIMERAPELLIDO + " " +
                     e.PERSONA.PERSONA_SEGUNDOAPELLIDO)
                     .Trim()
                     .ToLower()
                     .Contains(nombre.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(estado))
                clientes = clientes.Where(e => e.CLIENTE_ESTADO == estado);

            //if (fechaInicio.HasValue)
            //    empleados = empleados.Where(e => e.EMPLEADO_FECHAINGRESO >= fechaInicio.Value);

            //if (fechaFin.HasValue)
            //    empleados = empleados.Where(e => e.EMPLEADO_FECHAINGRESO <= fechaFin.Value);

            return View(clientes.ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //CLIENTE cLIENTE = db.CLIENTE.Find(id);

            var cliente = db.CLIENTE
                .Include(e => e.PERSONA)
                .FirstOrDefault(e => e.CLIENTE_ID == id);

            if (cliente == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ClienteViewModel
            {
                Persona = cliente.PERSONA,
                Cliente = cliente
            };
            return View(viewModel);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            var ultimoCliente = db.CLIENTE
               .OrderByDescending(e => e.PERSONA.PERSONA_ID)
               .FirstOrDefault();

            string nuevoCodigo;

            if (ultimoCliente != null && ultimoCliente.PERSONA.PERSONA_ID.Length == 10)
            {
                string numero = ultimoCliente.PERSONA.PERSONA_ID.Substring(4);
                int siguienteNumero = int.Parse(numero) + 1;
                nuevoCodigo = "CLIE" + siguienteNumero.ToString("D6");
            }
            else
            {
                nuevoCodigo = "CLIE000001";
            }

            // Construir el ViewModel con el nuevo código
            var viewModel = new ClienteViewModel
            {
                Persona = new PERSONA
                {
                    PERSONA_ID = nuevoCodigo
                },
                Cliente = new CLIENTE
                {
                    CLIENTE_ID = nuevoCodigo,
                    //EMPLEADO_ESTADO = "Activo"
                }
            };

            ViewBag.Creado = TempData["Creado"];

            return View(viewModel);
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "CLIENTE_ID,CLIENTE_NOTA,CLIENTE_ESTADO")] CLIENTE cLIENTE*/  ClienteViewModel viewModel)
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


            if (ModelState.IsValid)
            {
                try
                {


                    // Asignar el mismo ID a persona y empleado
                    string idGenerado = viewModel.Persona.PERSONA_ID;
                    viewModel.Cliente.CLIENTE_ID = idGenerado;
                    viewModel.Cliente.PERSONA = viewModel.Persona;
                    //viewModel.Persona.PERSONA_FECHAREGISTRO = DateTime.Now; //Fecha de registro al crear.
                    // Guardar persona
                    db.PERSONA.Add(viewModel.Persona);
                    db.SaveChanges();

                    // Guardar empleado
                    db.CLIENTE.Add(viewModel.Cliente);
                    db.SaveChanges();

                    TempData["Creado"] = true;
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.InnerException?.Message;
                    System.Diagnostics.Debug.WriteLine("ERROR INTERNO: " + innerMessage);
                    ModelState.AddModelError("", "Error al guardar el Cliente: " + (innerMessage ?? ex.Message));
                }
            }


            return View(viewModel);


        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();
            var cliente = db.CLIENTE.Include("PERSONA").FirstOrDefault(e => e.CLIENTE_ID == id);
            if (cliente == null) return HttpNotFound();



            var viewModel = new ClienteViewModel
            {
                Persona = cliente.PERSONA,
                Cliente = cliente
            };

            ViewBag.Editado = TempData["Editado"];
            return View(viewModel);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "CLIENTE_ID,CLIENTE_NOTA,CLIENTE_ESTADO")] CLIENTE cLIENTE*/ ClienteViewModel viewModel)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(cLIENTE).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //ViewBag.CLIENTE_ID = new SelectList(db.PERSONA, "PERSONA_ID", "PERSONA_PRIMERNOMBRE", cLIENTE.CLIENTE_ID);
            //return View(cLIENTE);

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

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener los datos actuales desde la base de datos
                    var personaBD = db.PERSONA.Find(viewModel.Persona.PERSONA_ID);
                    var clienteDB = db.CLIENTE.Find(viewModel.Cliente.CLIENTE_ID);

                    if (personaBD != null && clienteDB != null)
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
                        clienteDB.CLIENTE_NOTA = viewModel.Cliente.CLIENTE_NOTA;
                        clienteDB.CLIENTE_ESTADO = viewModel.Cliente.CLIENTE_ESTADO;
                        db.SaveChanges();

                        TempData["Editado"] = true;
                        return RedirectToAction("Edit", new { id = viewModel.Cliente.CLIENTE_ID });
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

            ViewBag.Puestos = new SelectList(db.PUESTO, "PUESTO_ID", "PUESTO_NOMBRE", viewModel.Cliente.CLIENTE_ID);
            return View(viewModel);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cliente = db.CLIENTE
                .FirstOrDefault(e => e.CLIENTE_ID == id);

            if (cliente == null)
                return HttpNotFound();

            var viewModel = new ClienteViewModel
            {
                Persona = cliente.PERSONA,
                Cliente = cliente
            };

            ViewBag.Desactivado = TempData["Desactivado"]; // importante para el modal
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ClienteViewModel viewModel)
        {
            var cliente = db.CLIENTE
                .FirstOrDefault(e => e.CLIENTE_ID == viewModel.Cliente.CLIENTE_ID);

            if (cliente == null)
                return HttpNotFound();

            try
            {
                // Cambiar estado a Inactivo
                cliente.CLIENTE_ESTADO = "Inactivo";
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Desactivado"] = true;

                // Redirigir a la misma vista Delete para mostrar el modal
                return RedirectToAction("Delete", new { id = cliente.CLIENTE_ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al desactivar el empleado: " + ex.Message);
            }

            // Si ocurre un error, recarga la vista con los datos del empleado
            var persona = db.PERSONA.Find(viewModel.Cliente.CLIENTE_ID);
            viewModel.Persona = persona;
            viewModel.Cliente = cliente;

            return View(viewModel);
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
