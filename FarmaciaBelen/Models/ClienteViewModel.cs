using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    public class ClienteViewModel
    {
        // Datos de la Persona
        public PERSONA Persona { get; set; }

        // Datos del Cliente
        public CLIENTE Cliente { get; set; }

        public ClienteViewModel()
        {
            Persona = new PERSONA
            {
                PERSONA_FECHAREGISTRO = DateTime.Now // si tienes este campo en PERSONA
            };

            Cliente = new CLIENTE
            {
                CLIENTE_ESTADO = "Activo",
                CLIENTE_NOTA = string.Empty // valor por defecto
            };
        }

    }
}