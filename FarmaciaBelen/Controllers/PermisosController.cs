using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class PermisosController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Permisos
        public ActionResult Index()
        {
            var roles = db.ROL.ToList();
            var permisos = db.PERMISOS.Include("MODULO").ToList();

            var viewModel = roles.Select(rol => new FarmaciaBelen.Models.PermisosPorRolViewModel
            {
                ROL_ID = rol.ROL_ID,
                ROL_NOMBRE = rol.ROL_NOMBRE,
                Modulos = permisos
                    .Where(p => p.ROL_ID == rol.ROL_ID && p.PERMISOS_PUEDEACCEDER)
                    .Select(p => new FarmaciaBelen.Models.ModuloPermisoItem
                    {
                        MODULO_ID = p.MODULO_ID,
                        MODULO_NOMBRE = p.MODULO.MODULO_NOMBRE,
                        PERMISOS_PUEDEACCEDER = true
                    }).ToList()
            }).ToList();

            return View(viewModel);
        }


        //public ActionResult Index()
        //{
        //    var permisos = db.PERMISOS.Include("ROL").Include("MODULO").ToList();
        //    return View(permisos);
        //}

        // GET: Permisos/Create
        public ActionResult Create()
        {
            var viewModel = new PermisosViewModel
            {
                Roles = db.ROL.Select(r => new SelectListItem
                {
                    Value = r.ROL_ID,
                    Text = r.ROL_NOMBRE
                }).ToList(),
                Modulos = db.MODULO.Select(m => new ModuloPermisoItem
                {
                    MODULO_ID = m.MODULO_ID,
                    MODULO_NOMBRE = m.MODULO_NOMBRE,
                    PERMISOS_PUEDEACCEDER = false
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PermisosViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Eliminar permisos existentes del rol
                    var permisosExistentes = db.PERMISOS.Where(p => p.ROL_ID == model.ROL_ID).ToList();
                    db.PERMISOS.RemoveRange(permisosExistentes);

                    // Obtener último ID numérico
                    string ultimoID = db.PERMISOS
                        .OrderByDescending(p => p.PERMISOS_ID)
                        .Select(p => p.PERMISOS_ID)
                        .FirstOrDefault();

                    int numero = 1;
                    if (!string.IsNullOrEmpty(ultimoID) && ultimoID.StartsWith("PERMISO"))
                    {
                        string numeroStr = ultimoID.Substring(7);
                        int.TryParse(numeroStr, out numero);
                        numero++;
                    }

                    // Guardar nuevos permisos seleccionados
                    foreach (var modulo in model.Modulos)
                    {
                        if (modulo.PERMISOS_PUEDEACCEDER)
                        {
                            string nuevoID = "PERMISO" + numero.ToString("D3");
                            numero++;

                            var permiso = new PERMISOS
                            {
                                PERMISOS_ID = nuevoID,
                                ROL_ID = model.ROL_ID,
                                MODULO_ID = modulo.MODULO_ID,
                                PERMISOS_PUEDEACCEDER = true
                            };
                            db.PERMISOS.Add(permiso);
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            ModelState.AddModelError("", $"{validationError.PropertyName}: {validationError.ErrorMessage}");
                        }
                    }
                }
            }

            // ⚠️ Este bloque corrige el error de LINQ a EF
            // Recargar combos si hay error
            model.Roles = db.ROL.Select(r => new SelectListItem
            {
                Value = r.ROL_ID,
                Text = r.ROL_NOMBRE
            }).ToList();

            var todosLosModulos = db.MODULO.ToList();
            var modulosSeleccionados = model.Modulos ?? new List<ModuloPermisoItem>();

            model.Modulos = todosLosModulos.Select(m => new ModuloPermisoItem
            {
                MODULO_ID = m.MODULO_ID,
                MODULO_NOMBRE = m.MODULO_NOMBRE,
                PERMISOS_PUEDEACCEDER = modulosSeleccionados.Any(x => x.MODULO_ID == m.MODULO_ID && x.PERMISOS_PUEDEACCEDER)
            }).ToList();

            return View(model);
        }

        // GET: Permisos/Edit/ROL001
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            var rol = db.ROL.Find(id);
            if (rol == null) return HttpNotFound();

            var todosLosModulos = db.MODULO.ToList();
            var permisosExistentes = db.PERMISOS.Where(p => p.ROL_ID == id).ToList();

            var viewModel = new PermisosViewModel
            {
                ROL_ID = id,
                Roles = db.ROL.Select(r => new SelectListItem
                {
                    Value = r.ROL_ID,
                    Text = r.ROL_NOMBRE,
                    Selected = r.ROL_ID == id
                }).ToList(),
                Modulos = todosLosModulos.Select(m => new ModuloPermisoItem
                {
                    MODULO_ID = m.MODULO_ID,
                    MODULO_NOMBRE = m.MODULO_NOMBRE,
                    PERMISOS_PUEDEACCEDER = permisosExistentes.Any(p => p.MODULO_ID == m.MODULO_ID && p.PERMISOS_PUEDEACCEDER)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Permisos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PermisosViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var permisosAnteriores = db.PERMISOS.Where(p => p.ROL_ID == model.ROL_ID).ToList();
                    db.PERMISOS.RemoveRange(permisosAnteriores);

                    int ultimoId = db.PERMISOS.Select(p => p.PERMISOS_ID).ToList()
                                        .Where(id => id.StartsWith("PERMISO"))
                                        .Select(id => int.TryParse(id.Substring(7), out int n) ? n : 0)
                                        .DefaultIfEmpty(0)
                                        .Max();

                    foreach (var modulo in model.Modulos)
                    {
                        if (modulo.PERMISOS_PUEDEACCEDER)
                        {
                            ultimoId++;
                            var permiso = new PERMISOS
                            {
                                PERMISOS_ID = "PERMISO" + ultimoId.ToString("D3"),
                                ROL_ID = model.ROL_ID,
                                MODULO_ID = modulo.MODULO_ID,
                                PERMISOS_PUEDEACCEDER = true
                            };
                            db.PERMISOS.Add(permiso);
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar los permisos: " + ex.Message);
                }
            }

            model.Roles = db.ROL.Select(r => new SelectListItem
            {
                Value = r.ROL_ID,
                Text = r.ROL_NOMBRE
            }).ToList();

            var todosModulos = db.MODULO.ToList();
            var modulosSeleccionados = model.Modulos ?? new List<ModuloPermisoItem>();

            model.Modulos = todosModulos.Select(m => new ModuloPermisoItem
            {
                MODULO_ID = m.MODULO_ID,
                MODULO_NOMBRE = m.MODULO_NOMBRE,
                PERMISOS_PUEDEACCEDER = modulosSeleccionados.Any(x => x.MODULO_ID == m.MODULO_ID && x.PERMISOS_PUEDEACCEDER)
            }).ToList();

            return View(model);
        }

    }
}
