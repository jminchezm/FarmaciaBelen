namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class USUARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIO()
        {
            this.VENTA = new HashSet<VENTA>();
        }

        [Required]
        [Display(Name = "Código")]
        public string USUARIO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string USUARIO_NOMBRE { get; set; }
        //[Required(ErrorMessage = "La contraseña es obligatoria.")]
        [Display(Name = "Contraseña")]
        public byte[] USUARIO_CONTRASENA { get; set; }
        //[Required]
        [Display(Name = "Fecha de Registro")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> USUARIO_FECHAREGISTRO { get; set; }
        [Required]
        [Display(Name = "Rol")]
        public string ROL_ID { get; set; }
        [Required]
        [Display(Name = "Empleado")]
        public string EMPLEADO_ID { get; set; }
        public string TOKEN_RECOVERY { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string USUARIO_ESTADO { get; set; }
        public Nullable<System.DateTime> TOKEN_EXPIRACION { get; set; }

        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual ROL ROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VENTA> VENTA { get; set; }
    }
}