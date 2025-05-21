namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class MODULO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MODULO()
        {
            this.PERMISOS = new HashSet<PERMISOS>();
        }

        [Required]
        [Display(Name = "Código")]
        public string MODULO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string MODULO_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "La descripción no debe exceder los 250 caracteres.")]
        public string MODULO_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string MODULO_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERMISOS> PERMISOS { get; set; }
    }
}