#region Namespaces
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices; // This is for Revit Application
#endregion

#region Description
// Revit Intro Lab - 1
//
// In this lab, you will learn how to "hook" your add-in to Revit.
// This lab defines a minimum external command.
//
// Explain about add-in manifest, how to create GUID.
// Hello World in VB.NET is from page 367 of Developer Guide.
#endregion

namespace IntroCs
{
    /// <summary>
    /// Hello World #1 - A minimal `hello, world` Revit 
    /// external command with all namespaces fully qualified.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class HelloWorld : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("My Dialog Title", "Hello World!");

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// Hello World #2 - simplified without full 
    /// namespace and using ReadOnly attribute.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class HelloWorldSimple : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("My Dialog Title", "Hello World Simple!");

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// Hello World #3 - minimum external application
    /// Difference: IExternalApplication instead of IExternalCommand.
    /// In addin manifest, use addin type "Application" and Name instead of Text tag.
    /// </summary>
    //public class HelloWorldApp : IExternalApplication
    //{
    //    // OnStartup() - called when Revit starts.

    //    public Result OnStartup(UIControlledApplication app)
    //    {
    //        TaskDialog.Show("My Dialog Title", "Hello World from App!");

    //        return Result.Succeeded;
    //    }

    //    // OnShutdown() - called when Revit ends.

    //    public Result OnShutdown(UIControlledApplication app)
    //    {
    //        return Result.Succeeded;
    //    }
    //}

    /// <summary>
    /// Command Arguments
    /// Take a look at the command arguments.
    /// commandData is the topmost object and
    /// provides the entry point to the Revit model.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CommandData : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // The first argument, commandData, provides access to the top most object model.
            // You will get the necessary information from commandData.
            // To see what's in there, print out a few data accessed from commandData
            //
            // Exercise: Place a break point at commandData and drill down the data.

            UIApplication uiApp = commandData.Application;
            Application rvtApp = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document rvtDoc = uiDoc.Document;

            // Print out a few information that you can get from commandData
            string versionName = rvtApp.VersionName;
            string documentTitle = rvtDoc.Title;

            TaskDialog.Show("Revit Intro Lab", "Version Name = " + versionName + "\nDocument Title = " + documentTitle);

            // Print out a list of wall types available in the current rvt project:

            FilteredElementCollector wallTypes  = new FilteredElementCollector(rvtDoc).OfClass(typeof(WallType));

            string s = "";
            foreach (WallType wallType in wallTypes)
            {
                s += wallType.Name + "\r\n";
            }

            // Show the result:

            TaskDialog.Show("Revit Intro Lab","Wall Types (in main instruction):\n\n" + s);

            // Return failure code to demonstrate output argument handling.
            // 2nd and 3rd arguments are displayed to the user when the command fails.
            // 2nd - set a message to display to the user.
            // 3rd - set elements to highlight on screen.

            return Result.Failed;
        }

        //#region Revit Macros
        //public void MyFirstMacro()
        //{
        //    TaskDialog.Show("My First Macro", "Hello World!");
        //}

        //public void MyFirstMacro2()
        //{
        //    TaskDialog.Show("My First Macro", "The current model file is ");
        //    //+ this.Application.ActiveUIDocument.Document.PathName );
        //}
        //#endregion // Revit Macros
    }
}
