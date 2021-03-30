#region Namespaces
using System;
using System.Collections.Generic;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

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

            for (int i = 0; i < 3; i++)
            {
                AddWindow(walls[i]);
            }

            AddRoof(walls);

        }

        private void AddWindow(Wall wall)
        {
            throw new NotImplementedException();
        }

        private void AddDoor(Wall wall)
        {
            throw new NotImplementedException();
        }

        private List<Wall> CreateWalls()
        {
            throw new NotImplementedException();
        }
    }
}
