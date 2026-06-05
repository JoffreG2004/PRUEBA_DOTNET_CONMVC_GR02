using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Controllers
{
    public class PlatosController : Controller
    {
        private readonly RestauranteDbContext db = new RestauranteDbContext();

        public ActionResult Index()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            var platos = db.Platos.Include(p => p.TipoPlato).OrderBy(p => p.TipoPlato.Nombre).ThenBy(p => p.Nombre).ToList();
            return View(platos);
        }

        public ActionResult Create()
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            CargarTipos();
            return View(new Plato());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTipoPlato,Nombre,Descripcion,Precio,Stock,Imagen,Activo")] Plato plato)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                db.Platos.Add(plato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarTipos(plato.IdTipoPlato);
            return View(plato);
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

            Plato plato = db.Platos.Find(id);
            if (plato == null)
            {
                return HttpNotFound();
            }

            CargarTipos(plato.IdTipoPlato);
            return View(plato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPlato,IdTipoPlato,Nombre,Descripcion,Precio,Stock,Imagen,Activo,FechaRegistro")] Plato plato)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                db.Entry(plato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarTipos(plato.IdTipoPlato);
            return View(plato);
        }

        public ActionResult AumentarStock(int? id)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Plato plato = db.Platos.Include(p => p.TipoPlato).FirstOrDefault(p => p.IdPlato == id);
            if (plato == null)
            {
                return HttpNotFound();
            }

            return View(plato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AumentarStock(int id, int cantidad)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            Plato plato = db.Platos.Find(id);
            if (plato == null)
            {
                return HttpNotFound();
            }

            if (cantidad <= 0)
            {
                ModelState.AddModelError("", "La cantidad debe ser mayor a cero.");
                return View(plato);
            }

            int stockAnterior = plato.Stock;
            plato.Stock += cantidad;
            db.MovimientosStock.Add(new MovimientoStock
            {
                IdPlato = plato.IdPlato,
                TipoMovimiento = "Entrada",
                Cantidad = cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = plato.Stock,
                Observacion = "Ingreso desde administrador"
            });
            db.SaveChanges();

            return RedirectToAction("Index");
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

            Plato plato = db.Platos.Find(id);
            if (plato == null)
            {
                return HttpNotFound();
            }

            plato.Activo = false;
            db.Entry(plato).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void CargarTipos(int? seleccionado = null)
        {
            ViewBag.IdTipoPlato = new SelectList(db.TiposPlato.Where(t => t.Activo).OrderBy(t => t.Nombre), "IdTipoPlato", "Nombre", seleccionado);
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
