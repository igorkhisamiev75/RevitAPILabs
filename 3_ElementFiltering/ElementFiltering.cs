#region Copyright
//
// Copyright (C) 2009-2021 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
// Migrated to C# by Adam Nagy 
// 
#endregion // Copyright

#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;



using System.Linq;
using System.Drawing;
using GenerateFloorByRoom;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


using Util;
#endregion

#region Description
// Revit Intro Lab 3 
// 
// In this lab, we'll take a look how to filter element from the database. 
// Disclaimer: minimum error checking to focus on the main topic. 
// 
#endregion

namespace GrimshawRibbon
{
    /// <summary>
    /// ElementFiltering
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ElementFiltering : IExternalCommand
    {
        // Member variables 
        Application _app;
        Document _doc;
        List<Room> projectRooms = new List<Room>();   // a list to store all rooms in the project

        private Hashtable m_floorTypes;
        private List<string> m_floorTypesName;
        private FloorType m_floorType;
        private Level m_level;
        private CurveArray m_profile;
        private bool m_structural;
        //private System.Drawing.PointF[] m_points;
        private double m_maxLength;
        private const double PRECISION = 0.00000001;
        private Autodesk.Revit.Creation.Application m_creApp;
        private Document m_document;

        public FloorType FloorType
        {
            get
            {
                return m_floorType;
            }
            set
            {
                m_floorType = value;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the access to the top most objects. 
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            _app = uiApp.Application;
            _doc = uiDoc.Document;

            // Get the Revit document
            //Autodesk.Revit.DB.Document _doc = application.ActiveUIDocument.Document;

            // Get the application creation object
            //Autodesk.Revit.Creation.Application appCreation = _app.Create;

            //// Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            collector.OfClass(typeof(FloorType));
            FloorType floorType = collector.FirstElement() as FloorType;

            // Build a floor profile for the floor creation
            XYZ first = new XYZ(0, 0, 0);
            XYZ second = new XYZ(20, 0, 0);
            XYZ third = new XYZ(20, 19, 0);
            XYZ fourth = new XYZ(0, 15, 0);
            CurveArray curves = new CurveArray();
            curves.Append(Line.CreateBound(first, second));
            curves.Append(Line.CreateBound(second, third));
            curves.Append(Line.CreateBound(third, fourth));
            curves.Append(Line.CreateBound(fourth, first));

            // The normal vector (0,0,1) that must be perpendicular to the profile.
            XYZ normal = XYZ.BasisZ;

            Level level1 = (Level)ElementFiltering.FindElement(_doc, typeof(Level), "Level 1", null);

            //using (Transaction transaction = new Transaction(_doc))
            //{
            //    transaction.Start("Create ");

            //    _doc.Create.NewFloor(curves, floorType, level1, true);
            //    transaction.Commit();
            //}


            using (Transaction transaction = new Transaction(_doc))
            {
                transaction.Start("Create ");

                NewMethod(floorType, curves, level1);
                transaction.Commit();
            }


            //ListFamilyTypes(); //поиск всех типов в проекте

            //ListRoom();





            // (2) List instances of specific object class. 
            //ListInstances(); //поиск всех экземляров

            //// (3) Find a specific family type. 
            //FindFamilyType();

            ////FindFamilyType_Wall_v2("Базовая стена", "Generic - 200mm");


            //// (4) Find specific instances, including filtering by parameters. 
            //FindInstance();

            //// (5) List all elements. 
            //ListAllElements();


            //Level level1 = (Level)FindElement(_doc, typeof(Level), "Level 1", null);


            //IList<Element> walls = FindInstancesOfType(typeof(Wall), idWallType, null);
            //IList<Element> doors = FindInstancesOfType(typeof(FamilyInstance), idDoorType, BuiltInCategory.OST_Doors);

            return Result.Succeeded;
        }

        private void NewMethod(FloorType floorType, CurveArray curves, Level level1)
        {
            RoomFilter filter = new RoomFilter();

            FilteredElementCollector collector = new FilteredElementCollector(_doc);
            IList<Element> rooms = collector.WherePasses(filter).ToElements();

            int n = rooms.Count();

            foreach (Room room in rooms)
            {
                IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                CurveArray roomsCurves = new CurveArray(); // Array to hold curv data collected from room boundreis
                roomsCurves.Clear();

                SpatialElementBoundaryOptions bo = new SpatialElementBoundaryOptions();
                SketchPlane sp = _doc.ActiveView.SketchPlane;

                foreach (IList<BoundarySegment> lstBs in room.GetBoundarySegments(bo))
                {
                    foreach (BoundarySegment bs in lstBs)
                    {
                        roomsCurves.Append(bs.GetCurve());
                    }

                }
                _doc.Create.NewFloor(roomsCurves, floorType, level1, false);

            }

            //_doc.Create.NewFloor(curves, floorType, level1, true);
        }



        /*public Floor CreateFloor(UIApplication application, Level level) //метод который создает пол по координатам
        {
            // Get the Revit document
            Autodesk.Revit.DB.Document document = application.ActiveUIDocument.Document;

            // Get the application creation object
            Autodesk.Revit.Creation.Application appCreation = application.Application.Create;

            // Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(FloorType));
            FloorType floorType = collector.FirstElement() as FloorType;

            // Build a floor profile for the floor creation
            XYZ first = new XYZ(0, 0, 0);
            XYZ second = new XYZ(20, 0, 0);
            XYZ third = new XYZ(20, 15, 0);
            XYZ fourth = new XYZ(0, 15, 0);
            CurveArray cerve = new CurveArray();
            cerve.Append(Line.CreateBound(first, second));
            cerve.Append(Line.CreateBound(second, third));
            cerve.Append(Line.CreateBound(third, fourth));
            cerve.Append(Line.CreateBound(fourth, first));

            //получить кривую из помещения и передать в метод NewFloor



            // The normal vector (0,0,1) that must be perpendicular to the profile.
            XYZ normal = XYZ.BasisZ;

            return document.Create.NewFloor(cerve, floorType, level, true, normal);
        }*/

        /*  public List<Room> ListRoom()
          {
              RoomFilter filter = new RoomFilter();

              // Apply the filter to the elements in the active document
              FilteredElementCollector collector = new FilteredElementCollector(_doc);
              IList<Element> rooms = collector.WherePasses(filter).ToElements();



              int n = rooms.Count();

              string s = string.Empty;
              //int n = 0;


              foreach (Room room in rooms)
              {
                  IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                  CurveArray roomsCurves = new CurveArray(); // Array to hold curv data collected from room boundreis

                  ////var floorType = _doc.GetDefaultElementTypeId(ElementTypeGroup.FloorType);
                  //FloorType floorType = new FilteredElementCollector(_doc)
                  //.OfClass(typeof(FloorType))
                  //.First<Element>(e => e.Name.Equals("Generic 150mm\"")) as FloorType;

                  SpatialElementBoundaryOptions bo = new SpatialElementBoundaryOptions();
                  SketchPlane sp = _doc.ActiveView.SketchPlane;

                  foreach (IList<BoundarySegment> lstBs in room.GetBoundarySegments(bo))
                  {
                      foreach (BoundarySegment bs in lstBs)
                      {
                          roomsCurves.Append(bs.GetCurve());
                      }

                  }
                  _doc.Create.NewFloor(roomsCurves, floorType, room.Level, false);


                  //s += "\r\n" + room.ToString() + ":" + room.Name;
                  //++n;
              }

              TaskDialog.Show("ПОБЕДА", "Перекрытия сделаны");
              //TaskDialog.Show(n.ToString() + " помещений:", s);

              //FilteredElementIdIterator roomIter =
              //    (new FilteredElementCollector(_doc)).WherePasses(filter).GetElementIdIterator();
              //roomIter.Reset();

              //// try to find all the Rooms in the Project and add it to the List
              //while (roomIter.MoveNext())
              //{
              //    object obj = roomIter.Current;

              //    // find all Rooms on Condition IsLocatedOnLevel
              //    Room tmpRoom = obj as Room;
              //    if (null != tmpRoom && null != _doc.GetElement(tmpRoom.LevelId))
              //    {
              //        projectRooms.Add(tmpRoom);
              //        continue;
              //    }
              //}




              return projectRooms;

          }*/

        /// <summary>
        /// List the family types 
        /// </summary>
        public void ListFamilyTypes()
        {
            // (1) Get a list of family types available in the current rvt project. 
            // 
            // For system family types, there is a designated 
            // properties that allows us to directly access to the types. 
            // e.g., _doc.WallTypes 

            FilteredElementCollector wallTypes = new FilteredElementCollector(_doc).OfClass(typeof(WallType)); //ищут стены
            int n = wallTypes.Count();

            
            string s = string.Empty;
            //int n = 0;

            foreach (WallType wType in wallTypes)
            {
                s += "\r\n" + wType.Kind.ToString() + " : " + wType.Name;
                //++n;
            }
            TaskDialog.Show(n.ToString() + " Wall Types:", s);

            // (1.1) Same idea applies to other system family, such as Floors, Roofs. 

            //FloorTypeSet floorTypes = _doc.FloorTypes;

            FilteredElementCollector floorTypes = new FilteredElementCollector(_doc).OfClass(typeof(FloorType)); //перекрытия

            s = string.Empty;

            foreach (FloorType fType in floorTypes)
            {
                // Family name is not in the property for 
                // floor, so use BuiltInParameter here. 

                Parameter param = fType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);

                if (param != null)
                {
                    s += param.AsString();
                }
                s += " : " + fType.Name + "\r\n";
            }
            TaskDialog.Show(floorTypes.Count().ToString() + " floor types (by rvtDoc.FloorTypes): ", s);

            // (1.2a) Another approach is to use a filter. here is an example with wall type. 

            var wallTypeCollector1 = new FilteredElementCollector(_doc);
            wallTypeCollector1.WherePasses(new ElementClassFilter(typeof(WallType)));
            IList<Element> wallTypes1 = wallTypeCollector1.ToElements();

            // Using a helper function to display the result here. See code below. 

            ShowElementList(wallTypes1, "Wall Types (by Filter): ");

            // (1.2b) The following are the same as two lines above. 
            // These alternative forms are provided for convenience. 
            // Using OfClass() 
            // 
            //FilteredElementCollector wallTypeCollector2 = new FilteredElementCollector(_doc);
            //wallTypeCollector2.OfClass(typeof(WallType)); 

            // (1.2c) The following are the same as above for convenience. 
            // Using short cut this time. 
            // 
            //FilteredElementCollector wallTypeCollector3 = new FilteredElementCollector(_doc).OfClass(typeof(WallType)); 

            // 
            // (2) Listing for component family types. 
            // 
            // For component family, it is slightly different. 
            // There is no designate property in the document class. 
            // You always need to use a filtering. e.g., for doors and windows. 
            // Remember for component family, you will need to check element type and category. 

            var doorTypeCollector = new FilteredElementCollector(_doc);
            doorTypeCollector.OfClass(typeof(FamilySymbol));
            doorTypeCollector.OfCategory(BuiltInCategory.OST_Doors);
            IList<Element> doorTypes = doorTypeCollector.ToElements();

            ShowElementList(doorTypes, "Door Types (by Filter): ");

            //окна
            var windiwsTypeCollector = new FilteredElementCollector(_doc);
            windiwsTypeCollector.OfClass(typeof(FamilySymbol));
            windiwsTypeCollector.OfCategory(BuiltInCategory.OST_Windows);
            IList<Element> windowType = windiwsTypeCollector.ToElements();

            ShowElementList(windowType, "Окна типы (по фильтру):");

        }

        /// <summary>
        /// To get a list of instances of a specific family type, you will need to use filters. 
        /// The same idea that we learned for family types applies for instances as well. 
        /// </summary>
        public void ListInstances()
        {
            // List all the wall instances 
            var wallCollector = new FilteredElementCollector(_doc).OfClass(typeof(Wall));
            IList<Element> wallList = wallCollector.ToElements();

            ShowElementList(wallList, "Wall Instances: ");

            // List all the door instances 
            var doorCollector = new FilteredElementCollector(_doc).OfClass(typeof(FamilyInstance));
            doorCollector.OfCategory(BuiltInCategory.OST_Doors);
            IList<Element> doorList = doorCollector.ToElements();

            ShowElementList(doorList, "Door Instance: ");

            var winCollector = new FilteredElementCollector(_doc).OfClass(typeof(FamilyInstance));
            winCollector.OfCategory(BuiltInCategory.OST_Windows);
            IList<Element> winList = winCollector.ToElements();

            ShowElementList(winList, "Окна:");
        }

        /// <summary>
        /// Looks at a way to get to the more specific family types with a given name. 
        /// </summary> 
        public void FindFamilyType()
        {
            // In this exercise, we will look for the following family types for wall and door 
            // Hard coding for similicity. modify here if you want to try out with other family types. 

            // Constant to this function. 
            // This is for wall. e.g., "Basic Wall: Generic - 200mm" 
            const string wallFamilyName = Util.Constant.WallFamilyName;
            const string wallTypeName = Util.Constant.WallTypeName;
            const string wallFamilyAndTypeName = wallFamilyName + ": " + wallTypeName;

            // This is for door. e.g., "M_Single-Flush: 0915 x 2134mm 
            const string doorFamilyName = Util.Constant.DoorFamilyName;
            const string doorTypeName = Util.Constant.DoorTypeName;
            const string doorFamilyAndTypeName = doorFamilyName + ": " + doorTypeName;

            // Keep messages to the user in this function. 
            string msg = "Find Family Type - All:\r\n\r\n";

            // (1) Get a specific system family type. e.g., wall type. 
            // There are a few different ways to do this. 

            // (1.1) First version uses LINQ query. 

            ElementType wallType1 = (ElementType)FindFamilyType_Wall_v1(wallFamilyName, wallTypeName);

            // Show the result. 
            msg += ShowFamilyTypeAndId("Find wall family type (using LINQ): ",
              wallFamilyAndTypeName, wallType1) + "\r\n";

            // (1.2) Another way is to use iterator. (cf. look for example, Developer guide 87) 

            ElementType wallType2 = (ElementType)FindFamilyType_Wall_v2(wallFamilyName, wallTypeName);

            msg += ShowFamilyTypeAndId("Find wall family type (using iterator): ",
              wallFamilyAndTypeName, wallType2) + "\r\n";

            // (1.3) The most efficient method is to use a parameter filter, since 
            // this avoids mashalling and transporting all the data for the rejected 
            // results from the internal Revit memory to the external .NET space:

            ElementType wallType3 = FindFamilyType_Wall_v3(
              wallFamilyName, wallTypeName) as ElementType;

            msg += ShowFamilyTypeAndId("Find wall family type (using parameter filter): ",
              wallFamilyAndTypeName, wallType2) + "\r\n";

            // (2) Get a specific component family type. e.g., door type. 
            // 
            // (2.1) Similar approach as (1.1) using LINQ. 

            ElementType doorType1 = (ElementType)FindFamilyType_Door_v1(doorFamilyName, doorTypeName);

            msg += ShowFamilyTypeAndId("Find door type (using LINQ): ", doorFamilyAndTypeName, doorType1) + "\r\n";

            // (2.2) Get a specific door type. The second approach. 
            // Another approach will be to look up from Family, then from Family.Symbols property. 
            // This gets more complicated although it is logical approach. 

            ElementType doorType2 = (ElementType)FindFamilyType_Door_v2(doorFamilyName, doorTypeName);

            msg += ShowFamilyTypeAndId("Find door type (using Family): ", doorFamilyAndTypeName, doorType2) + "\r\n";

            // (3) Here is more generic form. defining a more generalized function below. 
            // 
            // (3.1) For the wall type 

            ElementType wallType4 = (ElementType)FindFamilyType(_doc, typeof(WallType), wallFamilyName, wallTypeName, null);

            msg += ShowFamilyTypeAndId("Find wall type (using generic function): ", wallFamilyAndTypeName, wallType4) + "\r\n";

            // (3.2) For the door type. 

            ElementType doorType3 = (ElementType)FindFamilyType(_doc, typeof(FamilySymbol), doorFamilyName, doorTypeName, BuiltInCategory.OST_Doors);

            msg += ShowFamilyTypeAndId("Find door type (using generic function): ", doorFamilyAndTypeName, doorType3) + "\r\n";

            // (3.3) Simply return the first wall and door type encountered, so we do 
            // not have to worry about setting up its family and symbol name 
            // correctly: 

            ElementType wallType5 = new FilteredElementCollector(_doc)
              .WhereElementIsElementType() // superfluous, because WallType is derived from ElementType
              .OfClass(typeof(WallType))
              .OfCategory(BuiltInCategory.OST_Walls) // superfluous, because al WallType instances have this category
              .FirstElement() as ElementType;

            msg += ShowFamilyTypeAndId("Find first wall type (using generic function): ", wallType5.Name, wallType5) + "\r\n";

            ElementType doorType4 = GetFirstFamilySymbol(_doc, BuiltInCategory.OST_Doors) as ElementType;

            msg += ShowFamilyTypeAndId("Find first door type (using generic function): ", doorType4.Name, doorType4) + "\r\n";

            // Finally, show the result all together 

            TaskDialog.Show("Find family types", msg);
        }

        /// <summary>
        /// Find a specific family type for a wall with a given family and type names. 
        /// This version uses LINQ query. 
        /// </summary>
        public Element FindFamilyType_Wall_v1(string wallFamilyName, string wallTypeName)
        {
            // Narrow down a collector with class. 
            var wallTypeCollector1 = new FilteredElementCollector(_doc);
            wallTypeCollector1.OfClass(typeof(WallType));

            // LINQ query 
            var wallTypeElems1 =
                from element in wallTypeCollector1
                where element.Name.Equals(wallTypeName)
                select element;

            // Get the result. 
            Element wallType1 = null; // Result will go here. 
                                      // (1) Directly accessing from the query result. 
            if (wallTypeElems1.Count<Element>() > 0)
            {
                wallType1 = wallTypeElems1.First<Element>();
            }

            // (2) If you want to get the result as a list of element, here is how. 
            //IList<Element> wallTypeList1 = wallTypeElems1.ToList();
            //if (wallTypeList1.Count > 0)
            //  wallType1 = wallTypeList1[0]; // Found it. 

           
            return wallType1;


        }

        /// <summary>
        /// Find a specific family type for a wall, which is a system family. 
        /// This version uses iteration. (cf. look for example, Developer guide 87) 
        /// </summary>
        public Element FindFamilyType_Wall_v2(string wallFamilyName, string wallTypeName)
        {
            // First, narrow down the collector by Class 
            var wallTypeCollector2 = new FilteredElementCollector(_doc).OfClass(typeof(WallType));

            // Use iterator 
            FilteredElementIterator wallTypeItr = wallTypeCollector2.GetElementIterator();
            wallTypeItr.Reset();
            Element wallType2 = null;
            while (wallTypeItr.MoveNext())
            {
                WallType wType = (WallType)wallTypeItr.Current;
                // We check two names for the match: type name and family name. 
                if ((wType.Name == wallTypeName) & (wType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(wallFamilyName)))
                {
                    wallType2 = wType; // We found it. 
                    break;
                }
            }

            return wallType2;
        }

        /// <summary>
        /// Find a specific family type for a wall, which is a system family. 
        /// Most efficient way to find a named family symbol: use a parameter filter.
        /// </summary>
        public Element FindFamilyType_Wall_v3(string wallFamilyName, string wallTypeName)
        {
            ParameterValueProvider provider
              = new ParameterValueProvider(
                new ElementId(BuiltInParameter.DATUM_TEXT));

            FilterStringRuleEvaluator evaluator
              = new FilterStringEquals();

            FilterRule rule = new FilterStringRule(
              provider, evaluator, wallTypeName, true);

            ElementParameterFilter filter
              = new ElementParameterFilter(rule);

            return new FilteredElementCollector(_doc)
              .OfClass(typeof(WallType))
              .WherePasses(filter)
              .FirstElement();
        }

        /// <summary>
        /// Find a specific family type for a door, which is a component family. 
        /// This version uses LINQ. 
        /// </summary>
        public Element FindFamilyType_Door_v1(string doorFamilyName, string doorTypeName)
        {
            // narrow down the collection with class and category. 
            var doorFamilyCollector1 = new FilteredElementCollector(_doc);
            doorFamilyCollector1.OfClass(typeof(FamilySymbol));
            doorFamilyCollector1.OfCategory(BuiltInCategory.OST_Doors);

            // Parse the collection for the given name 
            // Using LINQ query here. 
            var doorTypeElems =
                from element in doorFamilyCollector1
                where element.Name.Equals(doorTypeName) &&
                element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(doorFamilyName)
                select element;

            // Get the result. 
            Element doorType1 = null;
            // (1) Directly accessing from the query result 
            //if (doorTypeElems.Count > 0) // we should have only one with the given name. minimum error checking. 
            // doorType1 = doorTypeElems[0]; // found it. 

            // (2) If we want to get the list of element, here is how. 
            IList<Element> doorTypeList = doorTypeElems.ToList();
            if (doorTypeList.Count > 0)
            {
                // We should have only one with the given name. minimum error checking. 
                doorType1 = doorTypeList[0]; // Found it. 
            }

            return doorType1;
        }

        /// <summary>
        /// Find a specific family type for a door. 
        /// another approach will be to look up from Family, then from Family.Symbols property. 
        /// This gets more complicated although it is logical approach. 
        /// </summary>
        public Element FindFamilyType_Door_v2(string doorFamilyName, string doorTypeName)
        {
            // (1) find the family with the given name. 

            var familyCollector = new FilteredElementCollector(_doc);
            familyCollector.OfClass(typeof(Family));

            // Use the iterator 
            Family doorFamily = null;
            FilteredElementIterator familyItr = familyCollector.GetElementIterator();
            //familyItr.Reset(); 
            while ((familyItr.MoveNext()))
            {
                Family fam = (Family)familyItr.Current;
                // Check name and categoty 
                if ((fam.Name == doorFamilyName) & (fam.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_Doors))
                {
                    // We found the family. 
                    doorFamily = fam;
                    break;
                }
            }

            // (2) Find the type with the given name. 

            Element doorType2 = null;
            // Id of door type we are looking for. 
            if (doorFamily != null)
            {
                // If we have a family, then proceed with finding a type under Symbols property. 

                //FamilySymbolSet doorFamilySymbolSet = doorFamily.Symbols;       // 'Autodesk.Revit.DB.Family.Symbols' is obsolete: 
                // 'This property is obsolete in Revit 2015.  Use Family.GetFamilySymbolIds() instead.'

                // Iterate through the set of family symbols. 
                //FamilySymbolSetIterator doorTypeItr = doorFamilySymbolSet.ForwardIterator();
                //while (doorTypeItr.MoveNext())
                //{
                //  FamilySymbol dType = (FamilySymbol)doorTypeItr.Current;
                //  if ((dType.Name == doorTypeName))
                //  {
                //    doorType2 = dType;  // Found it. 
                //    break;
                //  }
                //}

                /// Following part is modified code for Revit 2015

                ISet<ElementId> familySymbolIds = doorFamily.GetFamilySymbolIds();

                if (familySymbolIds.Count > 0)
                {
                    // Get family symbols which is contained in this family
                    foreach (ElementId id in familySymbolIds)
                    {
                        FamilySymbol dType = doorFamily.Document.GetElement(id) as FamilySymbol;
                        if ((dType.Name == doorTypeName))
                        {
                            doorType2 = dType;  // Found it. 
                            break;
                        }
                    }
                }

                /// End of modified code for Revit 2015          

            }
            return doorType2;
        }

        /// <summary>
        /// Find specific instances, including filtering by parameters. 
        /// </summary> 
        public void FindInstance()
        {
            // Constant to this function. (we may want to change the value here.) 
            // This is for wall. e.g., "Basic Wall: Generic - 200mm" 
            const string wallFamilyName = Util.Constant.WallFamilyName;
            const string wallTypeName = Util.Constant.WallTypeName; // Util.Constant.WallTypeName 
            const string wallFamilyAndTypeName = wallFamilyName + ": " + wallTypeName;

            // This is for door. e.g., "M_Single-Flush: 0915 x 2134mm 
            const string doorFamilyName = Util.Constant.DoorFamilyName;
            const string doorTypeName = Util.Constant.DoorTypeName;
            const string doorFamilyAndTypeName = doorFamilyName + ": " + doorTypeName;

            // (1) Find walls with a specific type 
            // 
            // Find a specific family type. use the function we defined earlier. 
            ElementId idWallType = FindFamilyType(_doc, typeof(WallType), wallFamilyName, wallTypeName, null).Id;
            // Find instances of the given family type. 
            IList<Element> walls = FindInstancesOfType(typeof(Wall), idWallType, null);

            // Show it. 
            string msgWalls = "Instances of wall with type: " + wallFamilyAndTypeName + "\r\n";
            ShowElementList(walls, msgWalls);

            // (2) Find a specific door. same idea. 
            ElementId idDoorType = FindFamilyType(_doc, typeof(FamilySymbol), doorFamilyName, doorTypeName, BuiltInCategory.OST_Doors).Id; // error in elementfiltering
            IList<Element> doors = FindInstancesOfType(typeof(FamilyInstance), idDoorType, BuiltInCategory.OST_Doors);

            string msgDoors = "Instances of door with type: " + doorFamilyAndTypeName + "\r\n";
            ShowElementList(doors, msgDoors);

            // (3) Apply the same idea to the supporting element, such as level. 
            // In this case, we simply check the name. 
            // This becomes handy when you are creating an object on a certain level, 
            // for example, when we create a wall. 
            // We will use this in the lab 5 when we create a simple house. 

            Level level1 = (Level)FindElement(_doc, typeof(Level), "Level 1", null);

            string msgLevel1 = "Level1: " + "\r\n" + ElementToString(level1) + "\r\n";
            TaskDialog.Show("Find instance", msgLevel1);

            // (4) Finally, let's see how to use parameter filter 
            // Let's try to get a wall whose length is larger than 60 feet. 

            IList<Element> longWalls = FindLongWalls();

            string msgLongWalls = "Long walls: " + "\r\n";

            ShowElementList(longWalls, msgLongWalls);
        }

        /// <summary>
        /// Helper function: find a list of element with given class, family type and category (optional). 
        /// </summary>
        public IList<Element> FindInstancesOfType(Type targetType, ElementId idType, Nullable<BuiltInCategory> targetCategory)
        {
            // First, narrow down to the elements of the given type and category 

            var collector = new FilteredElementCollector(_doc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // Parse the collection for the given family type id. 
            // Using LINQ query here. 
            var elems =
                from element in collector
                where element.get_Parameter(BuiltInParameter.SYMBOL_ID_PARAM).AsElementId().Equals(idType)
                select element;

            // Put the result as a list of element fo accessibility. 

            return elems.ToList();
        }

        /// <summary>
        /// Optional - example of parameter filter. 
        /// Find walls whose length is longer than a certain length. e.g., 60 feet 
        ///     wall.parameter(length) > 60 feet 
        /// This could get more complex than looping through in terms of writing a code. 
        /// See page 87 of Developer guide. 
        /// </summary> 
        public IList<Element> FindLongWalls()
        {
            // Constant for this function. 
            const double kWallLength = 60.0;  // 60 feet. hard coding for simplicity. 

            // First, narrow down to the elements of the given type and category 
            var collector = new FilteredElementCollector(_doc).OfClass(typeof(Wall));

            // Define a filter by parameter 
            // 1st arg - value provider 
            BuiltInParameter lengthParam = BuiltInParameter.CURVE_ELEM_LENGTH;
            int iLengthParam = (int)lengthParam;
            var paramValueProvider = new ParameterValueProvider(new ElementId(iLengthParam));

            // 2nd - evaluator 
            FilterNumericGreater evaluator = new FilterNumericGreater();

            // 3rd - rule value 
            double ruleVal = kWallLength;

            // 4th - epsilon 
            const double eps = 1E-06;

            // Define a rule 
            var filterRule = new FilterDoubleRule(paramValueProvider, evaluator, ruleVal, eps);

            // Create a new filter 
            var paramFilter = new ElementParameterFilter(filterRule);

            // Go through the filter 
            IList<Element> elems = collector.WherePasses(paramFilter).ToElements();

            return elems;
        }

        /// <summary>
        /// List all elements in Revit database.
        /// </summary>
        void ListAllElements()
        {
            // Create an output file:

            string filename = Path.Combine(Path.GetTempPath(), "RevitElements.txt");

            StreamWriter sw = new StreamWriter(filename);

            // The Revit API does not expect an application
            // ever to need to iterate over all elements.
            // To do so, we need to use a trick: ask for all
            // elements fulfilling a specific criteria and
            // unite them with all elements NOT fulfilling
            // the same criteria; an arbitrary criterion 
            // could be chosen:

            FilteredElementCollector collector  = new FilteredElementCollector(_doc).WhereElementIsElementType();

            FilteredElementCollector collector2 = new FilteredElementCollector(_doc).WhereElementIsNotElementType();

            collector.UnionWith(collector2);

            // Loop over the elements and list their data:

            string s, line;

            foreach (Element e in collector)
            {
                line = "Id=" + e.Id.IntegerValue.ToString(); // element id
                line += "; Class=" + e.GetType().Name; // element class, i.e. System.Type

                // The element category is not implemented for all classes,
                // and may return null; for family elements, one can sometimes
                // use the FamilyCategory property instead.

                s = string.Empty;

                if (null != e.Category)
                {
                    s = e.Category.Name;
                }
                if (0 == s.Length && e is Family && null != ((Family)e).FamilyCategory)
                {
                    s = ((Family)e).FamilyCategory.Name;
                }
                if (0 == s.Length)
                {
                    s = "?";
                }
                line += "; Category=" + s;

                // The element Name property has a different meaning for different classes,
                // but is mostly implemented 'logically'. More precise info on elements
                // can be obtained in class-specific ways.

                line += "; Name=" + e.Name;

                //line += "; UniqueId=" + e.UniqueId;
                //line += "; Guid=" + GetGuid( e.UniqueId );

                sw.WriteLine(line);
            }
            sw.Close();

            TaskDialog.Show("List all elements",
              string.Format("Element list has been written to '{0}'.", filename));
        }

        #region Helper Functions
        //====================================================================
        // Helper Functions 
        //====================================================================
        /// <summary>
        /// Helper function: find an element of the given type, name, and category(optional) 
        /// You can use this, for example, to find a specific wall and window family with the given name. 
        /// e.g., 
        /// FindFamilyType(_doc, GetType(WallType), "Basic Wall", "Generic - 200mm") 
        /// FindFamilyType(_doc, GetType(FamilySymbol), "M_Single-Flush", "0915 x 2134mm", BuiltInCategory.OST_Doors) 
        /// </summary>
        public static Element FindFamilyType(Document rvtDoc, Type targetType,
            string targetFamilyName, string targetTypeName, Nullable<BuiltInCategory> targetCategory)
        {
            // First, narrow down to the elements of the given type and category 

            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // Parse the collection for the given names 
            // Using LINQ query here. 

            var targetElems =
                from element in collector
                where element.Name.Equals(targetTypeName) &&
                element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).
                AsString().Equals(targetFamilyName)
                select element;

            // Put the result as a list of element fo accessibility. 

            IList<Element> elems = targetElems.ToList();

            // Return the result. 

            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        /// <summary>
        /// Return all elements of the requested class,
        /// i.e. System.Type, matching the given built-in
        /// category in the given document.
        /// </summary>
        public static FilteredElementCollector
          GetElementsOfType(
            Document doc,
            Type type,
            BuiltInCategory bic)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc);

            collector.OfCategory(bic);
            collector.OfClass(type);

            return collector;
        }

