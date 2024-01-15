using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PanelsDistribute.Application;
using PanelsDistribute.Command;
using PanelsDistribute.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitTemplate.ViewModel
{
    public class MainWindowViewModel :PropChanged
    {
        public static ExternalEvent m_ExEvent { get; set; }
        public static RequestHandler m_Handler { get; set; }
        public UIDocument Uidoc { get; set; } = EntryPoint.CommandUIdoc;
        public Document doc { get; set; } = EntryPoint.CommandDoc;
        public Autodesk.Revit.ApplicationServices.Application CurrentApp { get; set; } = EntryPoint.CommandApp;
        
        public MyCommand SelectingCommand {  get; set; }

        public static double vlDistance;

        public double VlDistance
        {
            get { return vlDistance; }
            set { vlDistance = value;
                OnPropertyChanged();
            }
        }

        public static double hzDistance;

        public double HzDistance
        {
            get { return hzDistance; }
            set
            {
                hzDistance = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            vlDistance = 0;
            hzDistance = 0;
            SelectingCommand = new MyCommand(SelectingMethod);
        }

     

        private void SelectingMethod(object obj)
        {
            if(hzDistance != 0 && vlDistance != 0)
            {
            MakeRequest(RequestId.SelectRequest);
            }
            else
            {
                TaskDialog.Show("Alert","Please enter horizontal and vertical distances");
            }
        }

        private void MakeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
        }
    }
}
