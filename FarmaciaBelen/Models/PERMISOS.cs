namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERMISOS
    {
        public string PERMISOS_ID { get; set; }
        public string ROL_ID { get; set; }
        public string MODULO_ID { get; set; }
        public bool PERMISOS_PUEDEACCEDER { get; set; }
    
        public virtual MODULO MODULO { get; set; }
        public virtual ROL ROL { get; set; }
    }
}
