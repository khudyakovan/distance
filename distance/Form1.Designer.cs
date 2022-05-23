
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.auditFilePath = new System.Windows.Forms.TextBox();
            this.dataSet1 = new System.Data.DataSet();
            this.rawData = new System.Data.DataTable();
            this.audit_id = new System.Data.DataColumn();
            this.audit_last_update = new System.Data.DataColumn();
            this.user_full_name = new System.Data.DataColumn();
            this.point_name = new System.Data.DataColumn();
            this.point_address = new System.Data.DataColumn();
            this.point_longitude = new System.Data.DataColumn();
            this.point_latitude = new System.Data.DataColumn();
            this.filling_start = new System.Data.DataColumn();
            this.filling_end = new System.Data.DataColumn();
            this.tracking_deviation_max = new System.Data.DataColumn();
            this.mappingsDT = new System.Data.DataTable();
            this.shop_id = new System.Data.DataColumn();
            this.office_longitude = new System.Data.DataColumn();
            this.office_latitude = new System.Data.DataColumn();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnLogs = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.mappingsFilePath = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsDT)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Путь к файлу с аудитами";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 183);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(693, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pBar
            // 
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(150, 24);
            this.pBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(179, 25);
            this.statusLabel.Text = "toolStripStatusLabel1";
            this.statusLabel.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(622, 26);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(46, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(309, 131);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(112, 35);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(430, 131);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(112, 35);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // auditFilePath
            // 
            this.auditFilePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::distance.Properties.Settings.Default, "excelFilePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.auditFilePath.Location = new System.Drawing.Point(289, 29);
            this.auditFilePath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.auditFilePath.Name = "auditFilePath";
            this.auditFilePath.Size = new System.Drawing.Size(322, 26);
            this.auditFilePath.TabIndex = 0;
            this.auditFilePath.Text = global::distance.Properties.Settings.Default.excelFilePath;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.rawData,
            this.mappingsDT});
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
            // mappingsDT
            // 
            this.mappingsDT.Columns.AddRange(new System.Data.DataColumn[] {
            this.shop_id,
            this.office_longitude,
            this.office_latitude});
            this.mappingsDT.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("ShopId", new string[] {
                        "shop_id"}, false)});
            this.mappingsDT.TableName = "mappings";
            // 
            // shop_id
            // 
            this.shop_id.Caption = "shop_id";
            this.shop_id.ColumnName = "shop_id";
            this.shop_id.DataType = typeof(int);
            // 
            // office_longitude
            // 
            this.office_longitude.Caption = "office_longitude";
            this.office_longitude.ColumnName = "office_longitude";
            // 
            // office_latitude
            // 
            this.office_latitude.Caption = "office_latitude";
            this.office_latitude.ColumnName = "office_latitude";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.FileName = global::distance.Properties.Settings.Default.resultFileName;
            this.saveFileDialog1.Filter = "ExcelFile|*.xlsx";
            // 
            // btnLogs
            // 
            this.btnLogs.Enabled = false;
            this.btnLogs.Location = new System.Drawing.Point(553, 129);
            this.btnLogs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogs.Name = "btnLogs";
            this.btnLogs.Size = new System.Drawing.Size(112, 35);
            this.btnLogs.TabIndex = 6;
            this.btnLogs.Text = "Log Files";
            this.btnLogs.UseVisualStyleBackColor = true;
            this.btnLogs.Click += new System.EventHandler(this.btnLogs_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(623, 73);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(46, 35);
            this.button5.TabIndex = 9;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 81);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(264, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Путь к файлу с сопоставлениями";
            // 
            // mappingsFilePath
            // 
            this.mappingsFilePath.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mappingsFilePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::distance.Properties.Settings.Default, "mapExcelFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.mappingsFilePath.Location = new System.Drawing.Point(290, 76);
            this.mappingsFilePath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mappingsFilePath.Name = "mappingsFilePath";
            this.mappingsFilePath.Size = new System.Drawing.Size(322, 26);
            this.mappingsFilePath.TabIndex = 7;
            this.mappingsFilePath.Text = global::distance.Properties.Settings.Default.mapExcelFile;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(693, 205);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mappingsFilePath);
            this.Controls.Add(this.btnLogs);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.auditFilePath);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::distance.Properties.Settings.Default, "excelFilePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = global::distance.Properties.Settings.Default.excelFilePath;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsDT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox auditFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar pBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
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
        private System.Windows.Forms.Button btnLogs;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mappingsFilePath;
        private System.Data.DataTable mappingsDT;
        private System.Data.DataColumn shop_id;
        private System.Data.DataColumn office_longitude;
        private System.Data.DataColumn office_latitude;
    }
}

