using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public int IdCliente { get; set; }

        public DateTime FechaPedido { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal")]
        public decimal IvaPorcentaje { get; set; }

        [Column(TypeName = "decimal")]
        public decimal IvaValor { get; set; }

        [Column(TypeName = "decimal")]
        public decimal ServicioPorcentaje { get; set; }

        [Column(TypeName = "decimal")]
        public decimal ServicioValor { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<PedidoDetalle> Detalles { get; set; }
        public virtual ICollection<MovimientoStock> MovimientosStock { get; set; }

        [NotMapped]
        public string Nombres { get; set; }

        [NotMapped]
        public string Telefono { get; set; }

        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        public List<Plato> Platos { get; set; }

        public Pedido()
        {
            FechaPedido = DateTime.Now;
            Estado = "Pendiente";
            IvaPorcentaje = 12m;
            ServicioPorcentaje = 10m;
            Detalles = new List<PedidoDetalle>();
            MovimientosStock = new List<MovimientoStock>();
            Platos = new List<Plato>();
        }
    }
}
