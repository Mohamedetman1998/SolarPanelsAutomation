using Autodesk.Revit.UI;
using RevitTemplate.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PanelsDistribute.Application
{
    public class App : IExternalApplication
    {
        public static App thisApp;
        public MainWindow Win;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        public Result OnStartup(UIControlledApplication application)
        {
            #region Ribbon

            string RibbonTabName = "3SixT";

            application.CreateRibbonTab(RibbonTabName);

            string path = Assembly.GetExecutingAssembly().Location;

            #endregion

            #region Ribbon Panel
            RibbonPanel RibbonPanel = application.CreateRibbonPanel(RibbonTabName, "Distribution");
            #endregion

            #region Button

            PushButtonData Button = new PushButtonData("Button", "Panels Automation", path, "PanelsDistribute.Application.EntryPoint");

            BitmapImage bti = new BitmapImage(new Uri("pack://application:,,,/PanelsDistribute;component/Resources/solar-panel-32.png"));

            #endregion

            PushButton btn = RibbonPanel.AddItem(Button) as PushButton;
            btn.LargeImage = bti;

            return Result.Succeeded;

        }
        #region Show a Window
        public void ShowWindow() 
        {
            if (Win == null)
            {
                RequestHandler _Handler = new RequestHandler();
                MainWindowViewModel.m_Handler = _Handler;
                ExternalEvent exe = ExternalEvent.Create(_Handler);
                MainWindowViewModel.m_ExEvent = exe;
                Win = new MainWindow();
                Win.Show();

            }
        }
        #endregion

    }
}
