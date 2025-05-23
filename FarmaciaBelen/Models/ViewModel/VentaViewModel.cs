using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmaciaBelen.Models.ViewModel
{
    public class VentaViewModel
    {
        public string VENTA_ID { get; set; }

        [Display(Name = "Fecha de Venta")]
        public DateTime? VENTA_FECHA { get; set; }

        [Display(Name = "Total Venta")]
        public decimal? VENTA_TOTAL { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public string CLIENTE_ID { get; set; }
        public string CLIENTE_NOMBRE { get; set; }

        [Display(Name = "Usuario Responsable")]
        public string USUARIO_ID { get; set; }
        public string USUARIO_NOMBRE { get; set; }

        public List<DetalleVentaViewModel> Detalles { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public int DETALLEVENTA_ID { get; set; }

        [Required]
        public string PRODUCTO_ID { get; set; }
        public string PRODUCTO_NOMBRE { get; set; }
        public string PRODUCTO_IMG { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int DETALLEVENTA_CANTIDADVENDIDA { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal DETALLEVENTA_PRECIOUNITARIO { get; set; }

        public decimal DETALLEVENTA_SUBTOTAL => DETALLEVENTA_CANTIDADVENDIDA * DETALLEVENTA_PRECIOUNITARIO;

        public int INVENTARIO_ID { get; set; } // Útil para seleccionar lote o inventario específico si aplica
    }
}
