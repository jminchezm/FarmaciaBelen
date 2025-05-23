using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;
using FarmaciaBelen.Models;

namespace FarmaciaBelen.Controllers
{
    public class LoginController : Controller
    {
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Index(string usuario, string password)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                ViewBag.Mensaje = "Debe ingresar usuario y contraseña.";
                return View();
            }

            // Buscar el usuario
            var user = db.USUARIO.FirstOrDefault(u => u.USUARIO_NOMBRE == usuario);

            if (user == null)
            {
                ViewBag.Mensaje = "Usuario no encontrado.";
                return View();
            }

            // Validar la contraseña con SHA256
            if (!VerificarPasswordSHA256(password, user.USUARIO_CONTRASENA))
            {
                ViewBag.Mensaje = "Contraseña incorrecta.";
                return View();
            }

            // Autenticación exitosa
            Session["UsuarioID"] = user.USUARIO_ID;
            Session["Usuario"] = user.USUARIO_NOMBRE;
            Session["Rol"] = user.ROL.ROL_NOMBRE;
            Session["Empleado"] = user.EMPLEADO.PERSONA.PERSONA_PRIMERNOMBRE + " " + user.EMPLEADO.PERSONA.PERSONA_PRIMERAPELLIDO;

            return RedirectToAction("Dashboard", "Home");
        }

        private bool VerificarPasswordSHA256(string passwordIngresada, byte[] passwordEncriptada)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordIngresadaBytes = Encoding.UTF8.GetBytes(passwordIngresada);
                byte[] hashIngresada = sha256.ComputeHash(passwordIngresadaBytes);

                return passwordEncriptada.SequenceEqual(hashIngresada);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}