        /// <summary>
        /// Return all family symbols in the given document
        /// matching the given built-in category.
        /// Todo: Compare this with the FamilySymbolFilter class.
        /// </summary>
        public static FilteredElementCollector
          GetFamilySymbols(
            Document doc,
            BuiltInCategory bic)
        {
            return GetElementsOfType(doc,
              typeof(FamilySymbol), bic);
        }

        /// <summary>
        /// Return the first family symbol found in the given document
        /// matching the given built-in category or null if none is found.
        /// </summary>
        public static FamilySymbol GetFirstFamilySymbol(
          Document doc,
          BuiltInCategory bic)
        {
            FamilySymbol s = GetFamilySymbols(doc, bic).FirstElement() as FamilySymbol;

            Debug.Assert(null != s, string.Format(
              "expected at least one {0} symbol in project",
              bic.ToString()));

            return s;
        }

        /// <summary>
        /// Helper function: find a list of element with given Class, Name and Category (optional). 
        /// </summary>  
        public static IList<Element> FindElements(
          Document rvtDoc,
          Type targetType,
          string targetName,
          Nullable<BuiltInCategory> targetCategory)
        {
            // First, narrow down to the elements of the given type and category 
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // Parse the collection for the given names 
            // Using LINQ query here. 
            var elems =
                from element in collector
                where element.Name.Equals(targetName)
                select element;

            // Put the result as a list of element for accessibility. 

            return elems.ToList();
        }

