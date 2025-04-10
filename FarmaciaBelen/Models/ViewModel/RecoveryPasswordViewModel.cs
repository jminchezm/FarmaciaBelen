using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.ViewModel
{
    public class RecoveryPasswordViewModel
    {

        //[StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string token { get; set; }


        //[Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string Password2 { get; set; }

    }
}