#region Namespaces
using System;
using System.Collections.Generic;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using GrimshawRibbon;

using Util;
#endregion

namespace IntroCs
{
    [Transaction(TransactionMode.Manual)]
    public class ModelCreation: IExternalCommand
    {
        Application _app;
        Document _doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet element)
        {
            UIApplication rvtUIApp = commandData.Application;
            UIDocument uiDoc=rvtUIApp.ActiveUIDocument;
            _app = rvtUIApp.Application;
            _doc = uiDoc.Document;

            using (Transaction transaction=new Transaction(_doc))
            {
                transaction.Start("Create House");

                //создадим простой дом
                CreateHouse();
                transaction.Commit();
            }
            return Result.Succeeded;
        }

        private void CreateHouse()
        {
            List<Wall> walls = CreateWalls(); //создаём стены

            AddDoor(walls[0]); //добавляем дверь в первую стену

            for (int i = 1; i < 4; i++)
            {
                AddWindow(walls[i]);
            }

            AddRoof(walls);

        }

        public void AddRoof(List<Wall> walls)
        {
            // Hard coding the roof type we will use. 
            // E.g., "Basic Roof: Generic - 400mm" 

            const string roofFamilyName = "Базовая крыша";
            const string roofTypeName = Constant.RoofTypeName; 
            const string roofFamilyAndTypeName = roofFamilyName + ": " + roofTypeName;

            // Find the roof type 

            RoofType roofType = (RoofType)ElementFiltering.FindFamilyType(_doc, typeof(RoofType), roofFamilyName, roofTypeName, null);

            if (roofType == null)
            {
                TaskDialog.Show("Add roof","Cannot find (" + roofFamilyAndTypeName +"). Maybe you use a different template? Try with DefaultMetric.rte.");
            }

            // Wall thickness to adjust the footprint of the walls 
            // to the outer most lines. 
            // Note: this may not be the best way, 
            // but we will live with this for this exercise. 

            
            double wallThickness = walls[0].Width;

            double dt = wallThickness / 2.0;
            List<XYZ> dts = new List<XYZ>(5);
            dts.Add(new XYZ(-dt, -dt, 0.0));
            dts.Add(new XYZ(dt, -dt, 0.0));
            dts.Add(new XYZ(dt, dt, 0.0));
            dts.Add(new XYZ(-dt, dt, 0.0));
            dts.Add(dts[0]);

            // Set the profile from four walls 

            CurveArray footPrint = new CurveArray();
            for (int i = 0; i <= 3; i++)
            {
                LocationCurve locCurve = (LocationCurve)walls[i].Location;
                XYZ pt1 = locCurve.Curve.GetEndPoint(0) + dts[i];
                XYZ pt2 = locCurve.Curve.GetEndPoint(1) + dts[i + 1];
                Line line = Line.CreateBound(pt1, pt2);
                footPrint.Append(line);
            }

            // Get the level2 from the wall 

            ElementId idLevel2 = walls[0].get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
           
            Level level2 = (Level)_doc.GetElement(idLevel2); // since 2013

            // Footprint to model curve mapping 

            ModelCurveArray mapping = new ModelCurveArray();

            // Create a roof. 

            FootPrintRoof aRoof = _doc.Create.NewFootPrintRoof(footPrint, level2, roofType, out mapping);

            foreach (ModelCurve modelCurve in mapping)
            {
                aRoof.set_DefinesSlope(modelCurve, true);
                aRoof.set_SlopeAngle(modelCurve, 0.5);
            }
        }

