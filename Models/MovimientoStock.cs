using System;
using System.ComponentModel.DataAnnotations;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class MovimientoStock
    {
        [Key]
        public int IdMovimientoStock { get; set; }

        [Required]
        public int IdPlato { get; set; }

        public int? IdPedido { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoMovimiento { get; set; }

        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public DateTime FechaMovimiento { get; set; }

        [StringLength(200)]
        public string Observacion { get; set; }

        public virtual Plato Plato { get; set; }
        public virtual Pedido Pedido { get; set; }

        public MovimientoStock()
        {
            FechaMovimiento = DateTime.Now;
        }
    }
}
