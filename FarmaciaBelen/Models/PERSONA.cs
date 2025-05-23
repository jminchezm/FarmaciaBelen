namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PERSONA
    {
        //[Key]
        //[StringLength(10)]
        public string PERSONA_ID { get; set; }
        [Required]
        [Display(Name = "Primer Nombre")]
        public string PERSONA_PRIMERNOMBRE { get; set; }
        [Display(Name = "Segundo Nombre")]
        public string PERSONA_SEGUNDONOMBRE { get; set; }
        [Display(Name = "Tercer Nombre")]
        public string PERSONA_TERCERNOMBRE { get; set; }
        [Required]
        [Display(Name = "Primer Apellido")]
        public string PERSONA_PRIMERAPELLIDO { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string PERSONA_SEGUNDOAPELLIDO { get; set; }
        [Display(Name = "Apellido de Casada")]
        public string PERSONA_APELLIDOCASADA { get; set; }
        [Display(Name = "NIT")]
        [MaxLength(10, ErrorMessage = "El NIT no debe exceder los 10 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string PERSONA_NIT { get; set; }
        [Display(Name = "CUI")]
        [MaxLength(13, ErrorMessage = "El CUI no debe exceder los 13 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string PERSONA_CUI { get; set; }
        [Required]
        [Display(Name = "Dirección")]
        public string PERSONA_DIRECCION { get; set; }
        [Display(Name = "Teléfono de Casa")]
        [MaxLength(8, ErrorMessage = "El número de teléfono no debe exceder los 8 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string PERSONA_TELEFONOCASA { get; set; }
        [Display(Name = "Teléfono Móvil")]
        [MaxLength(8, ErrorMessage = "El número del movil no debe exceder los 8 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        public string PERSONA_TELEFONOMOVIL { get; set; }
        [Display(Name = "Correo Electrónico")]
        public string PERSONA_CORREO { get; set; }
        [Display(Name = "Fecha de Registro")]
        public Nullable<System.DateTime> PERSONA_FECHAREGISTRO { get; set; }

        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
    }
}
