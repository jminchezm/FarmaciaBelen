namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SUBCATEGORIAPRODUCTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUBCATEGORIAPRODUCTO()
        {
            this.PRODUCTO = new HashSet<PRODUCTO>();
        }
        [Required]
        [Display(Name = "Codigo")]
        public string SUBCATEGORIAPRODUCTO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string SUBCATEGORIAPRODUCTO_NOMBRE { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string SUBCATEGORIAPRODUCTO_DESCRIPCION { get; set; }
        [Required]
        [Display(Name = "Categoría")]
        public string CATEGORIAPRODUCTO_ID { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string SUBCATEGORIAPRODUCTO_ESTADO { get; set; }

        public virtual CATEGORIAPRODUCTO CATEGORIAPRODUCTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCTO> PRODUCTO { get; set; }
    }
}
