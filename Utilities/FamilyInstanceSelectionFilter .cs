using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using PanelsDistribute.Application;  

public class FamilyInstanceSelectionFilter : ISelectionFilter
{
    public bool AllowElement(Element elem)
    {
        return elem is FamilyInstance;
    }

    public bool AllowReference(Reference reference, XYZ position)
    {
        Element elem = EntryPoint.CommandDoc.GetElement(reference);
        return elem != null && AllowElement(elem);
    }
}
