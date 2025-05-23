using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.ViewModel
{
    public class ProductoSelectViewModel
    {
        public string PRODUCTO_ID { get; set; }
        public string PRODUCTO_NOMBRE { get; set; }
        public string PRODUCTO_IMG { get; set; }
        public decimal PRECIO_UNITARIO { get; set; }
    }
}