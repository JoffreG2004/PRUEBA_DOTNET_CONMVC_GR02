using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Controllers
{
    public class ClientesController : Controller
    {
        private readonly RestauranteDbContext db = new RestauranteDbContext();

        public ActionResult Index()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            var clientes = db.Clientes
                .OrderBy(c => c.Apellidos)
                .ThenBy(c => c.Nombres)
                .ToList();

            return View(clientes);
        }

        public ActionResult Details(int? id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            return View(cliente);
        }

        public ActionResult Create()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cedula,Nombres,Apellidos,Telefono,Email,Direccion,Activo")] Cliente cliente)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            ValidarCliente(cliente);

            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                TempData["SwalIcon"] = "success";
                TempData["SwalTitle"] = "Cliente registrado";
                TempData["SwalText"] = "El cliente se guardo correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.SwalError = ObtenerPrimerError();
            return View(cliente);
        }

        public ActionResult Edit(int? id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCliente,Cedula,Nombres,Apellidos,Telefono,Email,Direccion,Activo,FechaRegistro")] Cliente cliente)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            ValidarCliente(cliente);

            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SwalIcon"] = "success";
                TempData["SwalTitle"] = "Cliente actualizado";
                TempData["SwalText"] = "Los datos del cliente se guardaron correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.SwalError = ObtenerPrimerError();
            return View(cliente);
        }

        public ActionResult Delete(int? id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            Cliente cliente = db.Clientes.Find(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SwalIcon"] = "success";
                TempData["SwalTitle"] = "Cliente eliminado";
                TempData["SwalText"] = "El cliente quedo marcado como inactivo.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool EsAdmin()
        {
            return Session["AdminId"] != null;
        }

        private string ObtenerPrimerError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e)) ?? "Revisa los datos ingresados.";
        }

        private void ValidarCliente(Cliente cliente)
        {
            cliente.Cedula = string.IsNullOrWhiteSpace(cliente.Cedula) ? null : cliente.Cedula.Trim();
            cliente.Nombres = string.IsNullOrWhiteSpace(cliente.Nombres) ? cliente.Nombres : cliente.Nombres.Trim();
            cliente.Apellidos = string.IsNullOrWhiteSpace(cliente.Apellidos) ? cliente.Apellidos : cliente.Apellidos.Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(cliente.Telefono) ? null : cliente.Telefono.Trim();
            cliente.Email = string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email.Trim().ToLower();
            cliente.Direccion = string.IsNullOrWhiteSpace(cliente.Direccion) ? null : cliente.Direccion.Trim();

            if (!ValidacionesEcuador.CedulaEcuador(cliente.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cedula ecuatoriana no es valida.");
            }

            if (!ValidacionesEcuador.SoloLetras(cliente.Nombres))
            {
                ModelState.AddModelError("Nombres", "Los nombres solo deben tener letras y espacios.");
            }

            if (!ValidacionesEcuador.SoloLetras(cliente.Apellidos))
            {
                ModelState.AddModelError("Apellidos", "Los apellidos solo deben tener letras y espacios.");
            }

            if (!ValidacionesEcuador.CelularEcuador(cliente.Telefono))
            {
                ModelState.AddModelError("Telefono", "El celular debe tener 10 digitos y empezar con 09.");
            }

            if (!ValidacionesEcuador.EmailValido(cliente.Email))
            {
                ModelState.AddModelError("Email", "El correo debe tener formato usuario@dominio.com.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Cedula) && db.Clientes.Any(c => c.Cedula == cliente.Cedula && c.IdCliente != cliente.IdCliente))
            {
                ModelState.AddModelError("Cedula", "Ya existe un cliente con esta cedula.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Telefono) && db.Clientes.Any(c => c.Telefono == cliente.Telefono && c.IdCliente != cliente.IdCliente))
            {
                ModelState.AddModelError("Telefono", "Ya existe un cliente con este celular.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Email) && db.Clientes.Any(c => c.Email == cliente.Email && c.IdCliente != cliente.IdCliente))
            {
                ModelState.AddModelError("Email", "Ya existe un cliente con este correo.");
            }
        }
    }
}
