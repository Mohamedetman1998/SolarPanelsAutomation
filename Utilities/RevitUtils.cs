using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PanelsDistribute.Application;
using RevitTemplate.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitTemplate.Utilities
{
    public class RevitUtils
    {
        public const double PanelWidth = 65.486;
        public const double PanelHeight = 12.95;
        public static XYZ SelectedPanelLocationPoint { get; set; }
        public static Element PanelElement { get; set; }

        public static List<CurveElement> selectedBoundaryLines = new List<CurveElement>();
        public static List<XYZ> filterPoints(List<XYZ> distinctPoints)
        {
            List<XYZ> filteredPoints = new List<XYZ>();

            foreach (var point in distinctPoints)
            {
                if (IsPointInsidePolygon(point, selectedBoundaryLines))
                {
                    if (!IsIntersectingBoundry(PanelBoundryLines(point)))
                    {

                        filteredPoints.Add(point);
                    }
                }
            }
            return filteredPoints;

        }
        public static void CreateInstancesAtSegments(List<XYZ> givenPoints)
        {
           
            foreach (var point in givenPoints)
                {
                    if (!IsIntersectingBoundry(PanelBoundryLines(point)))
                    {
                        Transaction subtr = new Transaction(EntryPoint.CommandDoc,"Distribute");
                        subtr.Start();
                        CreateInstanceAtPoint(point);
                        subtr.Commit();

                    }
                    else
                    {
                        continue;
                    }
                }

           
        }
        public void ShapeMaker(List<XYZ> points)
        {
            List<GeometryObject> shapes = new List<GeometryObject>();
            points.ForEach(point => shapes.Add(Point.Create(point)));

            Transaction trs = new Transaction(EntryPoint.CommandDoc, "SDSd");
            trs.Start();

            var dS = DirectShape.CreateElement(EntryPoint.CommandDoc, new ElementId(BuiltInCategory.OST_GenericModel));
            dS.AppendShape(shapes);

            trs.Commit();
        }
        public static void GetBoundryLines()
        {
            TaskDialog taskDialog = new TaskDialog("Alert");
            taskDialog.MainContent = "Please select the boundary lines";
            taskDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
            TaskDialogResult result = taskDialog.Show();
            if (result == TaskDialogResult.Ok)
            {
                var detailCurveFilter = new BoundarySelectionFilter();
                selectedBoundaryLines = EntryPoint.CommandUIdoc.Selection.PickObjects(
                     Autodesk.Revit.UI.Selection.ObjectType.Element,
                     detailCurveFilter)
                     .Select(reference => (CurveElement)EntryPoint.CommandDoc.GetElement(reference))
                     .ToList();
            }
            else
            {
                return;
            }
        }
        public static void GetFirstInstance()
        {
            TaskDialog taskDialog = new TaskDialog("Alert");
            taskDialog.MainContent = "Please select the first panel";
            taskDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
            TaskDialogResult result = taskDialog.Show();
            if (result == TaskDialogResult.Ok)
            {
                FamilyInstanceSelectionFilter familyInstanceFilter = new FamilyInstanceSelectionFilter();

                Reference firstSolarPanel = EntryPoint.CommandUIdoc.Selection.PickObject(
                    Autodesk.Revit.UI.Selection.ObjectType.Element,
                    familyInstanceFilter);

                PanelElement = EntryPoint.CommandDoc.GetElement(firstSolarPanel) as FamilyInstance;

                if (PanelElement != null)
                {
                    SelectedPanelLocationPoint = (PanelElement.Location as LocationPoint).Point;
                }
            }
            else
            {
                return;
            }
           
        }
        public static bool IsIntersectingBoundry(List<Curve> instanceLines)
        {
            foreach (CurveElement boundaryLine in selectedBoundaryLines)
            {
                foreach (Curve instanceLine in instanceLines)
                {
                    if (instanceLine.Intersect(boundaryLine.GeometryCurve) == SetComparisonResult.Overlap)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool AreInstanceLinesIntersecting(List<Curve> instanceLines1, List<Curve> instanceLines2)
        {
            foreach (Curve line1 in instanceLines1)
            {
                foreach (Curve line2 in instanceLines2)
                {
                    if (line1.Intersect(line2) == SetComparisonResult.Overlap)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static List<Curve> PanelBoundryLines(XYZ locPoint)
        {
           

            List<Curve> lines = new List<Curve>();
            var instanceLine1 = Line.CreateBound(new XYZ(locPoint.X - (PanelWidth / 2), locPoint.Y, 0), new XYZ(locPoint.X + (PanelWidth / 2), locPoint.Y, 0)) as Curve;

            var instanceLine2 = Line.CreateBound(new XYZ(locPoint.X - (PanelWidth / 2), locPoint.Y + (PanelHeight / 2), 0), new XYZ(locPoint.X + (PanelWidth / 2), locPoint.Y + (PanelHeight / 2), 0)) as Curve;

            var instanceLine3 = Line.CreateBound(new XYZ(locPoint.X - (PanelWidth / 2), locPoint.Y - (PanelHeight / 2), 0), new XYZ(locPoint.X + (PanelWidth / 2), locPoint.Y - (PanelHeight / 2), 0)) as Curve;

            lines.Add(instanceLine1);
            lines.Add(instanceLine2);
            lines.Add(instanceLine3);


            return lines;
        }
        public static void CreateInstanceAtPoint(XYZ location)
        {

            var fi = EntryPoint.CommandDoc.Create.NewFamilyInstance(location, (PanelElement as FamilyInstance).Symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            var axis = Line.CreateBound(XYZ.Zero, XYZ.BasisZ);
            fi.Location.Rotate(axis, Math.PI / 2);
            //fi.Location.Move(location - (fi.Location as LocationPoint).Point);
            (fi.Location as LocationPoint).Point = location;

        }
        public static List<XYZ> RemoveDuplicatePoints(List<XYZ> points, double tolerance = 1e-6)
        {
            List<XYZ> distinctPoints = new List<XYZ>();
            foreach (XYZ point in points)
            {
                if (!IsPointInList(distinctPoints, point, tolerance))
                {
                    distinctPoints.Add(point);
                }
            }
            return distinctPoints;
        }
        public static bool IsPointInList(List<XYZ> pointList, XYZ point, double tolerance)
        {
            foreach (XYZ p in pointList)
            {
                if (p.IsAlmostEqualTo(point, tolerance))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsPointInsidePolygon(XYZ point, List<CurveElement> boundaryCurves)
        {
            int count = 0;

            for (int i = 0; i < boundaryCurves.Count; i++)
            {
                Curve curve = boundaryCurves[i].GeometryCurve;
                IList<XYZ> curvePoints = curve.Tessellate();

                for (int j = 0; j < curvePoints.Count - 1; j++)
                {
                    XYZ vertex1 = curvePoints[j];
                    XYZ vertex2 = curvePoints[j + 1];

                    if (((vertex1.Y <= point.Y && point.Y < vertex2.Y) || (vertex2.Y <= point.Y && point.Y < vertex1.Y)) &&
                        (point.X < (vertex2.X - vertex1.X) * (point.Y - vertex1.Y) / (vertex2.Y - vertex1.Y) + vertex1.X))
                    {
                        count++;
                    }
                }
            }

            return count % 2 == 1;
        }
        public static List<XYZ> GeneratePointsAroundSelectedPointOld(XYZ selectedPoint, double maxXDistance, double maxYDistance)
        {
            List<XYZ> pointsList = new List<XYZ>();

            double distanceXIncrement = MmToFeet(MainWindowViewModel.hzDistance);
            double distanceyIncrement = MmToFeet(MainWindowViewModel.vlDistance);

            for (double distance = distanceyIncrement; distance <= maxXDistance; distance += distanceyIncrement)
            {
                XYZ newPointPositiveY = new XYZ(selectedPoint.X, selectedPoint.Y + distance, selectedPoint.Z);
                pointsList.Add(newPointPositiveY);
            }

            // Generate points in the negative Y direction
            for (double distance = -distanceyIncrement; distance >= -maxXDistance; distance -= distanceyIncrement)
            {
                XYZ newPointNegativeY = new XYZ(selectedPoint.X, selectedPoint.Y + distance, selectedPoint.Z);
                pointsList.Add(newPointNegativeY);
            }


            // Generate points in the positive X direction
            for (double xDistance = distanceXIncrement; xDistance <= maxXDistance; xDistance += distanceXIncrement)
            {
                XYZ newPointPositiveX = new XYZ(selectedPoint.X + xDistance, selectedPoint.Y, selectedPoint.Z);
                pointsList.Add(newPointPositiveX);

                // Generate points in Y and -Y for each X point

                for (double y = -distanceyIncrement; y <= maxYDistance; y += distanceyIncrement)
                {
                    XYZ newPointY = new XYZ(newPointPositiveX.X, newPointPositiveX.Y + y, selectedPoint.Z);
                    pointsList.Add(newPointY);
                }
                for (double y = -distanceyIncrement; y <= maxYDistance; y += distanceyIncrement)
                {
                    XYZ newPointY = new XYZ(newPointPositiveX.X, newPointPositiveX.Y - y, selectedPoint.Z);
                    pointsList.Add(newPointY);
                }
            }

            // Generate points in the negative X direction
            for (double xDistance = -distanceXIncrement; xDistance >= -maxXDistance; xDistance -= distanceXIncrement)
            {
                XYZ newPointNegativeX = new XYZ(selectedPoint.X + xDistance, selectedPoint.Y, selectedPoint.Z);
                pointsList.Add(newPointNegativeX);

                // Generate Y points for each negative X point

                for (double y = -distanceyIncrement; y <= maxYDistance; y += distanceyIncrement)
                {
                    XYZ newPointY = new XYZ(newPointNegativeX.X, newPointNegativeX.Y + y, selectedPoint.Z);
                    pointsList.Add(newPointY);
                }
                for (double y = -distanceyIncrement; y <= maxYDistance; y += distanceyIncrement)
                {
                    XYZ newPointY = new XYZ(newPointNegativeX.X, newPointNegativeX.Y - y, selectedPoint.Z);
                    pointsList.Add(newPointY);
                }
            }

            return pointsList;
        }

        public static void GetHighestDifferences(List<CurveElement> selectedBoundaryLines, out double maxXDifference, out double maxYDifference)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (CurveElement curveElement in selectedBoundaryLines)
            {
                Curve curve = curveElement.GeometryCurve;

                if (curve != null)
                {
                    // Get the start and end points of the curve
                    XYZ startPoint = curve.GetEndPoint(0);
                    XYZ endPoint = curve.GetEndPoint(1);

                    // Update min and max X coordinates
                    minX = Math.Min(minX, Math.Min(startPoint.X, endPoint.X));
                    maxX = Math.Max(maxX, Math.Max(startPoint.X, endPoint.X));

                    // Update min and max Y coordinates
                    minY = Math.Min(minY, Math.Min(startPoint.Y, endPoint.Y));
                    maxY = Math.Max(maxY, Math.Max(startPoint.Y, endPoint.Y));
                }
            }

            // Calculate the differences
            maxXDifference = maxX - minX;
            maxYDifference = maxY - minY;
        }
        public static double MmToFeet(double millimeters)
        {
            const double millimetersToFeetConversionFactor = 1 / 304.8;
            return millimeters * millimetersToFeetConversionFactor;
        }
    }
}
