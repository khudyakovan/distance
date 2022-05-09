
namespace distance
{
    partial class FrmMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.excelFilePath = new System.Windows.Forms.TextBox();
            this.dataSet1 = new System.Data.DataSet();
            this.rawData = new System.Data.DataTable();
            this.audit_id = new System.Data.DataColumn();
            this.audit_last_update = new System.Data.DataColumn();
            this.user_full_name = new System.Data.DataColumn();
            this.point_name = new System.Data.DataColumn();
            this.point_address = new System.Data.DataColumn();
            this.point_longitude = new System.Data.DataColumn();
            this.point_latitude = new System.Data.DataColumn();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.filling_start = new System.Data.DataColumn();
            this.filling_end = new System.Data.DataColumn();
            this.tracking_deviation_max = new System.Data.DataColumn();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Путь к файлу с аудитами";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 102);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(418, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pBar
            // 
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(100, 16);
            this.pBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 17);
            this.statusLabel.Text = "toolStripStatusLabel1";
            this.statusLabel.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(374, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(31, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(249, 57);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Import";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(330, 57);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Export";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // excelFilePath
            // 
            this.excelFilePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::distance.Properties.Settings.Default, "excelFilePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.excelFilePath.Location = new System.Drawing.Point(152, 19);
            this.excelFilePath.Name = "excelFilePath";
            this.excelFilePath.Size = new System.Drawing.Size(216, 20);
            this.excelFilePath.TabIndex = 0;
            this.excelFilePath.Text = global::distance.Properties.Settings.Default.excelFilePath;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.rawData});
            // 
            // rawData
            // 
            this.rawData.Columns.AddRange(new System.Data.DataColumn[] {
            this.audit_id,
            this.audit_last_update,
            this.user_full_name,
            this.point_name,
            this.point_address,
            this.point_longitude,
            this.point_latitude,
            this.filling_start,
            this.filling_end,
            this.tracking_deviation_max});
            this.rawData.TableName = "rawData";
            // 
            // audit_id
            // 
            this.audit_id.ColumnName = "audit_id";
            this.audit_id.DataType = typeof(int);
            // 
            // audit_last_update
            // 
            this.audit_last_update.ColumnName = "audit_last_update";
            this.audit_last_update.DataType = typeof(System.DateTime);
            // 
            // user_full_name
            // 
            this.user_full_name.ColumnName = "user_full_name";
            // 
            // point_name
            // 
            this.point_name.ColumnName = "point_name";
            this.point_name.DataType = typeof(int);
            // 
            // point_address
            // 
            this.point_address.ColumnName = "point_address";
            // 
            // point_longitude
            // 
            this.point_longitude.ColumnName = "point_longitude";
            // 
            // point_latitude
            // 
            this.point_latitude.ColumnName = "point_latitude";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.FileName = global::distance.Properties.Settings.Default.resultFileName;
            this.saveFileDialog1.Filter = "ExcelFile|*.xlsx";
            // 
            // filling_start
            // 
            this.filling_start.Caption = "filling_start";
            this.filling_start.ColumnName = "filling_start";
            this.filling_start.DataType = typeof(System.DateTime);
            // 
            // filling_end
            // 
            this.filling_end.Caption = "filling_end";
            this.filling_end.ColumnName = "filling_end";
            this.filling_end.DataType = typeof(System.DateTime);
            // 
            // tracking_deviation_max
            // 
            this.tracking_deviation_max.Caption = "tracking_deviation_max";
            this.tracking_deviation_max.ColumnName = "tracking_deviation_max";
            this.tracking_deviation_max.DataType = typeof(int);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 124);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.excelFilePath);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::distance.Properties.Settings.Default, "excelFilePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = global::distance.Properties.Settings.Default.excelFilePath;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox excelFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar pBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable rawData;
        private System.Data.DataColumn audit_id;
        private System.Data.DataColumn user_full_name;
        private System.Data.DataColumn point_name;
        private System.Data.DataColumn point_address;
        private System.Data.DataColumn point_longitude;
        private System.Data.DataColumn point_latitude;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Data.DataColumn audit_last_update;
        private System.Data.DataColumn filling_start;
        private System.Data.DataColumn filling_end;
        private System.Data.DataColumn tracking_deviation_max;
    }
}

