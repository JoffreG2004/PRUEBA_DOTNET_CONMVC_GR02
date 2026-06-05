using System;
using System.ComponentModel.DataAnnotations;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioLogin { get; set; }

        [Required]
        [StringLength(100)]
        public string Clave { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(30)]
        public string Rol { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Usuario()
        {
            Rol = "Administrador";
            Activo = true;
            FechaRegistro = DateTime.Now;
        }
    }
}
