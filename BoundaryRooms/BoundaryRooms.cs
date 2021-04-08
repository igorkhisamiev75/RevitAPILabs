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
    //[RegenerationAttribute(RegenerationOption.Manual)]
    public class BoundaryRooms : IExternalCommand
    {
        Application _app;
        Document _doc;
        private const double PRECISION = 0.00000001;
        private CurveArray profile;
        private Hashtable floorTypes;
        private List<string> floorTypesName;
        private FloorType floorType;
        private Level level;
        private bool structural;
        
        private Autodesk.Revit.Creation.Application creApp;
        private Document document;

        // a floor type to be used by the new floor instead of the default type
        public FloorType FloorType
        {
            get
            {
                return floorType;
            }
            set
            {
                floorType = value;
            }
        }

        // the Level on which the floors are to be placed
        public Level Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        // an array of planar lines and arcs that represent the horizontal profile of the floor
        public CurveArray Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
            }
        }

        // determine wether the floor is structural
        public bool Structural
        {
            get
            {
                return structural;
            }
            set
            {
                structural = value;
            }
        }

        // a list of all Floor Types Name that could be used by the new Floor
        public List<string> FloorTypesName
        {
            get
            {
                return floorTypesName;
            }
            set
            {
                floorTypesName = value;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            _app = uiApp.Application;
            _doc = uiDoc.Document;
            creApp = commandData.Application.Application.Create;

            //// Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            collector.OfClass(typeof(FloorType));
            FloorType floorType = collector.FirstElement() as FloorType;

            // The normal vector (0,0,1) that must be perpendicular to the profile.
            XYZ normal = XYZ.BasisZ;

            //уровень 1
            //FilteredElementCollector COllector = new FilteredElementCollector(_doc).OfClass(typeof(Level));
            //var ele = from Element in COllector where Element.Name == "Level 1" select Element;
            //Level lev = ele.Cast<Level>().ElementAt<Level>(0);

            //Get room's
            //FilteredElementCollector newRoomFilter = new FilteredElementCollector(_doc);
            //ICollection<Element> allRooms = newRoomFilter.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();

            //Get room's 
            RoomFilter filter = new RoomFilter();
            // Apply the filter to the elements in the active document
            FilteredElementCollector collector3 = new FilteredElementCollector(_doc);
            IList<Element> rooms = collector3.WherePasses(filter).ToElements();


            for (int i=0; i < rooms.Count; i++)
            {
                CurveArray roomsCurves = new CurveArray();
                var r1 = (Room)rooms[i];

                SpatialElementBoundaryOptions bo = new SpatialElementBoundaryOptions();
                IList<IList<BoundarySegment>> segments2 = r1.GetBoundarySegments(bo);


                CurveArray temp = new CurveArray();

                if (null != segments2)
                {
                    foreach (IList<BoundarySegment> segmentList in segments2)
                    {
                        foreach (BoundarySegment boundarySegment in segmentList)
                        {
                            Curve c = boundarySegment.GetCurve();
                            temp.Append(c);
                        }
                        SortCurves(temp);
                    }
                }

                using (Transaction transaction = new Transaction(_doc))
                {
                    transaction.Start("Create ");
                    try
                    {

                        _doc.Create.NewFloor(temp, floorType, r1.Level, false);
                        
                    }
                    catch
                    {
                        TaskDialog.Show("Ошибка", "Ты не красавчик!☺☺☺");
                    }
                    transaction.Commit();
                }
            }


            TaskDialog.Show("У тебя получилось", "Ты красавчик!☺☺☺");

            return Result.Succeeded;

        }

        private void SortCurves(CurveArray lines)
        {
            XYZ temp = lines.get_Item(0).GetEndPoint(1);
            Curve temCurve = lines.get_Item(0);

            
            Profile = creApp.NewCurveArray();

            Profile.Append(temCurve);

            while (Profile.Size != lines.Size)
            {

                temCurve = GetNext(lines, temp, temCurve);

                if (Math.Abs(temp.X - temCurve.GetEndPoint(0).X) < PRECISION
                 && Math.Abs(temp.Y - temCurve.GetEndPoint(0).Y) < PRECISION)
                {
                    temp = temCurve.GetEndPoint(1);
                }
                else
                {
                    temp = temCurve.GetEndPoint(0);
                }

                Profile.Append(temCurve);
            }

        }
        private Curve GetNext(CurveArray profile, XYZ connected, Curve line)
        {
            foreach (Curve c in profile)
            {
                if (c.Equals(line))
                {
                    continue;
                }
                if ((Math.Abs(c.GetEndPoint(0).X - line.GetEndPoint(1).X) < PRECISION && 
                    Math.Abs(c.GetEndPoint(0).Y - line.GetEndPoint(1).Y) < PRECISION && 
                    Math.Abs(c.GetEndPoint(0).Z - line.GetEndPoint(1).Z) < PRECISION) && 
                    (Math.Abs(c.GetEndPoint(1).X - line.GetEndPoint(0).X) < PRECISION && 
                    Math.Abs(c.GetEndPoint(1).Y - line.GetEndPoint(0).Y) < PRECISION && 
                    Math.Abs(c.GetEndPoint(1).Z - line.GetEndPoint(0).Z) < PRECISION) && 
                    2 != profile.Size)
                {
                    continue;
                }

                if (Math.Abs(c.GetEndPoint(0).X - connected.X) < PRECISION && 
                    Math.Abs(c.GetEndPoint(0).Y - connected.Y) < PRECISION && 
                    Math.Abs(c.GetEndPoint(0).Z - connected.Z) < PRECISION)
                {
                    return c;
                }
                else if (Math.Abs(c.GetEndPoint(1).X - connected.X) < PRECISION && 
                         Math.Abs(c.GetEndPoint(1).Y - connected.Y) < PRECISION && 
                         Math.Abs(c.GetEndPoint(1).Z - connected.Z) < PRECISION)
                {
                    if (c.GetType().Name.Equals("Line"))
                    {
                        XYZ start = c.GetEndPoint(1);
                       XYZ end = c.GetEndPoint(0);
                        return Line.CreateBound(start, end);
                    }
                    else if (c.GetType().Name.Equals("Arc"))
                    {
                        int size = c.Tessellate().Count;
                        XYZ start = c.Tessellate()[0];
                        XYZ middle = c.Tessellate()[size / 2];
                        XYZ end = c.Tessellate()[size];

                        return Arc.Create(start, end, middle);
                    }
                }
            }
            throw new InvalidOperationException("The Room Boundary should be closed.");
        }

        /*public IList<IList<BoundarySegment>> GetInfo_BoundarySegment(Room room)
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

            return segments;
        }*/

    }
}
