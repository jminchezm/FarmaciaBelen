using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class VentaDto
    {
        public string ventaId { get; set; }
        public DateTime? fecha { get; set; }
        public decimal total { get; set; }
        public string clienteId { get; set; }
        public string clienteNombre { get; set; }
        public string usuarioId { get; set; }
        public string usuarioNombre { get; set; }
        public List<DetalleDto> detalles { get; set; }

        public class DetalleDto
        {
            public string productoId { get; set; }
            public string productoNombre { get; set; }
            public int cantidad { get; set; }
            public decimal precioUnitario { get; set; }
            public decimal subtotal { get; set; }
        }
    }
}