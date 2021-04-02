#region Namespaces
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage; // needed for Extensible Storage 
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Util;
#endregion

namespace IntroCs
{
    [Transaction(TransactionMode.Manual)]
     class ExtensibleStorage:IExternalCommand
    {
        Guid _guid = new Guid("B80DC2A8-323B-4CB9-A50A-8AA95984E4DD");

        class WallSelectionFilter:ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return e is Wall;
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return true;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            //создаем транзакцию

            Transaction trans = new Transaction(doc, "Extensible Storage");
            trans.Start();

            //выбор стены

            Wall wall = null;

            try
            {
                Reference r = uiDoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter());
                wall = doc.GetElement(r) as Wall;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                message = "Ничего не выбрано; пожалуйста выбирете стену";
                return Result.Failed;
            }

            Debug.Assert(null != wall, "стены должны быть выьраны");

            if (null == wall)
            {
                message = "пожалуйста выбирете стену";
                return Result.Failed;
            }

            SchemaBuilder builder = new SchemaBuilder(_guid);

            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            builder.SetSchemaName("WallSocketLocation");
            builder.SetDocumentation("Data stroe for socket relates info in a wall");

            FieldBuilder fieldBuilder1 = builder.AddSimpleField("SocketLocation", typeof(XYZ));

            fieldBuilder1.SetSpec(SpecTypeId.Length);

            FieldBuilder fieldBuilder2 = builder.AddSimpleField("SocketNumber", typeof(string));

            Schema schema = builder.Finish();

            // Create an entity (object) for this schema (class)

            Entity ent = new Entity(schema);
            Field socketLocation = schema.GetField("SocketLocation");
            //ent.Set<XYZ>( socketLocation, new XYZ( 2, 0, 0 ), DisplayUnitType.DUT_METERS ); // 2020
            ent.Set<XYZ>(socketLocation, new XYZ(2, 0, 0), UnitTypeId.Meters); // 2021

            Field socketNumber = schema.GetField("SocketNumber");
            ent.Set<string>(socketNumber, "200");

            wall.SetEntity(ent);

            // Now create another entity (object) for this schema (class)

            Entity ent2 = new Entity(schema);
            Field socketNumber1 = schema.GetField("SocketNumber");
            ent2.Set<String>(socketNumber1, "400");
            wall.SetEntity(ent2);

            // Note: this will replace the previous entity on the wall 

            // List all schemas in the document

            string s = string.Empty;
            IList<Schema> schemas = Schema.ListSchemas();

            foreach(Schema sch in schemas)
            {
                s += "\r\nSchema Name: " + sch.SchemaName;
            }
            TaskDialog.Show("Schema details", s);

            //List all fields for our schema

            s = string.Empty;
            Schema ourSchema=Schema.Lookup(_guid);
            IList<Field> fields = ourSchema.ListFields();
            foreach(Field fld in fields)
            {
                s += "\r\nField Name: " + fld.FieldName;
            }
            TaskDialog.Show("Field details", s);

            //Extract the value for the field we created

            Entity wallSchemaEnt = wall.GetEntity(Schema.Lookup(_guid));

            XYZ wallSocketPos = wallSchemaEnt.Get<XYZ>(Schema.Lookup(_guid).GetField("SocketLocation"), UnitTypeId.Meters);

            s = "SocketLocation: " + Format.PointString(wallSocketPos);

            string wallSockerNumber = wallSchemaEnt.Get<String>(Schema.Lookup(_guid).GetField("SocketNumber"));

            s += "\r\nSocketNumber: " + wallSockerNumber;

            TaskDialog.Show("Field value", s);

            trans.Commit();

            return Result.Succeeded;
                
              

        }
    }
}
