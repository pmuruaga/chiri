using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlBingosChiri.Models
{    
    public class NumerosSorteo
    {
        public int Id { get; set; }
        public int NumVal { get; set; }
        public DateTime FechaSorteo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}