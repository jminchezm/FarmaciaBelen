using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class HomeController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Index", "Login");

            string usuarioID = Session["UsuarioID"].ToString();

            var usuario = db.USUARIO.Include("ROL").Include("EMPLEADO")
                                    .FirstOrDefault(u => u.USUARIO_ID == usuarioID);

            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var modulosPermitidos = db.PERMISOS
                .Where(p => p.ROL_ID == usuario.ROL_ID && p.PERMISOS_PUEDEACCEDER)
                .Select(p => p.MODULO_ID)
                .ToList();

            Session["PermisosModulos"] = modulosPermitidos;
            Session["Empleado"] = usuario.EMPLEADO?.PERSONA.PERSONA_PRIMERNOMBRE + " " + usuario.EMPLEADO?.PERSONA.PERSONA_PRIMERAPELLIDO;
            Session["Rol"] = usuario.ROL?.ROL_NOMBRE;
            Session["RolID"] = usuario.ROL_ID;

            return View();
        }


        public ActionResult Inicio()
        {
            return View();
        }
    }
}