namespace TesteTwain
{
    partial class MainForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.selectSource = new System.Windows.Forms.Button();
            this.scan = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.diagnostics = new System.Windows.Forms.Button();
            this.useAdfCheckBox = new System.Windows.Forms.CheckBox();
            this.useUICheckBox = new System.Windows.Forms.CheckBox();
            this.showProgressIndicatorUICheckBox = new System.Windows.Forms.CheckBox();
            this.useDuplexCheckBox = new System.Windows.Forms.CheckBox();
            this.blackAndWhiteCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBoxArea = new System.Windows.Forms.CheckBox();
            this.autoRotateCheckBox = new System.Windows.Forms.CheckBox();
            this.autoDetectBorderCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(475, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(257, 301);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(12, 9);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(58, 13);
            this.widthLabel.TabIndex = 1;
            this.widthLabel.Text = "widthLabel";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(12, 35);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(62, 13);
            this.heightLabel.TabIndex = 2;
            this.heightLabel.Text = "heightLabel";
            // 
            // selectSource
            // 
            this.selectSource.Location = new System.Drawing.Point(12, 93);
            this.selectSource.Name = "selectSource";
            this.selectSource.Size = new System.Drawing.Size(102, 23);
            this.selectSource.TabIndex = 3;
            this.selectSource.Text = "selectSource";
            this.selectSource.UseVisualStyleBackColor = true;
            this.selectSource.Click += new System.EventHandler(this.selectSource_Click);
            // 
            // scan
            // 
            this.scan.Location = new System.Drawing.Point(120, 93);
            this.scan.Name = "scan";
            this.scan.Size = new System.Drawing.Size(102, 23);
            this.scan.TabIndex = 4;
            this.scan.Text = "scan";
            this.scan.UseVisualStyleBackColor = true;
            this.scan.Click += new System.EventHandler(this.scan_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(228, 93);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(102, 23);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // diagnostics
            // 
            this.diagnostics.Location = new System.Drawing.Point(336, 93);
            this.diagnostics.Name = "diagnostics";
            this.diagnostics.Size = new System.Drawing.Size(102, 23);
            this.diagnostics.TabIndex = 6;
            this.diagnostics.Text = "diagnostics";
            this.diagnostics.UseVisualStyleBackColor = true;
            this.diagnostics.Click += new System.EventHandler(this.diagnostics_Click);
            // 
            // useAdfCheckBox
            // 
            this.useAdfCheckBox.AutoSize = true;
            this.useAdfCheckBox.Location = new System.Drawing.Point(15, 149);
            this.useAdfCheckBox.Name = "useAdfCheckBox";
            this.useAdfCheckBox.Size = new System.Drawing.Size(108, 17);
            this.useAdfCheckBox.TabIndex = 7;
            this.useAdfCheckBox.Text = "useAdfCheckBox";
            this.useAdfCheckBox.UseVisualStyleBackColor = true;
            // 
            // useUICheckBox
            // 
            this.useUICheckBox.AutoSize = true;
            this.useUICheckBox.Location = new System.Drawing.Point(15, 172);
            this.useUICheckBox.Name = "useUICheckBox";
            this.useUICheckBox.Size = new System.Drawing.Size(103, 17);
            this.useUICheckBox.TabIndex = 8;
            this.useUICheckBox.Text = "useUICheckBox";
            this.useUICheckBox.UseVisualStyleBackColor = true;
            // 
            // showProgressIndicatorUICheckBox
            // 
            this.showProgressIndicatorUICheckBox.AutoSize = true;
            this.showProgressIndicatorUICheckBox.Location = new System.Drawing.Point(15, 195);
            this.showProgressIndicatorUICheckBox.Name = "showProgressIndicatorUICheckBox";
            this.showProgressIndicatorUICheckBox.Size = new System.Drawing.Size(193, 17);
            this.showProgressIndicatorUICheckBox.TabIndex = 9;
            this.showProgressIndicatorUICheckBox.Text = "showProgressIndicatorUICheckBox";
            this.showProgressIndicatorUICheckBox.UseVisualStyleBackColor = true;
            // 
            // useDuplexCheckBox
            // 
            this.useDuplexCheckBox.AutoSize = true;
            this.useDuplexCheckBox.Location = new System.Drawing.Point(15, 218);
            this.useDuplexCheckBox.Name = "useDuplexCheckBox";
            this.useDuplexCheckBox.Size = new System.Drawing.Size(125, 17);
            this.useDuplexCheckBox.TabIndex = 10;
            this.useDuplexCheckBox.Text = "useDuplexCheckBox";
            this.useDuplexCheckBox.UseVisualStyleBackColor = true;
            // 
            // blackAndWhiteCheckBox
            // 
            this.blackAndWhiteCheckBox.AutoSize = true;
            this.blackAndWhiteCheckBox.Location = new System.Drawing.Point(15, 241);
            this.blackAndWhiteCheckBox.Name = "blackAndWhiteCheckBox";
            this.blackAndWhiteCheckBox.Size = new System.Drawing.Size(148, 17);
            this.blackAndWhiteCheckBox.TabIndex = 11;
            this.blackAndWhiteCheckBox.Text = "blackAndWhiteCheckBox";
            this.blackAndWhiteCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBoxArea
            // 
            this.checkBoxArea.AutoSize = true;
            this.checkBoxArea.Location = new System.Drawing.Point(15, 264);
            this.checkBoxArea.Name = "checkBoxArea";
            this.checkBoxArea.Size = new System.Drawing.Size(96, 17);
            this.checkBoxArea.TabIndex = 12;
            this.checkBoxArea.Text = "checkBoxArea";
            this.checkBoxArea.UseVisualStyleBackColor = true;
            // 
            // autoRotateCheckBox
            // 
            this.autoRotateCheckBox.AutoSize = true;
            this.autoRotateCheckBox.Location = new System.Drawing.Point(15, 287);
            this.autoRotateCheckBox.Name = "autoRotateCheckBox";
            this.autoRotateCheckBox.Size = new System.Drawing.Size(128, 17);
            this.autoRotateCheckBox.TabIndex = 13;
            this.autoRotateCheckBox.Text = "autoRotateCheckBox";
            this.autoRotateCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoDetectBorderCheckBox
            // 
            this.autoDetectBorderCheckBox.AutoSize = true;
            this.autoDetectBorderCheckBox.Location = new System.Drawing.Point(15, 310);
            this.autoDetectBorderCheckBox.Name = "autoDetectBorderCheckBox";
            this.autoDetectBorderCheckBox.Size = new System.Drawing.Size(159, 17);
            this.autoDetectBorderCheckBox.TabIndex = 14;
            this.autoDetectBorderCheckBox.Text = "autoDetectBorderCheckBox";
            this.autoDetectBorderCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 388);
            this.Controls.Add(this.autoDetectBorderCheckBox);
            this.Controls.Add(this.autoRotateCheckBox);
            this.Controls.Add(this.checkBoxArea);
            this.Controls.Add(this.blackAndWhiteCheckBox);
            this.Controls.Add(this.useDuplexCheckBox);
            this.Controls.Add(this.showProgressIndicatorUICheckBox);
            this.Controls.Add(this.useUICheckBox);
            this.Controls.Add(this.useAdfCheckBox);
            this.Controls.Add(this.diagnostics);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.scan);
            this.Controls.Add(this.selectSource);
            this.Controls.Add(this.heightLabel);
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.Button selectSource;
        private System.Windows.Forms.Button scan;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button diagnostics;
        private System.Windows.Forms.CheckBox useAdfCheckBox;
        private System.Windows.Forms.CheckBox useUICheckBox;
        private System.Windows.Forms.CheckBox showProgressIndicatorUICheckBox;
        private System.Windows.Forms.CheckBox useDuplexCheckBox;
        private System.Windows.Forms.CheckBox blackAndWhiteCheckBox;
        private System.Windows.Forms.CheckBox checkBoxArea;
        private System.Windows.Forms.CheckBox autoRotateCheckBox;
        private System.Windows.Forms.CheckBox autoDetectBorderCheckBox;
    }
}

