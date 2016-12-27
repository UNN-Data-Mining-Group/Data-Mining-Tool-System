using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace dms.tools
{
    public interface IDocumentContent
    {
        LayoutDocument ParentDocument { get; set; }
    }
}
