using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitTemplate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PanelsDistribute.Application
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class EntryPoint : IExternalCommand
    {
        public static UIDocument CommandUIdoc { get; set; }
        public static Document CommandDoc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application CommandApp { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            CommandApp = commandData.Application.Application;
            CommandUIdoc = commandData.Application.ActiveUIDocument;
            CommandDoc = CommandUIdoc.Document;

            #region Test
            //GetBoundryLines();
            //GetFirstInstance();

            //var bol =IsIntersectLine(PanelBoundryLines(SelectedPanelLocationPoint));
            //TaskDialog.Show("Alert", bol.ToString());
            //List<XYZ> generatedPoints = GeneratePointsAroundSelectedPoint2(SelectedPanelLocationPoint);
            //List<XYZ> distinctPoints = RemoveDuplicatePoints(generatedPoints);

            //CreateInstancesAtSegments(distinctPoints);
            //var bol = IsPointInsidePolygon(SelectedPanelLocationPoint, selectedBoundaryLines);
            //TaskDialog.Show("s", bol.ToString());

            //ShapeMaker(distinctPoints);
            #endregion

            App application = new App();
            App.thisApp = application;

            try
            {
                application.ShowWindow();
                return Result.Succeeded;
            }
            catch (Exception exp)
            {
                TaskDialog.Show("catch", exp.Message);

                //GetBoundryLines();
                //GetFirstInstance();
                //GetHighestDifferences(selectedBoundaryLines, out maxX, out maxHeight);
                //List<XYZ> generatedPoints = GeneratePointsAroundSelectedPointOld(SelectedPanelLocationPoint, maxX+50, maxHeight+50);
                //List<XYZ> distinctPoints = RemoveDuplicatePoints(generatedPoints);
                //var filteredPoints= filterPoints(distinctPoints);
                ////ShapeMaker(filteredPoints);

                //CreateInstancesAtSegments(filteredPoints);



            }
                return Result.Succeeded;

        }
    }
}
