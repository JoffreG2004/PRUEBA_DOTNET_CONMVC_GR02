using RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Controllers
{
    public class RestauranteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Salida(string nombres, string telefono, string email,
                           string rbtnEntrada, string rbtnSopa, string rbtnPlatoFuerte, string rbtnPostre)
        {
            List<Plato> listaPlatos = new List<Plato>();

            if (!string.IsNullOrEmpty(rbtnEntrada))
                listaPlatos.Add(ObtenerDetallesPlato(rbtnEntrada));

            if (!string.IsNullOrEmpty(rbtnSopa))
                listaPlatos.Add(ObtenerDetallesPlato(rbtnSopa));

            if (!string.IsNullOrEmpty(rbtnPlatoFuerte))
                listaPlatos.Add(ObtenerDetallesPlato(rbtnPlatoFuerte));

            if (!string.IsNullOrEmpty(rbtnPostre))
                listaPlatos.Add(ObtenerDetallesPlato(rbtnPostre));

            decimal total = listaPlatos.Sum(p => p.Precio);

            Pedido pedido = new Pedido
            {
                Nombres = nombres,
                Telefono = telefono,
                Email = email,
                Platos = listaPlatos,
                Total = total
            };

            return View(pedido);
        }

        private Plato ObtenerDetallesPlato(string nombrePlato)
        {
            var p = new Plato { Nombre = nombrePlato };

            switch (nombrePlato)
            {
                // ENTRADAS
                case "Empanada de Morocho":
                    p.Imagen = "empanada-de-morocho.jpg";
                    p.Descripcion = "La empanada de morocho es un platillo tradicional ecuatoriano, hecho con masa de maíz morocho y relleno de carne, arvejas y otros ingredientes, que se fríe hasta dorarse.";
                    p.Precio = 2.50m;
                    break;

                case "Bolón de Verde":
                    p.Imagen = "bolón-de-verde.jpg";
                    p.Descripcion = "El bolón de verde es un plato típico de Ecuador, especialmente popular en la región costera. Se elabora a base de plátano verde (plátano macho) que se cocina y se machaca, luego se mezcla con queso, chicharrón o chorizo, y se forma una masa que se fríe hasta dorarse.";
                    p.Precio = 3.00m;
                    break;

                case "Ceviche de Camarón":
                    p.Imagen = "ceviche-de-camaron.jpg";
                    p.Descripcion = "El ceviche de camarón es un platillo fresco y cítrico donde los camarones se “cocinan” en jugo de limón o lima y se mezclan con vegetales frescos como tomate, cebolla, cilantro y aguacate.";
                    p.Precio = 6.50m;
                    break;

                // SOPAS
                case "Caldo de Bolas de Verde":
                    p.Imagen = "caldo-bolas-verde.jpg";
                    p.Descripcion = "El caldo de bolas de verde es una sopa tradicional ecuatoriana de la región costera, hecha con bolas de plátano verde rellenas de carne y vegetales, cocidas en un caldo sabroso con yuca y maíz.";
                    p.Precio = 4.50m;
                    break;

                case "Caldo de Gallina":
                    p.Imagen = "caldo-de-gallina.jpg";
                    p.Descripcion = "El caldo de gallina criolla o de pollo ecuatoriano, es un plato muy simple, que se puede servir como consomé, o el caldo con la presa. También, puedes acompañarlo con una papa cocinada, cebollita blanca y cilantro o perejil picado.";
                    p.Precio = 4.00m;
                    break;

                case "Caldo de Patas":
                    p.Imagen = "caldo-de-patas.png";
                    p.Descripcion = "El Caldo de patas es una sopa tradicional ecuatoriana hecha con patas de res, mote, yuca, maní y leche, acompañada de arroz y ají, ideal para días fríos y reuniones familiares.";
                    p.Precio = 4.00m;
                    break;

                // PLATOS FUERTES
                case "Arroz Marinero":
                    p.Imagen = "arroz-marinero.jpg";
                    p.Descripcion = "El arroz marinero es un plato latinoamericano de arroz con mariscos, similar a la paella española, que combina arroz cocido en caldo de mariscos con camarones, calamares, mejillones, almejas y especias.";
                    p.Precio = 8.50m;
                    break;

                case "Churrasco":
                    p.Imagen = "churrasco.jpg";
                    p.Descripcion = "El churrasco es un corte de carne popular en América Latina, preparado a la parrilla o a la plancha, y varía en su preparación según el país.";
                    p.Precio = 6.00m;
                    break;

                case "Apanado":
                    p.Imagen = "apanado.jpg";
                    p.Descripcion = "El apanado de carne es un plato tradicional que consiste en filetes de carne empanizados, crujientes por fuera y jugosos por dentro, muy popular en la gastronomía latinoamericana.";
                    p.Precio = 5.50m;
                    break;

                // POSTRES
                case "Dulce de Higos":
                    p.Imagen = "dulce-de-higos.jpg";
                    p.Descripcion = "El dulce de higos es un postre tradicional ecuatoriano con profundas raíces en la época colonial, siendo un clásico en los hogares y festividades. Está hecho a base de higos tiernos que se cocinan lentamente en un espeso almíbar de panela, aromatizado con canela y clavo de olor, y requiere un largo proceso de preparación para lograr su textura suave. Tradicionalmente se sirve acompañado de una rebanada de queso fresco para contrastar su intenso dulzor.";
                    p.Precio = 2.00m;
                    break;

                case "Espumilla":
                    p.Imagen = "espumilla.jpg";
                    p.Descripcion = "La espumilla es un merengue tradicional ecuatoriano y una popular comida callejera. Posiblemente se remonta a 1907, según registros que mencionan su existencia, está hecha con claras de huevo, azúcar y pulpa de fruta, a menudo de guayaba, y se bate frecuentemente a mano para lograr la textura adecuada.";
                    p.Precio = 1.50m; 
                    break;

                case "Helado":
                    p.Imagen = "helado.jpg";
                    p.Descripcion = "En su forma más simple, el helado o crema helada es un alimento congelado que por lo general se hace de productos lácteos tales como leche o crema.";
                    p.Precio = 2.00m; 
                    break;

                default:
                    p.Imagen = "default.jpg";
                    p.Descripcion = "No seleccionado.";
                    p.Precio = 0m; 
                    break;
            }

            return p;
        }
    }
}
