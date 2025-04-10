using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    public class UsuarioViewModel
    {
        // Código del usuario generado automáticamente
        public string USUARIO_ID { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de Usuario")]
        public string USUARIO_NOMBRE { get; set; }

        //[Required(ErrorMessage = "Debe seleccionar un empleado.")]
        //[Required(ErrorMessage = "Debe seleccionar un empleado.")]
        //[Required(ErrorMessage = "El empleado es obligatorio.")]
        [Display(Name = "Código Empleado")]
        public string EMPLEADO_ID { get; set; }

        [Display(Name = "Nombre Empleado")]
        public string EMPLEADO_NOMBRE { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public string ROL_ID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        [Display(Name = "Estado")]
        public string USUARIO_ESTADO { get; set; }

        // Contraseñas (para crear o editar)
        //[Required(ErrorMessage = "Debe ingresar una contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        //[Required(ErrorMessage = "Debe de ingresar y confirmar la contraseña.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$", ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres, al menos una mayúscula, una minúscula y un número.")]
        public string NuevaContrasena { get; set; }

        //[Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; }

        // Para control interno si deseas usarlo en edición
        public DateTime USUARIO_FECHAREGISTRO { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Contraseña")]
        ////[Required(ErrorMessage = "Debe de ingresar y confirmar la contraseña.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$", ErrorMessage = "La contraseña debe tener entre 8 y 15 caracteres, al menos una mayúscula, una minúscula y un número.")]
        //public string NuevaContrasenaEditar { get; set; }

        //public virtual ICollection<EMPLEADO> EMPLEADO { get; set; }
    }
}


//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace FarmaciaBelen.Models
//{
//    public class UsuarioViewModel
//    {
//        public string USUARIO_ID { get; set; }

//        [Required]
//        [Display(Name = "Nombre de Usuario")]
//        public string USUARIO_NOMBRE { get; set; }

//        public string EMPLEADO_ID { get; set; }

//        [Required]
//        public string ROL_ID { get; set; }

//        // Contraseñas para edición
//        [DataType(DataType.Password)]
//        public string NuevaContrasena { get; set; }

//        [DataType(DataType.Password)]
//        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
//        public string ConfirmarContrasena { get; set; }
//    }
//}