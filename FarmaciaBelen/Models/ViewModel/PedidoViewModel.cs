using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.ViewModel
{

    public class PedidoViewModel
    {
        [Display(Name = "Código")]
        public string PEDIDO_ID { get; set; }

        [Display(Name = "Fecha del Pedido")]
        public DateTime? FECHA_PEDIDO { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string ESTADO_PEDIDO { get; set; }

        [Required]
        [Display(Name = "Fecha estimada de entrega")]
        public DateTime FECHA_ENTREGA_ESTIMADA { get; set; }

        [Display(Name = "Observaciones")]
        public string OBSERVACIONES { get; set; }

        //[Required]
        [Display(Name = "Proveedor")]
        public string PROVEEDOR_ID { get; set; }

        public string PROVEEDOR_NOMBRE { get; set; }


        public List<DetallePedidoViewModel> Detalles { get; set; } = new List<DetallePedidoViewModel>();
    }

    public class DetallePedidoViewModel
    {
        //public string PEDIDO_ID { get; set; }
        public int? DETALLEPEDIDO_ID { get; set; }

        [Required]
        public string PRODUCTO_ID { get; set; }

        public string PRODUCTO_NOMBRE { get; set; }

        public string PRODUCTO_IMG { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int DETALLEPEDIDO_CANTIDAD { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal? PRECIO_UNITARIO { get; set; }

        public decimal? SUBTOTAL => (PRECIO_UNITARIO ?? 0) * DETALLEPEDIDO_CANTIDAD;
    }

    public class ProductoViewModel
    {
        public string PRODUCTO_ID { get; set; }
        public string PRODUCTO_NOMBRE { get; set; }
        public string IMAGEN_URL { get; set; }
    }
}