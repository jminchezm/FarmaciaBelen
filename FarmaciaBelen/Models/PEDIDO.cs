//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PEDIDO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PEDIDO()
        {
            this.DETALLE_PEDIDO = new HashSet<DETALLE_PEDIDO>();
        }
    
        public string PEDIDO_ID { get; set; }
        public Nullable<System.DateTime> FECHA_PEDIDO { get; set; }
        public string ESTADO_PEDIDO { get; set; }
        public System.DateTime FECHA_ENTREGA_ESTIMADA { get; set; }
        public string OBSERVACIONES { get; set; }
        public string PROVEEDOR_ID { get; set; }
    
        public virtual PROVEEDOR PROVEEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_PEDIDO> DETALLE_PEDIDO { get; set; }
    }
}
