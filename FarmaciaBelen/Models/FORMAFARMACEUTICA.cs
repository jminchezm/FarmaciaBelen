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
    
    public partial class FORMAFARMACEUTICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FORMAFARMACEUTICA()
        {
            this.PRODUCTO = new HashSet<PRODUCTO>();
        }
    
        public string FORMAFARMACEUTICA_ID { get; set; }
        public string FARMAFARMACEUTICA_NOMBRE { get; set; }
        public string FORMAFARMACEUTICA_DESCRIPCION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCTO> PRODUCTO { get; set; }
    }
}