        public void AddWindow(Wall hostWall)
        {
            // Hard coding the window type we will use. 
            // E.g., "M_Fixed: 0915 x 1830mm 

            const string windowFamilyName = Constant.WindowFamilyName;
            const string windowTypeName = Constant.WindowTypeName;
            const string windowFamilyAndTypeName = windowFamilyName + ": " + windowTypeName;
            double sillHeight = Constant.MmToFeet(915);

            // Get the door type to use. 

            FamilySymbol windowType = (FamilySymbol)ElementFiltering.FindFamilyType(_doc, typeof(FamilySymbol), windowFamilyName, windowTypeName, BuiltInCategory.OST_Windows);
            if (windowType == null)
            {
                TaskDialog.Show("Add window","Cannot find (" +windowFamilyAndTypeName +"). Maybe you use a different template? Try with DefaultMetric.rte.");
            }

            if (!windowType.IsActive)
                windowType.Activate();

            // Get the start and end points of the wall. 

            LocationCurve locCurve = (LocationCurve)hostWall.Location;

            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);
            // Calculate the mid point. 
            XYZ pt = (pt1 + pt2) / 2.0;

            // we want to set the reference as a bottom of the wall or level1. 

            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            
            Level level1 = (Level)_doc.GetElement(idLevel1); // since 2013

            // Finally create a window. 

            FamilyInstance aWindow = _doc.Create.NewFamilyInstance(pt, windowType, hostWall, level1, StructuralType.NonStructural);

            aWindow.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(sillHeight);
        }

        public void AddDoor(Wall hostWall)
        {
            const string doorFamilyName = Constant.DoorFamilyName;
            const string doorTypeName = Constant.DoorTypeName;
            const string doorFamilyAndTypeName = doorFamilyName + ": " + doorTypeName;

            // Get the door type to use. 

            FamilySymbol doorType = (FamilySymbol)ElementFiltering.FindFamilyType(_doc, typeof(FamilySymbol), doorFamilyName, doorTypeName, BuiltInCategory.OST_Doors);

            if (doorType == null)
            {
                TaskDialog.Show("Add door", "Cannot find (" + doorFamilyAndTypeName + "). maybe you use a differnt template? Try with DefaultMetric.rte.");
            }

            if (!doorType.IsActive)
                doorType.Activate();

            // Get the start and end points of the wall. 

            LocationCurve locCurve = (LocationCurve)hostWall.Location;

            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);

            XYZ pt = (pt1 + pt2) / 2.0;   // Calculate the mid point. 

            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId(); //level wall

            Level level1 = (Level)_doc.GetElement(idLevel1);

            FamilyInstance aDoor = _doc.Create.NewFamilyInstance(pt, doorType, hostWall, level1, StructuralType.NonStructural);

        }

        public List<Wall> CreateWalls()
        {
            double width = Constant.MmToFeet(10000.0);
            double depth = Constant.MmToFeet(5000.0);

            Level level1 = (Level)ElementFiltering.FindElement(_doc, typeof(Level), "Level 1", null);
            if (level1 == null)
            {
                TaskDialog.Show("Create walls", "Cannot find (Level 1). Maybe you use a different template? Try with DefaultMetric.rte");
                return null;
            }

            Level level2 = (Level)ElementFiltering.FindElement(_doc, typeof(Level), "Level 2", null);
            if (level2 == null)
            {
                TaskDialog.Show("Create walls", "Cannot find(Level 2). Maybe you use a different template? Try with DifaulMetric.rte.");
                return null;
            }

            double dx = width / 2.0;
            double dy = depth / 2.0;

            List<XYZ> pts = new List<XYZ>(5);
            pts.Add(new XYZ(-dx, -dy, 0.0));
            pts.Add(new XYZ(dx, -dy, 0.0));
            pts.Add(new XYZ(dx, dy, 0.0));
            pts.Add(new XYZ(-dx, dy, 0.0));
            pts.Add(pts[0]);


            bool isStructural = false; // Flag for structural wall or not. 

            List<Wall> walls = new List<Wall>(4);  // Save walls we create. 

            for (int i = 0; i <= 3; i++)
            {
                Line baseCurve = Line.CreateBound(pts[i], pts[i + 1]); //создаем границу

                Wall aWall = Wall.Create(_doc, baseCurve, level1.Id, isStructural); //create a wall

                aWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id); //set the top constraction to level 2

                walls.Add(aWall);

            }

            _doc.Regenerate();
            _doc.AutoJoinElements();

            return walls;

        }
    }
}
