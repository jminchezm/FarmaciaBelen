namespace FarmaciaBelen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PRODUCTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCTO()
        {
            this.INVENTARIO_PRODUCTO = new HashSet<INVENTARIO_PRODUCTO>();
            this.DETALLE_PEDIDO = new HashSet<DETALLE_PEDIDO>();
        }

        [Required]
        [Display(Name = "Código")]
        public string PRODUCTO_ID { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string PRODUCTO_NOMBRE { get; set; }
        //[Required]
        [Display(Name = "Descripción")]
        public string PRODUCTO_DESCRIPCION { get; set; }

        [Display(Name = "Fecha Registro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Nullable<System.DateTime> PRODUCTO_FECHACREACION { get; set; }
        [Required]
        [Display(Name = "Forma Farmaceutica")]
        public string FORMAFARMACEUTICA_ID { get; set; }
        [Required]
        [Display(Name = "Vía de Administración")]
        public string VIAADMINISTRACION_ID { get; set; }
        //[Required]
        [Display(Name = "Casa Medica")]
        public string PRODUCTO_CASAMEDICA { get; set; }

        [Display(Name = "Referencia")]
        public string PRODUCTO_IMG { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string PRODUCTO_ESTADO { get; set; }
        [Required]
        [Display(Name = "Sub Categoria")]
        public string SUBCATEGORIAPRODUCTO_ID { get; set; }

        public virtual FORMAFARMACEUTICA FORMAFARMACEUTICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INVENTARIO_PRODUCTO> INVENTARIO_PRODUCTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_PEDIDO> DETALLE_PEDIDO { get; set; }
        public virtual SUBCATEGORIAPRODUCTO SUBCATEGORIAPRODUCTO { get; set; }
        public virtual VIAADMINISTRACION VIAADMINISTRACION { get; set; }
    }
}