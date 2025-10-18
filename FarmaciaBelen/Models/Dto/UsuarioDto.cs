using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class UsuarioDto
    {

        public string usuarioId { get; set; }
        public string usuarioNombre { get; set; }
        public DateTime? fechaRegistro { get; set; }
        public string rolId { get; set; }
        public string rolNombre { get; set; }
        public string empleadoId { get; set; }
        public string empleadoNombre { get; set; }
        public string estado { get; set; }

    }
}