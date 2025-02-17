using Core.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.ObservableEntity
{
    public class ViewProductoUnidades
    {
        public ProductoDto Producto { get; set; } = new ();
        public double Unidades { get; set; }
        public double Surtidas { get; set; }
    }
}
