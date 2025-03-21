namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class USUARIO
    {
        public string USUARIO_ID { get; set; }
        public string USUARIO_NOMBRE { get; set; }
        public byte[] USUARIO_CONTRASENA { get; set; }
        public Nullable<System.DateTime> USUARIO_FECHAREGISTRO { get; set; }
        public string ROL_ID { get; set; }
        public string EMPLEADO_ID { get; set; }
    
        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual ROL ROL { get; set; }
    }
}
