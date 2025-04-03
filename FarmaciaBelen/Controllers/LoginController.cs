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


//using System;
//using System.Linq;
//using System.Web.Mvc;
//using FarmaciaBelen.Models; // Importa tu modelo

//namespace FarmaciaBelen.Controllers
//{
//    public class LoginController : Controller
//    {
//        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities(); // Contexto de la base de datos

//        // GET: Login
//        public ActionResult Index()
//        {
//            return View();
//        }

//        // POST: Login (Validar credenciales)
//        [HttpPost]
//        public ActionResult Index(string usuario, string password)
//        {
//            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
//            {
//                ViewBag.Mensaje = "Debe ingresar usuario y contraseña.";
//                return View();
//            }

//            // Buscar el usuario en la base de datos
//            var user = db.USUARIO.FirstOrDefault(u => u.USUARIO_NOMBRE == usuario);

//            if (user == null)
//            {
//                ViewBag.Mensaje = "Usuario no encontrado.";
//                return View();
//            }

//            // Verificar la contraseña (suponiendo que está almacenada como byte[] encriptada)
//            if (!VerificarPassword(password, user.USUARIO_CONTRASENA))
//            {
//                ViewBag.Mensaje = "Contraseña incorrecta.";
//                return View();
//            }

//            // Si el usuario es válido, iniciar sesión
//            Session["Usuario"] = user.USUARIO_NOMBRE;
//            Session["Rol"] = user.ROL.ROL_NOMBRE;
//            @Session["Empleado"] = user.EMPLEADO.PERSONA.PERSONA_PRIMERNOMBRE + " " + user.EMPLEADO.PERSONA.PERSONA_PRIMERAPELLIDO;

//            return RedirectToAction("Dashboard", "Home"); // Redirige al Dashboard si es correcto
//        }

//        // Método para verificar la contraseña almacenada encriptada (debes modificarlo según tu esquema de cifrado)
//        private bool VerificarPassword(string passwordIngresada, byte[] passwordAlmacenado)
//        {
//            string passwordHashed = Convert.ToBase64String(passwordAlmacenado); // Simulación de conversión
//            return passwordHashed == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(passwordIngresada));
//        }

//        // Cerrar sesión
//        public ActionResult Logout()
//        {
//            Session.Clear();
//            return RedirectToAction("Index", "Login");
//        }
//    }
//}
