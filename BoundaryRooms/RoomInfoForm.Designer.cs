
namespace GrimshawRibbon.BoundaryRooms.CS
{
    partial class roomsInformationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreateFloor = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.roomsListView = new System.Windows.Forms.ListView();
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSetFloor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.wallTypesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnCreateFloor
            // 
            this.btnCreateFloor.Location = new System.Drawing.Point(1283, 51);
            this.btnCreateFloor.Name = "btnCreateFloor";
            this.btnCreateFloor.Size = new System.Drawing.Size(182, 73);
            this.btnCreateFloor.TabIndex = 0;
            this.btnCreateFloor.Text = "Create Floors in Rooms";
            this.btnCreateFloor.UseVisualStyleBackColor = true;
            this.btnCreateFloor.Click += new System.EventHandler(this.btnCreateFloor_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(1498, 51);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(178, 73);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // roomsListView
            // 
            this.roomsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.roomsListView.FullRowSelect = true;
            this.roomsListView.GridLines = true;
            this.roomsListView.HideSelection = false;
            this.roomsListView.Location = new System.Drawing.Point(32, 51);
            this.roomsListView.Name = "roomsListView";
            this.roomsListView.Size = new System.Drawing.Size(1230, 1162);
            this.roomsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.roomsListView.TabIndex = 5;
            this.roomsListView.UseCompatibleStateImageBehavior = false;
            this.roomsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "ID";
            this.columnHeader0.Width = 105;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 213;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Number";
            this.columnHeader2.Width = 158;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Level";
            this.columnHeader3.Width = 193;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Type Floor";
            this.columnHeader4.Width = 198;
            // 
            // btnSetFloor
            // 
            this.btnSetFloor.Location = new System.Drawing.Point(1379, 399);
            this.btnSetFloor.Name = "btnSetFloor";
            this.btnSetFloor.Size = new System.Drawing.Size(182, 78);
            this.btnSetFloor.TabIndex = 3;
            this.btnSetFloor.Text = "Set Floor";
            this.btnSetFloor.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1304, 279);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Floor Type";
            // 
            // wallTypesComboBox
            // 
            this.wallTypesComboBox.FormattingEnabled = true;
            this.wallTypesComboBox.Location = new System.Drawing.Point(1309, 326);
            this.wallTypesComboBox.Name = "wallTypesComboBox";
            this.wallTypesComboBox.Size = new System.Drawing.Size(331, 32);
            this.wallTypesComboBox.TabIndex = 5;
            // 
            // roomsInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1744, 1266);
            this.Controls.Add(this.wallTypesComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSetFloor);
            this.Controls.Add(this.roomsListView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateFloor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "roomsInformationForm";
            this.ShowInTaskbar = false;
            this.Text = "Rooms information";
            this.Load += new System.EventHandler(this.RoomInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateFloor;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView roomsListView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnSetFloor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox wallTypesComboBox;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}