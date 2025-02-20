using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizador.Models
{
    class SincronizadorSettings
    {
        public string? FiscalConnectionString { get; set; }
        public string? NoFiscalConnectionString { get; set; }
        public string? SerieDefault { get; set; }
        public string? ServerUri { get; set; }
    }
}
