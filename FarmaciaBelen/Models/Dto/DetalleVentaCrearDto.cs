using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.Dto
{
    public class DetalleVentaCrearDto
    {

        public string productoId { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        // opcional: si manejas lotes específicos
        public int? inventarioId { get; set; }

    }
}