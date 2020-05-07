using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlBingosChiri.Models
{
    public class VerificarViewModel
    {
        public bool Existe { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El valor a verificar debe ser un numero")]
        public int NumeroVerificado { get; set; }
    }
}