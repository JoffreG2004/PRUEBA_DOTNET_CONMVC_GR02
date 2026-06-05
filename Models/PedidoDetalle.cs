using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class PedidoDetalle
    {
        [Key]
        public int IdPedidoDetalle { get; set; }

        [Required]
        public int IdPedido { get; set; }

        [Required]
        public int IdPlato { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Subtotal { get; set; }

        public virtual Pedido Pedido { get; set; }
        public virtual Plato Plato { get; set; }
    }
}
