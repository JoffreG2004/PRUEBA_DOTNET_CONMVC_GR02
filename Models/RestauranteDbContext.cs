using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models
{
    public class RestauranteDbContext : DbContext
    {
        public RestauranteDbContext() : base("RestauranteDbContext")
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TipoPlato> TiposPlato { get; set; }
        public DbSet<Plato> Platos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }
        public DbSet<ConfiguracionFacturacion> ConfiguracionesFacturacion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<TipoPlato>().ToTable("TipoPlato");
            modelBuilder.Entity<Plato>().ToTable("Plato");
            modelBuilder.Entity<Pedido>().ToTable("Pedido");
            modelBuilder.Entity<PedidoDetalle>().ToTable("PedidoDetalle");
            modelBuilder.Entity<MovimientoStock>().ToTable("MovimientoStock");
            modelBuilder.Entity<ConfiguracionFacturacion>().ToTable("ConfiguracionFacturacion");

            modelBuilder.Entity<Cliente>().HasIndex(c => c.Cedula).IsUnique();
            modelBuilder.Entity<TipoPlato>().HasIndex(t => t.Nombre).IsUnique();
            modelBuilder.Entity<Plato>().HasIndex(p => p.Nombre).IsUnique();

            modelBuilder.Entity<Plato>().Property(p => p.Precio).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.Subtotal).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.IvaPorcentaje).HasPrecision(5, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.IvaValor).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.ServicioPorcentaje).HasPrecision(5, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.ServicioValor).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.Total).HasPrecision(10, 2);
            modelBuilder.Entity<PedidoDetalle>().Property(d => d.PrecioUnitario).HasPrecision(10, 2);
            modelBuilder.Entity<PedidoDetalle>().Property(d => d.Subtotal).HasPrecision(10, 2);
            modelBuilder.Entity<ConfiguracionFacturacion>().Property(c => c.IvaPorcentaje).HasPrecision(5, 2);
            modelBuilder.Entity<ConfiguracionFacturacion>().Property(c => c.ServicioPorcentaje).HasPrecision(5, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
