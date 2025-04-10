using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;
using FarmaciaBelen.Models;
using System.Web.UI.WebControls;

namespace FarmaciaBelen.Controllers
{
    public class AccessController : Controller
    {
        private string urlDomain = "https://localhost:44328/";
        private DBFARMACIABELENEntities db = new DBFARMACIABELENEntities();

        // GET: Access
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StartRecovery()
        {
            return View(new Models.ViewModel.RecoveryViewModel());
        }

        [HttpPost]
        public ActionResult StartRecovery(Models.ViewModel.RecoveryViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string token = GetSha256(Guid.NewGuid().ToString());

                var persona = db.PERSONA.FirstOrDefault(p => p.PERSONA_CORREO == model.EMAIL);
                if (persona == null)
                {
                    //ModelState.AddModelError("EMAIL", "El correo ingresado no está registrado.");
                    //return View(model);
                    return Json(new { success = false, message = "El correo ingresado no está registrado." });
                }

                var empleado = db.EMPLEADO.FirstOrDefault(e => e.EMPLEADO_ID == persona.PERSONA_ID);
                if (empleado == null)
                {
                    ModelState.AddModelError("EMAIL", "No se encontró el empleado asociado a este correo.");
                    return View(model);
                }

                var usuario = db.USUARIO.FirstOrDefault(u => u.EMPLEADO_ID == empleado.EMPLEADO_ID);
                if (usuario != null)
                {
                    usuario.TOKEN_RECOVERY = token;
                    usuario.TOKEN_EXPIRACION = DateTime.Now.AddMinutes(15);
                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //SendEmail(persona.PERSONA_CORREO, token);
                    string nombreEmpleado = $"{persona.PERSONA_PRIMERNOMBRE} {persona.PERSONA_PRIMERAPELLIDO}";

                    SendEmail(persona.PERSONA_CORREO, token, nombreEmpleado);
                    return Json(new { success = true, message = "Correo enviado exitosamente. Por favor, revise su bandeja de entrada." });
                }
                else
                {
                    ModelState.AddModelError("EMAIL", "No se encontró el usuario asociado al empleado.");
                    return View(model);
                }

                //ViewBag.Mensaje = "Se ha enviado un enlace de recuperación al correo ingresado.";

                //return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        public ActionResult Recovery(string token)
        {
            Models.ViewModel.RecoveryPasswordViewModel model = new Models.ViewModel.RecoveryPasswordViewModel();
            model.token = token;

            using (Models.DBFARMACIABELENEntities db = new Models.DBFARMACIABELENEntities())
            {
                if (string.IsNullOrWhiteSpace(model.token))
                {
                    TempData["ErrorMessage"] = "Tu token ha expirado";
                    return RedirectToAction("Index", "Login");
                }

                var usuario = db.USUARIO.FirstOrDefault(u => u.TOKEN_RECOVERY == model.token);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Tu token ha expirado";
                    return RedirectToAction("Index", "Login");
                }

                if (usuario.TOKEN_EXPIRACION < DateTime.Now)
                {
                    usuario.TOKEN_RECOVERY = null;
                    usuario.TOKEN_EXPIRACION = null;
                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["ErrorMessage"] = "Tu token ha expirado";
                    return RedirectToAction("Index", "Login");
                }
            }

            return View(model);
        }




















        //[HttpGet]
        //public ActionResult Recovery(string token)
        //{
        //    //return View(new Models.ViewModel.RecoveryPasswordViewModel { token = token });
        //    Models.ViewModel.RecoveryPasswordViewModel model = new Models.ViewModel.RecoveryPasswordViewModel();
        //    model.token = token;
        //    using (Models.DBFARMACIABELENEntities db= new Models.DBFARMACIABELENEntities())
        //    {
        //        if (model.token==null || model.token.Trim().Equals(""))
        //        {
        //            TempData["ErrorMessage"] = "Tu token ha expirado";
        //            return RedirectToAction("Index", "Login");
        //        }
        //        var usuario = db.USUARIO.FirstOrDefault(u => u.TOKEN_RECOVERY == model.token);
        //        if (usuario == null || usuario.TOKEN_EXPIRACION < DateTime.Now)
        //        {
        //            if (usuario == null)
        //            {
        //                usuario.TOKEN_RECOVERY = null;
        //                usuario.TOKEN_EXPIRACION = null;
        //                db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //            TempData["ErrorMessage"] = "Tu token ha expirado";
        //            return RedirectToAction("Index", "Login");
        //        }

        //    }
        //   return View(model);
        //}

        //[HttpPost]
        //public ActionResult Recovery(Models.ViewModel.RecoveryPasswordViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View(model);
        //        }

