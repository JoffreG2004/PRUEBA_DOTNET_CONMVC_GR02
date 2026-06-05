using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class TipoPlato
    {
        [Key]
        public int IdTipoPlato { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<Plato> Platos { get; set; }

        public TipoPlato()
        {
            Activo = true;
            Platos = new List<Plato>();
        }
    }
}
