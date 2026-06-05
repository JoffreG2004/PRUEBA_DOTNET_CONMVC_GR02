using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class Plato
    {
        [Key]
        public int IdPlato { get; set; }

        [Required]
        public int IdTipoPlato { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [StringLength(200)]
        public string Imagen { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual TipoPlato TipoPlato { get; set; }
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; }

        public Plato()
        {
            Activo = true;
            FechaRegistro = DateTime.Now;
            PedidoDetalles = new List<PedidoDetalle>();
        }
    }
}
