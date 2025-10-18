namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VENTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VENTA()
        {
            this.RECIBO = new HashSet<RECIBO>();
            this.DETALLEVENTA = new HashSet<DETALLEVENTA>();
        }
    
        public string VENTA_ID { get; set; }
        public Nullable<System.DateTime> VENTA_FECHA { get; set; }
        public Nullable<decimal> VENTA_TOTAL { get; set; }
        public string CLIENTE_ID { get; set; }
        public string USUARIO_ID { get; set; }
    
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECIBO> RECIBO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLEVENTA> DETALLEVENTA { get; set; }
    }
}
