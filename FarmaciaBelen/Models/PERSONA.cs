namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERSONA
    {
        public string PERSONA_ID { get; set; }
        public string PERSONA_PRIMERNOMBRE { get; set; }
        public string PERSONA_SEGUNDONOMBRE { get; set; }
        public string PERSONA_TERCERNOMBRE { get; set; }
        public string PERSONA_PRIMERAPELLIDO { get; set; }
        public string PERSONA_SEGUNDOAPELLIDO { get; set; }
        public string PERSONA_APELLIDOCASADA { get; set; }
        public string PERSONA_NIT { get; set; }
        public string PERSONA_CUI { get; set; }
        public string PERSONA_DIRECCION { get; set; }
        public string PERSONA_TELEFONOCASA { get; set; }
        public string PERSONA_TELEFONOMOVIL { get; set; }
        public string PERSONA_CORREO { get; set; }
        public Nullable<System.DateTime> PERSONA_FECHAREGISTRO { get; set; }
    
        public virtual EMPLEADO EMPLEADO { get; set; }
    }
}
