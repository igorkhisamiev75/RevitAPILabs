using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GenerateFloorByRoom
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members Implementation

        /// <summary>
        ///     Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var tran = new Transaction(commandData.Application.ActiveUIDocument.Document, "Generate Floor");
            tran.Start();

            try
            {
                if (null == commandData) throw new ArgumentNullException("commandData");

                var data = new Data();
                data.ObtainData(commandData);

                var dlg = new GenerateFloorForm(data);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    CreateFloor(data, commandData.Application.ActiveUIDocument.Document);

                    tran.Commit();
                    return Result.Succeeded;
                }

                tran.RollBack();
                return Result.Cancelled;
            }
            catch (Exception e)
            {
                message = e.Message;
                tran.RollBack();
                return Result.Failed;
            }
        }

        #endregion IExternalCommand Members Implementation

        /// <summary>
        ///     create a floor by the data obtain from revit.
        /// </summary>
        /// <param name="data">Data including the profile, level etc, which is need for create a floor.</param>
        /// <param name="doc">Retrieves an object that represents the currently active project.</param>
        public static void CreateFloor(Data data, Document doc)
        {
            doc.Create.NewFloor(data.Profile, data.FloorType, data.Level, data.Structural);
        }
    }
}