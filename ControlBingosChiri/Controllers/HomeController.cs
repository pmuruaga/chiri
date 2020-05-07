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

        public ActionResult Verificar(string busqueda)
        {
            int numerov = 0;
            try
            {
                if (!String.IsNullOrEmpty(busqueda))
                {
                    numerov = Convert.ToInt32(busqueda);
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
            //if (numeroJuega)
            //{
            return View(vm);
            //}
            //else
            //{
            //    return View();
            //}
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
