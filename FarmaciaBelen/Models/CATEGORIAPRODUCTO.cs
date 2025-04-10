namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CATEGORIAPRODUCTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIAPRODUCTO()
        {
            this.SUBCATEGORIAPRODUCTO = new HashSet<SUBCATEGORIAPRODUCTO>();
        }
        [Required]
        [Display(Name = "Codigo")]
        public string CATEGORIAPRODUCTO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string CATEGORIAPRODUCTO_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string CATEGORIAPRODUCTO_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string CATEGORIAPRODUCTO_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIAPRODUCTO> SUBCATEGORIAPRODUCTO { get; set; }
    }
}
