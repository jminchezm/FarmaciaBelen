namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PUESTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PUESTO()
        {
            this.EMPLEADO = new HashSet<EMPLEADO>();
        }
    
        public string PUESTO_ID { get; set; }
        public string PUESTO_NOMBRE { get; set; }
        public string PUESTO_DESCRIPCION { get; set; }
        public string PUESTO_ESTADO { get; set; }
        public string AREA_ID { get; set; }
    
        public virtual AREA AREA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLEADO> EMPLEADO { get; set; }
    }
}
