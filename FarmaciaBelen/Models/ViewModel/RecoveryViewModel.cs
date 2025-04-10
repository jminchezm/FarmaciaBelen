using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models.ViewModel
{
    public class RecoveryViewModel
    {

        public string token { get; set; }



        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [Display(Name = "Ingrese su correo electrónico")]
        public string EMAIL { get; set; }
    }
}