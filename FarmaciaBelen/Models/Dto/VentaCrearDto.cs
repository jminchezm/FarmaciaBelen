using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class VentaCrearDto
    {
        public string clienteId { get; set; }
        public string usuarioId { get; set; }
        public DateTime? fecha { get; set; }          // opcional; se usa DateTime.Now si viene nula
        public List<DetalleVentaCrearDto> detalles { get; set; }
    }
}