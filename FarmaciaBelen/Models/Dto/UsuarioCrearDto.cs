using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class UsuarioCrearDto
    {

        // opcional: si lo quieres autogenerar como en tu MVC, deja null
        public string usuarioId { get; set; }

        public string usuarioNombre { get; set; }
        public string contrasenaPlano { get; set; }   // viene en texto plano y el API lo hashea
        public string rolId { get; set; }
        public string empleadoId { get; set; }
        public string usuarioEstado { get; set; }     // "Activo" / "Inactivo"

    }
}