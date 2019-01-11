namespace GR_ExcelFunc.View
{
    partial class GetObjectParameterControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SiteListCB = new System.Windows.Forms.ComboBox();
            this.searchBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.EntityGridView = new System.Windows.Forms.DataGridView();
            this.ok = new System.Windows.Forms.Button();
            this.EntityTypeCB = new System.Windows.Forms.ComboBox();
            this.PropertyTypeCB = new System.Windows.Forms.ComboBox();
            this.PeriodTypeCB = new System.Windows.Forms.ComboBox();
            this.startTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.endTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.startTime = new System.Windows.Forms.DateTimePicker();
            this.endTime = new System.Windows.Forms.DateTimePicker();
            this.Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.EntityGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SiteListCB
            // 
            this.SiteListCB.FormattingEnabled = true;
            this.SiteListCB.Location = new System.Drawing.Point(201, 59);
            this.SiteListCB.Name = "SiteListCB";
            this.SiteListCB.Size = new System.Drawing.Size(180, 21);
            this.SiteListCB.TabIndex = 0;
            // 
            // searchBox1
            // 
            this.searchBox1.Location = new System.Drawing.Point(15, 11);
            this.searchBox1.Name = "searchBox1";
            this.searchBox1.Size = new System.Drawing.Size(398, 20);
            this.searchBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Image = global::GR_ExcelFunc.Properties.Resources.search;
            this.button1.Location = new System.Drawing.Point(419, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 26);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // EntityGridView
            // 
            this.EntityGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EntityGridView.Location = new System.Drawing.Point(15, 86);
            this.EntityGridView.Name = "EntityGridView";
            this.EntityGridView.Size = new System.Drawing.Size(512, 265);
            this.EntityGridView.TabIndex = 3;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(349, 455);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(64, 23);
            this.ok.TabIndex = 4;
            this.ok.Text = "Выбрать";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.OnOkClick);
            // 
            // EntityTypeCB
            // 
            this.EntityTypeCB.FormattingEnabled = true;
            this.EntityTypeCB.Location = new System.Drawing.Point(15, 59);
            this.EntityTypeCB.Name = "EntityTypeCB";
            this.EntityTypeCB.Size = new System.Drawing.Size(180, 21);
            this.EntityTypeCB.TabIndex = 5;
            // 
            // PropertyTypeCB
            // 
            this.PropertyTypeCB.FormattingEnabled = true;
            this.PropertyTypeCB.Location = new System.Drawing.Point(15, 367);
            this.PropertyTypeCB.Name = "PropertyTypeCB";
            this.PropertyTypeCB.Size = new System.Drawing.Size(180, 21);
            this.PropertyTypeCB.TabIndex = 6;
            // 
            // PeriodTypeCB
            // 
            this.PeriodTypeCB.FormattingEnabled = true;
            this.PeriodTypeCB.Location = new System.Drawing.Point(221, 367);
            this.PeriodTypeCB.Name = "PeriodTypeCB";
            this.PeriodTypeCB.Size = new System.Drawing.Size(200, 21);
            this.PeriodTypeCB.TabIndex = 7;
            // 
            // startTimePicker1
            // 
            this.startTimePicker1.Location = new System.Drawing.Point(15, 414);
            this.startTimePicker1.Name = "startTimePicker1";
            this.startTimePicker1.Size = new System.Drawing.Size(111, 20);
            this.startTimePicker1.TabIndex = 8;
            // 
            // endTimePicker1
            // 
            this.endTimePicker1.Location = new System.Drawing.Point(221, 414);
            this.endTimePicker1.Name = "endTimePicker1";
            this.endTimePicker1.Size = new System.Drawing.Size(138, 20);
            this.endTimePicker1.TabIndex = 9;
            // 
            // startTime
            // 
            this.startTime.Location = new System.Drawing.Point(133, 414);
            this.startTime.Name = "startTime";
            this.startTime.Size = new System.Drawing.Size(62, 20);
            this.startTime.TabIndex = 10;
            // 
            // endTime
            // 
            this.endTime.Location = new System.Drawing.Point(365, 414);
            this.endTime.Name = "endTime";
            this.endTime.Size = new System.Drawing.Size(56, 20);
            this.endTime.TabIndex = 11;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(419, 455);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(64, 23);
            this.Cancel.TabIndex = 12;
            this.Cancel.Text = "Отмена";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // GetObjectParameterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.endTime);
            this.Controls.Add(this.startTime);
            this.Controls.Add(this.endTimePicker1);
            this.Controls.Add(this.startTimePicker1);
            this.Controls.Add(this.PeriodTypeCB);
            this.Controls.Add(this.PropertyTypeCB);
            this.Controls.Add(this.EntityTypeCB);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.EntityGridView);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.searchBox1);
            this.Controls.Add(this.SiteListCB);
            this.Name = "GetObjectParameterControl";
            this.Size = new System.Drawing.Size(545, 498);
            ((System.ComponentModel.ISupportInitialize)(this.EntityGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SiteListCB;
        private System.Windows.Forms.TextBox searchBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView EntityGridView;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.ComboBox EntityTypeCB;
        private System.Windows.Forms.ComboBox PropertyTypeCB;
        private System.Windows.Forms.ComboBox PeriodTypeCB;
        private System.Windows.Forms.DateTimePicker startTimePicker1;
        private System.Windows.Forms.DateTimePicker endTimePicker1;
        private System.Windows.Forms.DateTimePicker startTime;
        private System.Windows.Forms.DateTimePicker endTime;
        private System.Windows.Forms.Button Cancel;
    }
}
