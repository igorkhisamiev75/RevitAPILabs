using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;


namespace GrimshawRibbon.BoundaryRooms.CS
{
    public partial class CreateFloorForm : System.Windows.Forms.Form
    {
        #region Class member variables
        RoomsData m_data; // Room's data for current active document

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public CreateFloorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Overload the constructor
        /// </summary>
        /// <param name="data">an instance of Data class</param>
        public CreateFloorForm(RoomsData data)
        {
            m_data = data;
            InitializeComponent();
        }

        private void DisplayRooms(ReadOnlyCollection<Room> roomList, bool isHaveTag)
        {
            String propertyValue = null;   //value of department
            String departmentName = null;  //department name 
            Double areaValue = 0.0;        //room area

            // add rooms to the listview
            foreach (Room tmpRoom in roomList)
            {
                // make sure the room has Level, that's it locates at level.
                if (tmpRoom.Document.GetElement(tmpRoom.LevelId) == null)
                {
                    continue;
                }

                int idValue = tmpRoom.Id.IntegerValue;
                string roomId = idValue.ToString();

                // create a list view Item
                ListViewItem tmpItem = new ListViewItem(roomId);
                tmpItem.SubItems.Add(tmpRoom.Name);       //display room name.
                tmpItem.SubItems.Add(tmpRoom.Number);     //display room number.
                tmpItem.SubItems.Add((tmpRoom.Document.GetElement(tmpRoom.LevelId) as Level).Name); //display the level

                // get department name from Department property 
                departmentName = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_DEPARTMENT);
                tmpItem.SubItems.Add(departmentName);

                // get property value 
                propertyValue = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_AREA);

                // get the area value
                areaValue = Double.Parse(propertyValue);
                tmpItem.SubItems.Add(propertyValue + " SF");

                // display whether the room with tag or not
                if (isHaveTag)
                {
                    tmpItem.SubItems.Add("Yes");
                }
                else
                {
                    tmpItem.SubItems.Add("No");
                }

                // add the item to the listview
                roomsListView.Items.Add(tmpItem);

                // add the area to the department
                m_data.CalculateDepartmentArea(departmentName, areaValue);
            }
        }

        /// <summary>
        /// when the form was loaded, display the information of rooms 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomInfoForm_Load(object sender, EventArgs e)
        {
            roomsListView.Items.Clear();

            // add rooms in the list roomsWithoutTag to the listview
            this.DisplayRooms(m_data.RoomsWithoutTag, false);

            // add rooms in the list roomsWithTag to the listview
            this.DisplayRooms(m_data.RoomsWithTag, true);

            // display the total area of each department
            this.DisplayDartmentsInfo();

            // if all the rooms have tags ,the button will be set to disable
            //if (0 == m_data.RoomsWithoutTag.Count)
            //{
            //    addTagsButton.Enabled = false;
            //}
        }

        private void DisplayDartmentsInfo()
        {
            for (int i = 0; i < m_data.DepartmentInfos.Count; i++)
            {
                // create a listview item
                ListViewItem tmpItem = new ListViewItem(m_data.DepartmentInfos[i].DepartmentName);
                tmpItem.SubItems.Add(m_data.DepartmentInfos[i].RoomsAmount.ToString());
                tmpItem.SubItems.Add(m_data.DepartmentInfos[i].DepartmentAreaValue.ToString() +
                                     " SF");
                //departmentsListView.Items.Add(tmpItem);
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
