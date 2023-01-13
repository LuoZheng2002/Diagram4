using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram4
{
    internal interface IDiagramItem
    {
        double CanvasTop { get; set; }
        double CanvasLeft { get; set; }
    }
}
