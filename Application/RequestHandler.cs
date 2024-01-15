using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Xml.Linq;
using Autodesk.Revit.UI;
using RevitTemplate.Utilities;
using Autodesk.Revit.DB;
using RevitTemplate.ViewModel;

namespace PanelsDistribute.Application
{
    public class RequestHandler : IExternalEventHandler
    {
        private Request m_request = new Request();
        public Request Request
        {
            get { return m_request; }
        }
        public void Execute(UIApplication app)
        {

            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }

                    case RequestId.SelectRequest:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(EntryPoint.CommandDoc, "Panel Distribute 3SixT");
                            transactionGroup.Start();

                            try
                            {
                                SelectFamilyMethod();

                                // Commit the transaction group if everything is successful
                                transactionGroup.Assimilate();
                            }
                            catch (Exception ex)
                            {
                                // Handle exceptions if needed
                                TaskDialog.Show("Error", $"An error occurred: {ex.Message}");
                                transactionGroup.RollBack();
                            }
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
            finally
            {
                //ExternalApplication.thisApp.WakeFormUp();
            }

            return;
        }

        private void SelectFamilyMethod()
        {
            double maxX;
            double maxHeight;
            RevitUtils.GetBoundryLines();
            if (RevitUtils.selectedBoundaryLines.Count > 0)
            {
              
                RevitUtils.GetFirstInstance();
                 
                if (IsValidPoints())
                {
                    RevitUtils.GetHighestDifferences(RevitUtils.selectedBoundaryLines, out maxX, out maxHeight);
                    List<XYZ> generatedPoints = RevitUtils.GeneratePointsAroundSelectedPointOld(RevitUtils.SelectedPanelLocationPoint, maxX + 50, maxHeight + 50);
                    List<XYZ> distinctPoints = RevitUtils.RemoveDuplicatePoints(generatedPoints);
                    var filteredPoints = RevitUtils.filterPoints(distinctPoints);
                    RevitUtils.CreateInstancesAtSegments(filteredPoints);


                }
                else
                {
                    TaskDialog.Show("Alert", "Distance given is too small , panels will clash");
                }

            }
            else
            {
                TaskDialog.Show("Alert", "No boundries selected");
            }
        }

        private bool IsValidPoints()
        {
            var hzDistanceFt = RevitUtils.MmToFeet(MainWindowViewModel.hzDistance);
            var vlDistanceFt = RevitUtils.MmToFeet(MainWindowViewModel.vlDistance);

            if (hzDistanceFt <= (RevitUtils.PanelWidth) || vlDistanceFt <= (RevitUtils.PanelHeight))
            {
                return false;
            }
            else
            {
                return true;
            }
        }






        public string GetName()
        {
            return "None";
        }
    }
}
