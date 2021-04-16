using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Drawing;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace GrimshawRibbon.BoundaryRooms.CS
{
    public partial class roomsInformationForm : System.Windows.Forms.Form
    {
        #region Class member variables
        RoomsData m_data; // Room's data for current active document
        FloorsData floorsData; // Floor's data for current active document

        private ExternalCommandData m_commandData;
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public roomsInformationForm()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Overload the constructor
        /// </summary>
        /// <param name="data">an instance of Data class</param>
        public roomsInformationForm(RoomsData data, ExternalCommandData commandData)
        {
            m_data = data;
            m_commandData = commandData;
            InitializeComponent();
            Initialize();
        }

        public roomsInformationForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            m_commandData = commandData;
            Initialize();
        }


        private void Initialize()
        {
            //add levels to combo box levelsComboBox
            Document document = m_commandData.Application.ActiveUIDocument.Document;
           
            // Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(FloorType));
            

            foreach (Element element in collector)
            {
                FloorType floorType = element as FloorType;
                if (null == floorType)
                {
                    continue;
                }
                //this.roomsAndFloorsGridView["Columns4"]
                //this.roomsAndFloorsGridView.Rows[0].Cells[3].Value = floorType;
                //this.roomsAndFloorsGridView.Columns
                this.floorsComboBox.Items.Add(floorType);
               
            }
            if (this.floorsComboBox.Items.Count > 0 )
            {
                this.floorsComboBox.DisplayMember = "Name";
                this.floorsComboBox.SelectedIndex = 0;
                //this.roomsAndFloorsGridView.Columns[3].Name = floorType.Name;
                //this.roomsAndFloorsGridView.Columns4.DisplayMember = "Name";
                //this.roomsAndFloorsGridView.Columns4.SelectedIndex = 0;

            }
        }
        private void DisplayRooms2(ReadOnlyCollection<Room> roomList)
        {
            //add levels to combo box levelsComboBox
            Document document = m_commandData.Application.ActiveUIDocument.Document;


            // Get a floor type for floor creation
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(FloorType));
            List<ElementId> flooorList = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Floors).ToElementIds()
                .ToList();

            int i = 1;
            // add rooms to the gridview
            foreach (Room tmpRoom in roomList)
            {
                
                // make sure the room has Level, that's it locates at level.
                if (tmpRoom.Document.GetElement(tmpRoom.LevelId) == null)
                {
                    continue;
                }

                // create a list view Item
                ListViewItem tmpItem = new ListViewItem();
                //tmpItem.SubItems.Add((tmpRoom.Document.GetElement(tmpRoom.LevelId) as Level).Name); //display the level
                tmpItem.SubItems.Add(tmpRoom.Number);     //display room number.
                tmpItem.SubItems.Add(tmpRoom.Name);       //display room name.

                roomsAndFloorsGridView.Rows.Add((tmpRoom.Document.GetElement(tmpRoom.LevelId) as Level).Name);
                roomsAndFloorsGridView.Rows[i - 1].Cells[1].Value = tmpRoom.Number;
                roomsAndFloorsGridView.Rows[i - 1].Cells[2].Value = tmpRoom.Name;
                //roomsAndFloorsGridView.Rows[i - 1].Cells[3].Value = flooorList;

                // roomsAndFloorsGridView.Rows[i - 1].Cells[3].Value = ("1", "2");
                //roomsAndFloorsGridView.Rows[i - 1].Cells[3].Value = tmpItem.Text;//how get floor this

                i++;


            }
        }

        /// <summary>
        /// when the form was loaded, display the information of rooms 
        /// </summary>
      
        private void RoomInfoForm_Load(object sender, EventArgs e)
        {
            

            this.DisplayRooms2(m_data.Rooms);

           
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnCreateFloor_Click(object sender, EventArgs e)
        {
          

        }

        private void roomsInformationForm_MouseEnter(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Move window
        /// </summary>
        System.Drawing.Point lastPoint;
        private void roomsInformationForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }

        }

        private void roomsInformationForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new System.Drawing.Point(e.X, e.Y);
        }
    }
}