namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class EMPLEADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLEADO()
        {
            this.USUARIO = new HashSet<USUARIO>();
        }

        //[Key]
        //[ForeignKey("PERSONA")] // Usa el mismo ID que PERSONA
        //[Required]
        [Display(Name = "Código")]
        public string EMPLEADO_ID { get; set; }
        //[Required]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EMPLEADO_FECHANACIMIENTO { get; set; }
        //[Required]
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EMPLEADO_FECHAINGRESO { get; set; }
        [Required]
        [Display(Name = "Genero")]
        public string EMPLEADO_GENERO { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string EMPLEADO_ESTADO { get; set; }
        [Required]
        [Display(Name = "Puesto")]
        public string PUESTO_ID { get; set; }

        public virtual PERSONA PERSONA { get; set; }
        public virtual PUESTO PUESTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
    }
}
