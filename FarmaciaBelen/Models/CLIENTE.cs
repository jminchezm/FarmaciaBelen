namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CLIENTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CLIENTE()
        {
            this.VENTA = new HashSet<VENTA>();
        }

        [Display(Name = "Codigo de Cliente")]
        public string CLIENTE_ID { get; set; }

        [Display(Name = "Nota del cliente")]
        public string CLIENTE_NOTA { get; set; }

        [Display(Name = "Estado")]
        public string CLIENTE_ESTADO { get; set; }

        public virtual PERSONA PERSONA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VENTA> VENTA { get; set; }
    }
}
