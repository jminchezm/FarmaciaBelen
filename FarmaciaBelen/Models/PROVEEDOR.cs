namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PROVEEDOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROVEEDOR()
        {
            this.PEDIDO = new HashSet<PEDIDO>();
        }
        [Required]
        [Display(Name = "Codigo")]
        public string PROVEEDOR_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string PROVEEDOR_NOMBRE { get; set; }
        //[Required]
        [Display(Name = "Teléfono")]
        [MaxLength(8, ErrorMessage = "El número de teléfono no debe exceder los 8 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string PROVEEDOR_TELEFONO { get; set; }
        //[Required]
        [Display(Name = "Celular")]
        [MaxLength(8, ErrorMessage = "El número del movil no debe exceder los 8 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]

        public string PROVEEDOR_CELULAR { get; set; }
        //[Required]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Por favor, ingresa un correo electrónico válido.")]
        public string PROVEEDOR_CORREO { get; set; }
        //[Required]
        [Display(Name = "Dirrección")]
        public string PROVEEDOR_DIRECCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string PROVEEDOR_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO> PEDIDO { get; set; }
    }
}