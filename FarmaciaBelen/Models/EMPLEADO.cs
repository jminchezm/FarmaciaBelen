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
    
    public partial class EMPLEADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLEADO()
        {
            this.USUARIO = new HashSet<USUARIO>();
        }
    
        public string EMPLEADO_ID { get; set; }
        public System.DateTime EMPLEADO_FECHANACIMIENTO { get; set; }
        public System.DateTime EMPLEADO_FECHAINGRESO { get; set; }
        public string EMPLEADO_GENERO { get; set; }
        public string EMPLEADO_ESTADO { get; set; }
        public string PUESTO_ID { get; set; }
    
        public virtual PERSONA PERSONA { get; set; }
        public virtual PUESTO PUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
    }
}
