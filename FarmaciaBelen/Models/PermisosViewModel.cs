using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    //public class PermisosViewModel
    //{
    //}

    public class PermisoModuloViewModel
    {
        public string ModuloId { get; set; }
        public string ModuloNombre { get; set; }
        public string ModuloDescripcion { get; set; }
        public bool PuedeAcceder { get; set; }
    }

    public class PermisosViewModel
    {
        public string RolId { get; set; }
        public string RolNombre { get; set; }
        public List<PermisoModuloViewModel> PermisosPorModulo { get; set; }
    }

}