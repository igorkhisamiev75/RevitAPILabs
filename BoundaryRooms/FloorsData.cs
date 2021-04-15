#region Namespace
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
#endregion

namespace GrimshawRibbon.BoundaryRooms.CS
{
    class FloorsData
    {
        #region class member variables
        UIApplication m_revit;  // Store the reference of the application in revit
        List<Floor> m_floors = new List<Floor>();    // a list to store all floor in the project

        #endregion

        /// <summary>
        /// a list of all the floors in the project
        /// </summary>
        public ReadOnlyCollection<Floor> Floors
        {
            get
            {
                return new ReadOnlyCollection<Floor>(m_floors);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="commandData"></param>
        public FloorsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;

            // get all the floors in the project
            GetAllFloors();

        }

        private void GetAllFloors()
        {
            // get the active document 
            Document document = m_revit.ActiveUIDocument.Document;
            // List all the floors instances 

            // Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(FloorType));
            //foreach(Element element in collector)
            //{
            //    FloorType floorType = element as FloorType;
            //    if (null == floorType)
            //    {
            //        continue;
            //    }
                
            //}

            //FloorType floorType = collector.FirstElement() as FloorType;



        }
    }
}
