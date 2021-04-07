#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using System.Linq;
using System.Drawing;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion


namespace GrimshawRibbon
{
    [Transaction(TransactionMode.Manual)]
    public class BoundaryRooms : IExternalCommand
    {

        Application _app;
        Document _doc;
        

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            _app = uiApp.Application;
            _doc = uiDoc.Document;
          


            using (Transaction transaction = new Transaction(_doc))
            {
                transaction.Start("Create ");
                RoomFilter filter = new RoomFilter();

                FilteredElementCollector collector2 = new FilteredElementCollector(_doc);
                IList<Element> rooms = collector2.WherePasses(filter).ToElements();

                GetInfo_BoundarySegment((Room)rooms);


                transaction.Commit();
            }


            return Result.Succeeded;
        }

        public void GetInfo_BoundarySegment(Room room)
        {
            IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            if (null != segments)  //the room may not be bound
            {
                string message = "BoundarySegment";
                foreach (IList<BoundarySegment> segmentList in segments)
                {
                    foreach (BoundarySegment boundarySegment in segmentList)
                    {

                        // Get curve start point
                        message += "\nCurve start point: (" + boundarySegment.GetCurve().GetEndPoint(0).X + ","
                                       + boundarySegment.GetCurve().GetEndPoint(0).Y + "," +
                                      boundarySegment.GetCurve().GetEndPoint(0).Z + ")";
                        // Get curve end point
                        message += ";\nCurve end point: (" + boundarySegment.GetCurve().GetEndPoint(1).X + ","
                             + boundarySegment.GetCurve().GetEndPoint(1).Y + "," +
                             boundarySegment.GetCurve().GetEndPoint(1).Z + ")";
                        // Get document path name
                        message += ";\nDocument path name: " + room.Document.PathName;
                        // Get boundary segment element name
                        if (boundarySegment.ElementId != ElementId.InvalidElementId)
                        {
                            message += ";\nElement name: " + room.Document.GetElement(boundarySegment.ElementId).Name;
                        }
                    }
                }
                TaskDialog.Show("Revit", message);
            }
        }

    }
}