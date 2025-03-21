using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FarmaciaBelen.Controllers
{
    public class HomeController : Controller
    {
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
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Index", "Login"); // Redirige al login si no está autenticado
            }

            ViewBag.Usuario = Session["Usuario"];
            return View();
        }

        public ActionResult Inicio()
        {
            return View();
        }
    }
}