namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PUESTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PUESTO()
        {
            this.EMPLEADO = new HashSet<EMPLEADO>();
        }

        [Required]
        [Display(Name = "Código")]
        public string PUESTO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string PUESTO_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250)]
        public string PUESTO_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string PUESTO_ESTADO { get; set; }
        [Required]
        public string AREA_ID { get; set; }

        public virtual AREA AREA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLEADO> EMPLEADO { get; set; }
    }
}