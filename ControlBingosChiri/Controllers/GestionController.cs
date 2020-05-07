using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ControlBingosChiri.Models;
using log4net;

namespace ControlBingosChiri.Controllers
{
    public class GestionController : Controller
    {
        ControlBingosChiriDb _db = new ControlBingosChiriDb();
        ILog log = log4net.LogManager.GetLogger(typeof(GestionController));
        //
        // GET: /Gestion/
        [Authorize(Users="manager")]
        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        [Authorize(Users = "manager")]
        public ActionResult Upload(string fecha_sorteo, HttpPostedFileBase file)
        {
            DateTime fechaSorteo = DateTime.Parse(fecha_sorteo);
            log.Debug("Iniciando Upload...");

            var a = _db.NumerosSorteo.ToList();
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                //var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                var path = Path.Combine( Server.MapPath("~/App_Data/uploads")
                , string.Concat( Path.GetFileNameWithoutExtension(fileName)
                               , DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss")
                               , Path.GetExtension(fileName)
                               ));
                log.Debug("Subiendo Archivo...");
                try
                {
                    log.Debug("Grabando...");
                    file.SaveAs(path);
                }
                catch (Exception e) {
                    log.Debug("Error al Grabar...");
                    ViewBag.Message = "Error";
                    return View();
                }
                ViewBag.Message = "Ok";
                log.Debug("Grabacion correcta / Importando...");
                //ImportDBF(path.ToString());                
                ImportCSV(path.ToString(), fechaSorteo);    
                //Agregar Parametro de Fecha y guardar los numeros con la fecha...
            }            
            return View();
        }

        public void ImportCSV(string strFilePath, DateTime fechaSorteo)
        {
            try
            {
                log.Debug("Subiendo Archivo CSV");
                DataTable dt = new DataTable();
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    string[] headers = sr.ReadLine().Split(';');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(';');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }

                }

                //Dejar solo los ultimos 3 sorteos
                //using (var ctx = new ControlBingosChiriDb()) // assuming IDisposable
                //{
                //    log.Debug("Limpiando Datos viejos");
                //    ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE [NumerosSorteos]");
                //    ctx.SaveChanges();
                //}
                using (var ctx = new ControlBingosChiriDb()) // assuming IDisposable
                {
                    log.Debug("Limpiando Datos viejos");
                    ctx.Database.ExecuteSqlCommand("Delete From [NumerosSorteos] Where DATEADD(month, 2, FechaCreacion) < getdate()");
                    ctx.SaveChanges();
                }
                //  Where DATEADD(month, 2, FechaCreacion) < getdate()

                using (var ctx2 = new ControlBingosChiriDb()) // assuming IDisposable
                {
                    log.Debug("Preparando Registros a Grabar");
                    foreach (DataRow item in dt.Rows)
                    {
                        NumerosSorteo n = new NumerosSorteo();
                        n.NumVal = Convert.ToInt32(item["numven"]);
                        n.FechaSorteo = fechaSorteo;
                        n.FechaCreacion = DateTime.Now;
                        //Console.Write(item["numven"]);
                        ctx2.NumerosSorteo.Add(n);
                    }
                    log.Debug("Grabando");
                    ctx2.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Debug("Error en la importación: ");
                log.Debug("Exception: " + ex.GetType() + ex.InnerException);
                log.Debug("Message: " + ex.Message + "$$" + ex);
                if (ex.GetType() != typeof(InvalidOperationException))
                {
                    ViewBag.Message = "Error";
                }
                return;
            }

            //return dt;
        }

        public void ImportDBF(string filename)
        {
            log.Debug("Importando archivo...");
            try
            {

                string FileName = filename;
                string Path = FileName.Substring(0);
                int Separator = Path.LastIndexOf("\\");
                string DataSource = FileName.Substring(0, Separator) + "\\";

                string File = FileName.Substring(Separator);

                int Separator2 = File.LastIndexOf("\\");
                string DBF = File.Remove(Separator2, 1);

                int Separator3 = DBF.LastIndexOf(".");
                string DBF_Extension = DBF.Substring(Separator3);
                string DBF_FileName = DBF.Remove(Separator3, 4);

                log.Debug("Iniciando Conexion Con DBF...");
                //define the connections to the .dbf file
                OleDbConnection conn = new OleDbConnection(@"Provider=vfpoledb;Data Source=" + DataSource + ";Collating Sequence=machine;");
                log.Debug("Preparando Comando");
                OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
                //open the connection and read in all the data from .dbf file into a datatable
                conn.Open();
                DataTable dt = new DataTable();
                log.Debug("Ejecutando Lectura del DBF");
                dt.Load(command.ExecuteReader());
                log.Debug("Cerrando Conexión");
                conn.Close();  //close connection to the .dbf file

                using(var ctx = new ControlBingosChiriDb()) // assuming IDisposable
                {
                    log.Debug("Limpiando Datos viejos");
                    ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE [NumerosSorteos]");
                    ctx.SaveChanges();
                }

                using (var ctx2 = new ControlBingosChiriDb()) // assuming IDisposable
                {
                    log.Debug("Preparando Registros a Grabar");
                    foreach (DataRow item in dt.Rows)
                    {
                        NumerosSorteo n = new NumerosSorteo();
                        n.NumVal = Convert.ToInt32(item["numven"]);
                        //Console.Write(item["numven"]);
                        ctx2.NumerosSorteo.Add(n);
                    }
                    log.Debug("Grabando");
                    ctx2.SaveChanges();
                }
                
                
                //string SQLTableName = "Control";

                ////create a reader for the datatable
                //DataTableReader reader = dt.CreateDataReader();
                //myConnection.Open();   ///this is my connection to the sql server
                //SqlBulkCopy sqlcpy = new SqlBulkCopy(myConnection);
                //sqlcpy.DestinationTableName = SQLTableName;  //copy the datatable to the sql table

                //sqlcpy.WriteToServer(dt);

                //myConnection.Close();

                //reader.Close();

                //MessageBox.Show("Congratulations, your .dbf file has been transferred.", "Success");

            }
            
            catch (Exception ex)
            {
                log.Debug("Error en la importación: ");
                log.Debug("Exception: " + ex.GetType() + ex.InnerException);
                log.Debug("Message: " + ex.Message +"$$" + ex);
                if(ex.GetType() != typeof(InvalidOperationException)){
                    ViewBag.Message = "Error";
                }                
                return;
                //LogEntry(ex.ToString());
                //MessageBox.Show("Sorry, your .dbf file transfer failed.  " +
                //"Please make sure that you entered a valid SQL table name.  " +
                //"It must contain the correct column names and lengths that correspond to the .dbf.", "Failure");
                //this.Refresh();
            }

        }

        [Authorize(Users = "manager")]
        public ActionResult Popup() {

            PopupTexto popuptexto = _db.PopupTexto.FirstOrDefault();
            if (popuptexto == null)
            {
                return View();
            }
            return View(popuptexto);
            
        }

        [HttpPost]
        [Authorize(Users = "manager")]
        public ActionResult SavePopup(PopupTexto popupTexto) {            
            PopupTexto popupContenido = _db.PopupTexto.FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (popupContenido != null)
                {
                    popupContenido.Titulo = popupTexto.Titulo;
                    popupContenido.Contenido = popupTexto.Contenido;
                    popupContenido.ShowPopup = popupTexto.ShowPopup;
                    _db.Entry(popupContenido).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    _db.PopupTexto.Add(popupTexto);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View();           
        }        

    }
}
