using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [StringLength(20)]
        public string Cedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(120)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }

        public Cliente()
        {
            Activo = true;
            FechaRegistro = DateTime.Now;
            Pedidos = new List<Pedido>();
        }
    }
}
