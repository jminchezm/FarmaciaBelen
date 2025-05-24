using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FarmaciaBelen.Filters
{
    public class PermisoModuloAttribute : AuthorizeAttribute
    {
        private readonly string _moduloId;

        public PermisoModuloAttribute(string moduloId)
        {
            _moduloId = moduloId;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var permisos = httpContext.Session["PermisosModulos"] as List<string>;

            if (permisos == null)
                return false;

            return permisos.Contains(_moduloId);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/SinPermiso");
        }
    }
}
