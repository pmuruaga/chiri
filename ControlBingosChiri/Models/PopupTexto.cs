using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlBingosChiri.Models
{
    public class PopupTexto
    {
        public int Id { get; set; }

        public bool ShowPopup { get; set; }

        public String Titulo { get; set; }

        [AllowHtml]
        [UIHint("tinymce_jquery_full")]
        public String Contenido { get; set; }
    }
}