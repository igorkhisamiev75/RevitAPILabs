#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion
namespace UiCs
{
    class UIRibbon:IExternalApplication
    {
        const string _introLabName = "IntroCs";
        const string _uiLabName = "UiCs";
        const string _dllExtension = ".dll";

        const string _imageFolderName = "Image";

        string _introLabPath;

        string _imageFolder;

        string FindFolderInParents(string path, string target)
        {
            Debug.Assert(Directory.Exists(path), "expected an existing directory to start search in");

            string s;

            do
            {
                s = Path.Combine(path, target);
                if (Directory.Exists(s))
                {
                    return s;
                }
                path = Path.GetDirectoryName(path);
            } while (null != path);

            return null; 
        }

        BitmapImage NewBitmapImage(string imageName)
        {
            return new BitmapImage(new Uri(Path.Combine(_imageFolder, imageName)));
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        public Result OnStartup (UIControlledApplication app)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _introLabPath = Path.Combine(dir, _introLabPath + _dllExtension);

            if (!File.Exists(_introLabPath))
            {
                TaskDialog.Show("UIRibbon", "External command assembly not found: " + _introLabPath);
                return Result.Failed;
            }

            _imageFolder = FindFolderInParents(dir, _imageFolderName);

            if(null==_imageFolder||!Directory.Exists(_imageFolder))
            {
                TaskDialog.Show("UIRibbon", string.Format("No image folder name '{0}' found in the parent directories of '{1}'", _imageFolderName, dir));

                return Result.Failed;
            }

            AddRibbonSampler(app);

            AddUILabsButtons(app);

            return Result.Succeeded;
        }
        private void AddRibbonSampler(UIControlledApplication app)
        {
            // (1) create a ribbon tab and ribbon panel 
            app.CreateRibbonTab("Ribbon Sampler");

            RibbonPanel panel = app.CreateRibbonPanel("Ribbom Sampler", "Ribbon sampler");

        }
        private void AddUILabsButtons(UIControlledApplication app)
        {
            throw new NotImplementedException();
        }


    }

    #region Helper Classes
    /// <summary>
    /// This lab uses Revit Intro Labs. 
    /// If you prefer to use a dummy command instead, you can do so. 
    /// Providing a command template here. 
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class DummyCommand1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Write your command implementation here 

            TaskDialog.Show("Dummy command", "You have called Command1");

            return Result.Succeeded;
        }
    }
    #endregion
}
