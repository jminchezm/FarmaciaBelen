using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    public class PermisosPorRolViewModel
    {
        public string ROL_ID { get; set; }
        public string ROL_NOMBRE { get; set; }
        public List<ModuloPermisoItem> Modulos { get; set; }
    }

}