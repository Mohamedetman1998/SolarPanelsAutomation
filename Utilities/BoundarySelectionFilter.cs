using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitTemplate.Utilities
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;
    using PanelsDistribute.Application;
    using System.Linq;

    public class BoundarySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is CurveElement && IsDetailCurve(elem as CurveElement);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            Element elem = EntryPoint.CommandDoc.GetElement(reference);
            return elem != null && AllowElement(elem);
        }

        private bool IsDetailCurve(CurveElement curveElement)
        {
            // Add your criteria to identify detail curves.
            // For example, checking if the curve element is a DetailCurve.
            return curveElement is DetailCurve;
        }
    }



}
