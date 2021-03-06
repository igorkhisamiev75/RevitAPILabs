using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GenerateFloorByRoom
{
    /// <summary>
    /// User interface.
    /// </summary>
    public partial class GenerateFloorForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// the data get/set with revit. 
        /// </summary>
        private Data m_data;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="data"></param>
        public GenerateFloorForm(Data data)
        {
            m_data = data;
            InitializeComponent();
        }

        /// <summary>
        /// paint the floor's profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            double maxLength = previewPictureBox.Width > previewPictureBox.Height ? previewPictureBox.Width : previewPictureBox.Height;
            float scale = (float)(maxLength / m_data.MaxLength * 0.8);
            e.Graphics.ScaleTransform(scale, scale);
            e.Graphics.DrawLines(new Pen(System.Drawing.Color.Red, 1), m_data.Points);
        }

        /// <summary>
        /// initialize the data binding with revit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateFloorForm_Load(object sender, EventArgs e)
        {
            floorTypesComboBox.DataSource = m_data.FloorTypesName;
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        /// <summary>
        /// set the floor type to be create.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floorTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        /// <summary>
        /// set if the floor to be create is structural.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void structralCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_data.Structural = structuralCheckBox.Checked;
        }

        private void OK_Click(object sender, EventArgs e)
        {

        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}