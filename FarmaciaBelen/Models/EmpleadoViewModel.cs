using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    public class EmpleadoViewModel
    {
        // Datos de la Persona
        public PERSONA Persona { get; set; }

        // Datos del Empleado
        public EMPLEADO Empleado { get; set; }

        public EmpleadoViewModel()
        {
            Persona = new PERSONA
            {
                PERSONA_FECHAREGISTRO = DateTime.Now
            };

            Empleado = new EMPLEADO
            {
                EMPLEADO_ESTADO = "Activo",
                EMPLEADO_FECHAINGRESO = DateTime.Now
            };
        }
    }
}