        /// <summary>
        /// Helper function: searches elements with given Class, Name and Category (optional), 
        /// and returns the first in the elements found. 
        /// This gets handy when trying to find, for example, Level. 
        /// e.g., FindElement(_doc, GetType(Level), "Level 1") 
        /// </summary>
        public static Element FindElement(Document rvtDoc, Type targetType, string targetName, Nullable<BuiltInCategory> targetCategory)
        {
            // Find a list of elements using the overloaded method. 
            IList<Element> elems = FindElements(rvtDoc, targetType, targetName, targetCategory);

            // Return the first one from the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        /// <summary>
        /// Helper function: to show the result of finding a family type. 
        /// </summary>  
        public string ShowFamilyTypeAndId(string header, string familyAndTypeName, ElementType familyType)
        {
            // Show the result. 
            string msg = header + "\r\n" + familyAndTypeName + " >> Id = ";

            if (familyType != null)
            {
                msg += familyType.Id.ToString() + "\r\n";
            }

            // Uncomment this if you want to show each result. 
            //TaskDialog.Show( "Show family type and id", msg );

            return msg;
        }

        /// <summary>
        /// Helper function to display info from a list of elements passed onto. 
        /// </summary>  
        public void ShowElementList(IList<Element> elems, string header)
        {
            string s = " - Class - Category - Name (or Family: Type Name) - Id - \r\n";
            foreach (Element e in elems)
            {
                s += ElementToString(e);
            }
            TaskDialog.Show(header + "(" + elems.Count.ToString() + "):", s);
        }

        /// <summary>
        /// Helper function: summarize an element information as a line of text, 
        /// which is composed of: class, category, name and id. 
        /// name will be "Family: Type" if a given element is ElementType. 
        /// Intended for quick viewing of list of element, for example. 
        /// </summary> 
        public string ElementToString(Element e)
        {
            if (e == null)
            {
                return "none";
            }

            string name = "";

            if (e is ElementType)
            {
                Parameter param = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
                if (param != null)
                {
                    name = param.AsString();
                }
            }
            else
            {
                name = e.Name;
            }
            return e.GetType().Name + "; "
              + e.Category.Name + "; "
              + name + "; "
              + e.Id.IntegerValue.ToString() + "\r\n";
        }
        #endregion
    }
}