using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
        public ActionResult Create([Bind(Include = "IdTipoPlato,Nombre,Descripcion,Precio,Stock,Activo")] Plato plato, HttpPostedFileBase imagenArchivo)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            GuardarImagenSiExiste(plato, imagenArchivo);

            if (ModelState.IsValid)
            {
                db.Platos.Add(plato);
                db.SaveChanges();
                TempData["SwalIcon"] = "success";
                TempData["SwalTitle"] = "Plato registrado";
                TempData["SwalText"] = "El plato se agrego correctamente.";
                return RedirectToAction("Index");
            }

            CargarTipos(plato.IdTipoPlato);
            ViewBag.SwalError = ObtenerPrimerError();
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
        public ActionResult Edit([Bind(Include = "IdPlato,IdTipoPlato,Nombre,Descripcion,Precio,Stock,Activo,FechaRegistro")] Plato plato, HttpPostedFileBase imagenArchivo)
        {
            if (!EsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            Plato platoActual = db.Platos.AsNoTracking().FirstOrDefault(p => p.IdPlato == plato.IdPlato);
            if (platoActual == null)
            {
                return HttpNotFound();
            }

            plato.Imagen = platoActual.Imagen;
            GuardarImagenSiExiste(plato, imagenArchivo);

            if (ModelState.IsValid)
            {
                db.Entry(plato).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SwalIcon"] = "success";
                TempData["SwalTitle"] = "Plato actualizado";
                TempData["SwalText"] = "Los datos del plato se guardaron correctamente.";
                return RedirectToAction("Index");
            }

            CargarTipos(plato.IdTipoPlato);
            ViewBag.SwalError = ObtenerPrimerError();
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
                ViewBag.SwalError = "La cantidad debe ser mayor a cero.";
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
            TempData["SwalIcon"] = "success";
            TempData["SwalTitle"] = "Stock actualizado";
            TempData["SwalText"] = "Se aumento el stock del plato.";

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
            TempData["SwalIcon"] = "success";
            TempData["SwalTitle"] = "Plato desactivado";
            TempData["SwalText"] = "El plato ya no aparecera en compras.";
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

        private void GuardarImagenSiExiste(Plato plato, HttpPostedFileBase imagenArchivo)
        {
            if (imagenArchivo == null || imagenArchivo.ContentLength == 0)
            {
                return;
            }

            string extension = Path.GetExtension(imagenArchivo.FileName);
            string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

            if (string.IsNullOrWhiteSpace(extension) || !extensionesPermitidas.Contains(extension.ToLowerInvariant()))
            {
                ModelState.AddModelError("Imagen", "La imagen debe ser JPG, PNG o WEBP.");
                return;
            }

            const int maximoBytes = 2 * 1024 * 1024;
            if (imagenArchivo.ContentLength > maximoBytes)
            {
                ModelState.AddModelError("Imagen", "La imagen no debe superar 2 MB.");
                return;
            }

            string nombreSeguro = (plato.Nombre ?? "plato").Trim().ToLowerInvariant();
            nombreSeguro = string.Join("-", nombreSeguro.Split(Path.GetInvalidFileNameChars()));
            nombreSeguro = new string(nombreSeguro.Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray());
            while (nombreSeguro.Contains("--"))
            {
                nombreSeguro = nombreSeguro.Replace("--", "-");
            }
            nombreSeguro = nombreSeguro.Trim('-');

            if (string.IsNullOrWhiteSpace(nombreSeguro))
            {
                nombreSeguro = "plato";
            }

            string nombreArchivo = nombreSeguro + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension.ToLowerInvariant();
            string ruta = Server.MapPath("~/Images/" + nombreArchivo);
            imagenArchivo.SaveAs(ruta);
            plato.Imagen = nombreArchivo;
        }

        private string ObtenerPrimerError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e)) ?? "Revisa los datos ingresados.";
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
