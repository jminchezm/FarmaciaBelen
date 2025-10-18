using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class EmpleadoDto
    {

        public string id { get; set; }                // EMPLEADO_ID
        public string primerNombre { get; set; }      // PERSONA_PRIMERNOMBRE
        public string segundoNombre { get; set; }     // PERSONA_SEGUNDONOMBRE
        public string primerApellido { get; set; }    // PERSONA_PRIMERAPELLIDO
        public string segundoApellido { get; set; }   // PERSONA_SEGUNDOAPELLIDO
        public string correo { get; set; }            // PERSONA_CORREO
        public string telefonoCasa { get; set; }      // PERSONA_TELEFONOCASA
        public string telefonoMovil { get; set; }     // PERSONA_TELEFONOMOVIL
        public string puesto { get; set; }            // PUESTO.PUESTO_NOMBRE
        public string estado { get; set; }            // EMPLEADO_ESTADO
        public string nombreCompleto { get; set; }

    }
}