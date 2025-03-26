namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class AREA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AREA()
        {
            this.PUESTO = new HashSet<PUESTO>();
        }

        [Required]
        [Display(Name = "C�digo")]
        public string AREA_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string AREA_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripci�n")]
        [StringLength(250)]
        public string AREA_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "N�mero de Extensi�n")]
        [MaxLength(8, ErrorMessage = "La extensi�n no debe exceder los 8 d�gitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten n�meros.")]
        public string AREA_EXTENSION { get; set; }
        [Display(Name = "Correo Electr�nico")]
        [EmailAddress(ErrorMessage = "El formato del correo electr�nico no es v�lido.")]
        public string AREA_CORREO { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string AREA_ESTADO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PUESTO> PUESTO { get; set; }
    }
}
