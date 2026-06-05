using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
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

            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
    }
}
