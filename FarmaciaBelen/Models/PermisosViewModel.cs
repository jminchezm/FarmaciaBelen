using System.Collections.Generic;
using System.Web.Mvc;

namespace FarmaciaBelen.Models
{
    public class ModuloPermisoItem
    {
        public string MODULO_ID { get; set; }
        public string MODULO_NOMBRE { get; set; }
        public bool PERMISOS_PUEDEACCEDER { get; set; }
    }

    public class PermisosViewModel
    {
        public string ROL_ID { get; set; }
        public List<ModuloPermisoItem> Modulos { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}




//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace FarmaciaBelen.Models
//{
//    //public class PermisosViewModel
//    //{
//    //}

//    public class PermisoModuloViewModel
//    {
//        public string ModuloId { get; set; }
//        public string ModuloNombre { get; set; }
//        public string ModuloDescripcion { get; set; }
//        public bool PuedeAcceder { get; set; }
//    }

//    public class PermisosViewModel
//    {
//        public string RolId { get; set; }
//        public string RolNombre { get; set; }
//        public List<PermisoModuloViewModel> PermisosPorModulo { get; set; }
//    }

//}