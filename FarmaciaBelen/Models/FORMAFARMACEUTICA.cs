namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class FORMAFARMACEUTICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FORMAFARMACEUTICA()
        {
            this.PRODUCTO = new HashSet<PRODUCTO>();
        }

        [Required]
        [Display(Name = "Código")]
        public string FORMAFARMACEUTICA_ID { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string FARMAFARMACEUTICA_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "La descripción no debe exceder los 250 caracteres.")]
        public string FORMAFARMACEUTICA_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string FORMAFARMACEUTICA_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCTO> PRODUCTO { get; set; }
    }
}