        //        var usuario = db.USUARIO.FirstOrDefault(u => u.TOKEN_RECOVERY == model.token);
        //        if (usuario != null)
        //        {
        //            usuario.USUARIO_CONTRASENA = Encoding.UTF8.GetBytes(GetSha256(model.Password));
        //            usuario.TOKEN_RECOVERY = null; // Eliminar el token tras el uso
        //            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();

        //            ViewBag.Message = "Contraseña modificada con éxito";
        //            return RedirectToAction("Index", "Login");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Token inválido o expirado.");
        //            return View(model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error: " + ex.Message);
        //        return View(model);
        //    }
        //}

        [HttpPost]
        public ActionResult Recovery(Models.ViewModel.RecoveryPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var usuario = db.USUARIO.FirstOrDefault(u => u.TOKEN_RECOVERY == model.token);

                if (usuario != null && usuario.TOKEN_EXPIRACION >= DateTime.Now)
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        usuario.USUARIO_CONTRASENA = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)); // Guardar como byte[]
                    }
                    //usuario.USUARIO_CONTRASENA = Encoding.UTF8.GetBytes(GetSha256(model.Password));
                    usuario.TOKEN_RECOVERY = null;
                    usuario.TOKEN_EXPIRACION = null;
                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Contraseña modificada con éxito.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Token inválido o expirado.";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(model);
            }
        }





        #region HELPERS

        private string GetSha256(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        //private void SendEmail(string emailDestino, string token)
        //{
        //    string emailOrigen = "infofarmaciabelen@gmail.com";
        //    string contraseña = "xgro sdwv rcui dwea";
        //    string url = urlDomain + "/Access/Recovery/?token=" + token;

        //    MailMessage mensaje = new MailMessage(emailOrigen, emailDestino, "Recuperación de contraseña",
        //        "<p>Correo para recuperación de contraseña</p><br>" +
        //        "<a href='" + url + "'>Haz clic aquí para recuperar tu contraseña</a>");
        //    mensaje.IsBodyHtml = true;

        //    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
        //    {
        //        smtp.EnableSsl = true;
        //        smtp.Port = 587;
        //        smtp.Credentials = new System.Net.NetworkCredential(emailOrigen, contraseña);
        //        smtp.Send(mensaje);
        //    }
        //}

        private void SendEmail(string emailDestino, string token, string nombreEmpleado)
        {
            string emailOrigen = "infofarmaciabelen@gmail.com";
            string contraseña = "xgro sdwv rcui dwea";
            string url = urlDomain + "/Access/Recovery/?token=" + token;

            string cuerpoCorreo = $@"
        <html>
        <head>
    <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; text-align: center; padding: 20px; }}
            .container {{ max-width: 500px; background: white; padding: 20px; border-radius: 8px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1); margin: auto; }}
            .banner {{ background-color: #0367A6; padding: 15px; color: white; font-size: 24px; font-weight: bold; display: flex; align-items: center; justify-content: center; border-radius: 8px 8px 0 0; }}
            .logo {{ width: 50px; height: 50px; margin-right: 10px; }}
            .content {{ padding: 20px; text-align: left; color: #333; }}
            .button {{ display: block; width: 200px; margin: 20px auto; padding: 10px; background-color: #049DBF; color: #FFFFFF !important; text-decoration: none; font-size: 16px; border-radius: 5px; text-align: center; }}
            .footer {{ font-size: 12px; color: #777; margin-top: 20px; }}
        </style>
</head>
<body>
    <div class='container'>
        <div class='banner'>
            <img src='https://i.postimg.cc/66mvTHBt/logo-farmacia-Bel-n.png' class='logo' alt='Farmacia Belén'>
            Farmacia Belén
        </div>
        <div class='content'>
                <p><strong>Hola {nombreEmpleado}.</strong></p>
            <p>Hemos recibido una solicitud para restablecer tu contraseña. Si no realizaste esta solicitud, ignora este mensaje.</p>
            <p>Para restablecer tu contraseña, haz clic en el siguiente botón:</p>
            <a class='button' href='" + url + @"' target='_blank'>Recuperar Contraseña</a>
            <p>Si el botón no funciona, copia y pega este enlace en tu navegador:</p>
            <p><a href='" + url + @"'>" + url + @"</a></p>
            <p class='footer'>Este es un mensaje automático, por favor no respondas a este correo.</p>
        </div>
    </div>
</body>
        </html>";

            MailMessage mensaje = new MailMessage(emailOrigen, emailDestino, "Recuperación de Contraseña", cuerpoCorreo);
            mensaje.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(emailOrigen, contraseña);
                smtp.Send(mensaje);
            }
        }



        #endregion
    }
}
