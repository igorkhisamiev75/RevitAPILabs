using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;


namespace GrimshawRibbon
{
    class App : IExternalApplication
    {
        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            
            String tabName = "MRGT_TOOL's";    // создание вкладки
            application.CreateRibbonTab(tabName);

            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Надстройки"); //подпись под кнопками

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // create push button for CurveTotalLength  //namespace и класс
            PushButtonData b1Data = new PushButtonData("cmdCurveTotalLength", "Длина" + Environment.NewLine + "  Линий детализаций ", thisAssemblyPath, "TotalLength.CurveTotalLength");

            PushButton pb1 = ribbonPanel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Выбор линий для подсчета длинны";

            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/GrimshawRibbon;component/Resources/totalLength.png"));
            pb1.LargeImage = pb1Image;

            //попробуем создать еще одну кнопку
            ribbonPanel.AddSeparator(); //разделитель между кнопками

            // Get dll assembly path
            //string thisAssemblyPath2 = Assembly.GetExecutingAssembly().Location;

            // create push button for CurveTotalLength
            PushButtonData b1Data2 = new PushButtonData("cmdDbElement", "Инфо Элемента ", thisAssemblyPath, "GrimshawRibbon2.DbElement");

            PushButton pb2 = ribbonPanel.AddItem(b1Data2) as PushButton;
            pb2.ToolTip = "Информация о выбраннм объекте";

            BitmapImage pb2Image = new BitmapImage(new Uri("pack://application:,,,/GrimshawRibbon;component/Resources/totalLength.png"));
            pb2.LargeImage = pb2Image;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // do nothing
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // call our method that will load up our toolbar
            AddRibbonPanel(application);
            return Result.Succeeded;
        }
    }
}