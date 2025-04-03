namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ROL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROL()
        {
            this.USUARIO = new HashSet<USUARIO>();
            this.PERMISOS = new HashSet<PERMISOS>();
        }
        [Required]
        [Display(Name = "Código")]
        public string ROL_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string ROL_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250)]
        public string ROL_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string ROL_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERMISOS> PERMISOS { get; set; }
    }
}
