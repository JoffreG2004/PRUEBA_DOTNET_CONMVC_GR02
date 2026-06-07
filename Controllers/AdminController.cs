using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestauranteDbContext db = new RestauranteDbContext();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string usuario, string clave)
        {
            var admin = db.Usuarios.FirstOrDefault(u =>
                u.UsuarioLogin == usuario &&
                u.Clave == clave &&
                u.Activo);

            if (admin == null)
            {
                ViewBag.SwalError = "Usuario o clave incorrectos.";
                return View();
            }

            Session["AdminId"] = admin.IdUsuario;
            Session["AdminNombre"] = admin.Nombre;
            TempData["SwalIcon"] = "success";
            TempData["SwalTitle"] = "Ingreso correcto";
            TempData["SwalText"] = "Bienvenido al panel administrador.";
            return RedirectToAction("Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Dashboard()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login");
            }

            ViewBag.TotalClientes = db.Clientes.Count();
            ViewBag.TotalPlatos = db.Platos.Count(p => p.Activo);
            ViewBag.TotalPedidos = db.Pedidos.Count();
            ViewBag.TotalVentas = db.Pedidos.Where(p => p.Estado != "Anulado").Select(p => p.Total).DefaultIfEmpty(0).Sum();
            return View();
        }

        public ActionResult Facturas()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login");
            }

            var pedidos = db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles.Select(d => d.Plato))
                .OrderByDescending(p => p.FechaPedido)
                .ToList();

            return View(pedidos);
        }

        public ActionResult Factura(int id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login");
            }

            var pedido = db.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles.Select(d => d.Plato))
                .FirstOrDefault(p => p.IdPedido == id);

            if (pedido == null)
            {
                return HttpNotFound();
            }

            ViewBag.ModoAdmin = true;
            return View("~/Views/Restaurante/salida.cshtml", pedido);
        }

        private bool EsAdmin()
        {
            return Session["AdminId"] != null;
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
