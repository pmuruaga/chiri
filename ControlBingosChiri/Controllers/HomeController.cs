using ControlBingosChiri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlBingosChiri.Controllers
{
    public class HomeController : Controller
    {
        ControlBingosChiriDb _db = new ControlBingosChiriDb();

        public ActionResult Index()
        {
            ViewBag.Message = "Siga las instrucciones para validar su cartón.";

            PopupTexto popupContenido = _db.PopupTexto.FirstOrDefault();

            if(popupContenido!=null)
            {
                ViewBag.MostrarPopup = popupContenido.ShowPopup;

                ViewBag.TituloPopup = popupContenido.Titulo;

                ViewBag.ContenidoPopup = popupContenido.Contenido;
            }
              
            return View();
        }        

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Verificar(string id)
        {
            return VerificarCarton(id);
        }

        [HttpPost]
        public ActionResult Verificar(string busqueda, string id)
        {
            var numeroCarton = !String.IsNullOrEmpty(busqueda) ? busqueda : id;
            return VerificarCarton(numeroCarton);
        }

        private ActionResult VerificarCarton(string numeroCarton)
        {
            int numerov = 0;
            try
            {
                if (!String.IsNullOrEmpty(numeroCarton))
                {
                    numerov = Convert.ToInt32(numeroCarton);
                }
            }
            catch (Exception e) {
                numerov = -500;
            }

            //Max Sorteo es la fecha mayor de los sorteos, es decir el próximo...
            DateTime maxSorteo = _db.NumerosSorteo.Max(s => s.FechaSorteo);
            //Agregar chequeo fecha sorteo
            bool numeroJuega = _db.NumerosSorteo.Any(numero => (numero.NumVal.Equals(numerov)) && numero.FechaSorteo.Equals(maxSorteo));

            VerificarViewModel vm = new VerificarViewModel();
            vm.Existe = numeroJuega;
            vm.NumeroVerificado = numerov;
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetProximoSorteo()
        {
            DateTime maxSorteo = _db.NumerosSorteo.Max(s => s.FechaSorteo);

            var fecha_sorteo = new {
                            DateTime = maxSorteo.ToString("dd/MM/yyyy")
                        };            
            return Json(fecha_sorteo, JsonRequestBehavior.AllowGet);            
        }
    }
}
