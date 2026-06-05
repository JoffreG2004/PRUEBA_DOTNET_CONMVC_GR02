using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class ConfiguracionFacturacion
    {
        [Key]
        public int IdConfiguracion { get; set; }

        [Column(TypeName = "decimal")]
        public decimal IvaPorcentaje { get; set; }

        [Column(TypeName = "decimal")]
        public decimal ServicioPorcentaje { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public ConfiguracionFacturacion()
        {
            IvaPorcentaje = 12m;
            ServicioPorcentaje = 10m;
            Activo = true;
            FechaRegistro = DateTime.Now;
        }
    }
}
