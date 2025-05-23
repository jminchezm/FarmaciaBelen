namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class INVENTARIO_PRODUCTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INVENTARIO_PRODUCTO()
        {
            this.DETALLEVENTA = new HashSet<DETALLEVENTA>();
        }
        [Display(Name = "No.")]

        public int INVENTARIO_ID { get; set; }
        [Display(Name = "Codigo de producto")]
        public string PRODUCTO_ID { get; set; }

        [Display(Name = "Stok Actual")]

        public int STOCK_ACTUAL { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal PRECIO_UNITARIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLEVENTA> DETALLEVENTA { get; set; }
        public virtual PRODUCTO PRODUCTO { get; set; }
    }
}
