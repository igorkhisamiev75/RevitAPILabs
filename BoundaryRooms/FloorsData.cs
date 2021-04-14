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


    }
}
