
namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ROL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROL()
        {
            this.USUARIO = new HashSet<USUARIO>();
        }
    
        public string ROL_ID { get; set; }
        public string ROL_NOMBRE { get; set; }
        public string ROL_DESCRIPCION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
    }
}
