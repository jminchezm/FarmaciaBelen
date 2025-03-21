namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AREA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AREA()
        {
            this.PUESTO = new HashSet<PUESTO>();
        }
    
        public string AREA_ID { get; set; }
        public string AREA_NOMBRE { get; set; }
        public string AREA_DESCRIPCION { get; set; }
        public string AREA_EXTENSION { get; set; }
        public string AREA_CORREO { get; set; }
        public string AREA_ESTADO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PUESTO> PUESTO { get; set; }
    }
}
