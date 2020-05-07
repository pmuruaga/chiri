using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ControlBingosChiri.Models
{
    public class ControlBingosChiriDb : DbContext
    {
        public ControlBingosChiriDb()
            : base("DefaultConnectionChiri")
        {
            Database.SetInitializer<ControlBingosChiriDb>(null);
        }

        public DbSet<NumerosSorteo> NumerosSorteo { get; set; }

        public DbSet<PopupTexto> PopupTexto { get; set; }
    }
}