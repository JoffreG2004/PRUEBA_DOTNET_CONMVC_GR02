using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly RestauranteDbContext db = new RestauranteDbContext();

        public ActionResult Index()
        {
            var platos = db.Platos
                .Include(p => p.TipoPlato)
                .Where(p => p.Activo && p.Stock > 0)
                .OrderBy(p => p.TipoPlato.Nombre)
                .ThenBy(p => p.Nombre)
                .ToList();

            return View(platos);
        }

        [HttpPost]
        public ActionResult Salida(string tipoCliente, string nombres, string apellidos, string cedula, string telefono, string email, string direccion, int[] platoIds)
        {
            if (platoIds == null || platoIds.Length == 0)
            {
                TempData["ErrorPedido"] = "Selecciona al menos un plato.";
                return RedirectToAction("Index");
            }

            if (tipoCliente == "datos" && !DatosClienteValidos(nombres, apellidos, cedula, telefono, email))
            {
                return RedirectToAction("Index");
            }

            var configuracion = db.ConfiguracionesFacturacion.FirstOrDefault(c => c.Activo)
                ?? new ConfiguracionFacturacion();

            Cliente cliente = ObtenerCliente(tipoCliente, nombres, apellidos, cedula, telefono, email, direccion);
            var detalles = new List<PedidoDetalle>();

            foreach (int platoId in platoIds.Distinct())
            {
                Plato plato = db.Platos.Find(platoId);
                if (plato == null || !plato.Activo)
                {
                    continue;
                }

                int cantidad = ObtenerCantidad(platoId);
                if (cantidad <= 0)
                {
                    continue;
                }

                if (cantidad > plato.Stock)
                {
                    TempData["ErrorPedido"] = "No hay stock suficiente para " + plato.Nombre + ". Stock disponible: " + plato.Stock;
                    return RedirectToAction("Index");
                }

                detalles.Add(new PedidoDetalle
                {
                    IdPlato = plato.IdPlato,
                    Plato = plato,
                    Cantidad = cantidad,
                    PrecioUnitario = plato.Precio,
                    Subtotal = plato.Precio * cantidad
                });
            }

            if (detalles.Count == 0)
            {
                TempData["ErrorPedido"] = "Selecciona cantidades validas.";
                return RedirectToAction("Index");
            }

            decimal subtotal = detalles.Sum(d => d.Subtotal);
            decimal ivaValor = Math.Round(subtotal * configuracion.IvaPorcentaje / 100m, 2);
            decimal servicioValor = Math.Round(subtotal * configuracion.ServicioPorcentaje / 100m, 2);

            var pedido = new Pedido
            {
                IdCliente = cliente.IdCliente,
                Cliente = cliente,
                Estado = "Pagado",
                Subtotal = subtotal,
                IvaPorcentaje = configuracion.IvaPorcentaje,
                IvaValor = ivaValor,
                ServicioPorcentaje = configuracion.ServicioPorcentaje,
                ServicioValor = servicioValor,
                Total = subtotal + ivaValor + servicioValor,
                Nombres = cliente.Nombres + " " + cliente.Apellidos,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                Detalles = detalles
            };

            db.Pedidos.Add(pedido);

            foreach (var detalle in detalles)
            {
                int stockAnterior = detalle.Plato.Stock;
                detalle.Plato.Stock -= detalle.Cantidad;
                db.MovimientosStock.Add(new MovimientoStock
                {
                    IdPlato = detalle.IdPlato,
                    Pedido = pedido,
                    TipoMovimiento = "Salida",
                    Cantidad = detalle.Cantidad,
                    StockAnterior = stockAnterior,
                    StockNuevo = detalle.Plato.Stock,
                    Observacion = "Venta"
                });
            }

            db.SaveChanges();
            return RedirectToAction("Factura", new { id = pedido.IdPedido });
        }

        public ActionResult Factura(int id)
        {
            var pedido = db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles.Select(d => d.Plato))
                .FirstOrDefault(p => p.IdPedido == id);

            if (pedido == null)
            {
                return HttpNotFound();
            }

            pedido.Nombres = pedido.Cliente != null ? pedido.Cliente.Nombres + " " + pedido.Cliente.Apellidos : "";
            pedido.Telefono = pedido.Cliente != null ? pedido.Cliente.Telefono : "";
            pedido.Email = pedido.Cliente != null ? pedido.Cliente.Email : "";

            return View("salida", pedido);
        }

        private Cliente ObtenerCliente(string tipoCliente, string nombres, string apellidos, string cedula, string telefono, string email, string direccion)
        {
            if (tipoCliente != "datos")
            {
                const string cedulaFinal = "9999999999";
                var consumidorFinal = db.Clientes.FirstOrDefault(c => c.Cedula == cedulaFinal);
                if (consumidorFinal != null)
                {
                    return consumidorFinal;
                }

                consumidorFinal = new Cliente
                {
                    Cedula = cedulaFinal,
                    Nombres = "Consumidor",
                    Apellidos = "Final",
                    Telefono = "N/A",
                    Email = "consumidorfinal@restaurante.local",
                    Direccion = "N/A"
                };
                db.Clientes.Add(consumidorFinal);
                db.SaveChanges();
                return consumidorFinal;
            }

            string cedulaCliente = string.IsNullOrWhiteSpace(cedula) ? null : cedula.Trim();
            string telefonoCliente = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
            string emailCliente = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();
            Cliente cliente = null;

            if (!string.IsNullOrWhiteSpace(cedulaCliente))
            {
                cliente = db.Clientes.FirstOrDefault(c => c.Cedula == cedulaCliente);
            }

            if (cliente == null)
            {
                cliente = new Cliente();
                db.Clientes.Add(cliente);
            }

            cliente.Cedula = cedulaCliente;
            cliente.Nombres = string.IsNullOrWhiteSpace(nombres) ? "Cliente" : nombres.Trim();
            cliente.Apellidos = string.IsNullOrWhiteSpace(apellidos) ? "Sin apellido" : apellidos.Trim();
            cliente.Telefono = telefonoCliente;
            cliente.Email = emailCliente;
            cliente.Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion.Trim();
            cliente.Activo = true;
            db.SaveChanges();

            return cliente;
        }

        private int ObtenerCantidad(int platoId)
        {
            string valor = Request.Form["cantidad_" + platoId];
            int cantidad;
            return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out cantidad) ? cantidad : 0;
        }

        private bool DatosClienteValidos(string nombres, string apellidos, string cedula, string telefono, string email)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(cedula) || !ValidacionesEcuador.CedulaEcuador(cedula))
            {
                errores.Add("Ingresa una cedula ecuatoriana valida.");
            }

            if (string.IsNullOrWhiteSpace(nombres) || !ValidacionesEcuador.SoloLetras(nombres))
            {
                errores.Add("Los nombres solo deben tener letras y espacios.");
            }

            if (string.IsNullOrWhiteSpace(apellidos) || !ValidacionesEcuador.SoloLetras(apellidos))
            {
                errores.Add("Los apellidos solo deben tener letras y espacios.");
            }

            if (string.IsNullOrWhiteSpace(telefono) || !ValidacionesEcuador.CelularEcuador(telefono))
            {
                errores.Add("El celular debe tener 10 digitos y empezar con 09.");
            }

            if (string.IsNullOrWhiteSpace(email) || !ValidacionesEcuador.EmailValido(email))
            {
                errores.Add("El correo debe tener formato usuario@dominio.com.");
            }

            string cedulaCliente = string.IsNullOrWhiteSpace(cedula) ? null : cedula.Trim();
            string telefonoCliente = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
            string emailCliente = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(telefonoCliente) && db.Clientes.Any(c => c.Telefono == telefonoCliente && c.Cedula != cedulaCliente))
            {
                errores.Add("Ya existe un cliente con ese celular.");
            }

            if (!string.IsNullOrWhiteSpace(emailCliente) && db.Clientes.Any(c => c.Email == emailCliente && c.Cedula != cedulaCliente))
            {
                errores.Add("Ya existe un cliente con ese correo.");
            }

            if (errores.Count > 0)
            {
                TempData["ErrorPedido"] = string.Join(" ", errores);
                return false;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
