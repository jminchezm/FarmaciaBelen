namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class INVENTARIO_PRODUCTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INVENTARIO_PRODUCTO()
        {
            this.DETALLEVENTA = new HashSet<DETALLEVENTA>();
        }
    
        public int INVENTARIO_ID { get; set; }
        public string PRODUCTO_ID { get; set; }
        public int STOCK_ACTUAL { get; set; }
        public decimal PRECIO_UNITARIO { get; set; }
    
        public virtual PRODUCTO PRODUCTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLEVENTA> DETALLEVENTA { get; set; }
    }
}
