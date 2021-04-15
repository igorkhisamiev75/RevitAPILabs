
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
            this.roomsAndFloorsGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.floorsComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.roomsAndFloorsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreateFloor
            // 
            this.btnCreateFloor.Location = new System.Drawing.Point(1519, 51);
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
            this.btnCancel.Location = new System.Drawing.Point(1523, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(178, 73);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // roomsAndFloorsGridView
            // 
            this.roomsAndFloorsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.roomsAndFloorsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.roomsAndFloorsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.roomsAndFloorsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.roomsAndFloorsGridView.Location = new System.Drawing.Point(58, 51);
            this.roomsAndFloorsGridView.Name = "roomsAndFloorsGridView";
            this.roomsAndFloorsGridView.RowHeadersWidth = 72;
            this.roomsAndFloorsGridView.RowTemplate.Height = 31;
            this.roomsAndFloorsGridView.Size = new System.Drawing.Size(1355, 792);
            this.roomsAndFloorsGridView.TabIndex = 6;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column1.HeaderText = "Level";
            this.Column1.MinimumWidth = 12;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Number";
            this.Column2.MinimumWidth = 9;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Name";
            this.Column3.MinimumWidth = 9;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Floors";
            this.Column4.MinimumWidth = 9;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // floorsComboBox
            // 
            this.floorsComboBox.FormattingEnabled = true;
            this.floorsComboBox.Location = new System.Drawing.Point(1471, 431);
            this.floorsComboBox.Name = "floorsComboBox";
            this.floorsComboBox.Size = new System.Drawing.Size(210, 32);
            this.floorsComboBox.TabIndex = 7;
            // 
            // roomsInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1744, 1266);
            this.Controls.Add(this.floorsComboBox);
            this.Controls.Add(this.roomsAndFloorsGridView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateFloor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "roomsInformationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rooms information";
            this.Load += new System.EventHandler(this.RoomInfoForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.roomsInformationForm_MouseDown);
            this.MouseEnter += new System.EventHandler(this.roomsInformationForm_MouseEnter);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.roomsInformationForm_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.roomsAndFloorsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreateFloor;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView roomsAndFloorsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column4;
        private System.Windows.Forms.ComboBox floorsComboBox;
    }
}