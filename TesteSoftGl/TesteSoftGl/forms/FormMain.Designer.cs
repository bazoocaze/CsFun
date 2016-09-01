namespace TesteSoftGl.forms
{
    partial class FormMain
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
            this.panelCanvas = new System.Windows.Forms.Panel();
            this.buttonOneFrame = new System.Windows.Forms.Button();
            this.buttonSwap = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.checkEnableFps = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioGContext2 = new System.Windows.Forms.RadioButton();
            this.radioGContext1 = new System.Windows.Forms.RadioButton();
            this.buttonTeste = new System.Windows.Forms.Button();
            this.checkRotate = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCanvas
            // 
            this.panelCanvas.BackColor = System.Drawing.Color.Gray;
            this.panelCanvas.Location = new System.Drawing.Point(15, 183);
            this.panelCanvas.Name = "panelCanvas";
            this.panelCanvas.Size = new System.Drawing.Size(839, 282);
            this.panelCanvas.TabIndex = 0;
            // 
            // buttonOneFrame
            // 
            this.buttonOneFrame.Location = new System.Drawing.Point(215, 98);
            this.buttonOneFrame.Name = "buttonOneFrame";
            this.buttonOneFrame.Size = new System.Drawing.Size(75, 23);
            this.buttonOneFrame.TabIndex = 1;
            this.buttonOneFrame.Text = "1 frame";
            this.buttonOneFrame.UseVisualStyleBackColor = true;
            this.buttonOneFrame.Click += new System.EventHandler(this.buttonOneFrame_Click);
            // 
            // buttonSwap
            // 
            this.buttonSwap.Location = new System.Drawing.Point(296, 98);
            this.buttonSwap.Name = "buttonSwap";
            this.buttonSwap.Size = new System.Drawing.Size(75, 23);
            this.buttonSwap.TabIndex = 2;
            this.buttonSwap.Text = "swap buffers";
            this.buttonSwap.UseVisualStyleBackColor = true;
            this.buttonSwap.Click += new System.EventHandler(this.buttonSwap_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 36);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(93, 36);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(47, 13);
            this.labelInfo.TabIndex = 5;
            this.labelInfo.Text = "labelInfo";
            // 
            // checkEnableFps
            // 
            this.checkEnableFps.AutoSize = true;
            this.checkEnableFps.Checked = true;
            this.checkEnableFps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEnableFps.Location = new System.Drawing.Point(183, 40);
            this.checkEnableFps.Name = "checkEnableFps";
            this.checkEnableFps.Size = new System.Drawing.Size(75, 17);
            this.checkEnableFps.TabIndex = 7;
            this.checkEnableFps.Text = "fps control";
            this.checkEnableFps.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioGContext2);
            this.groupBox1.Controls.Add(this.radioGContext1);
            this.groupBox1.Location = new System.Drawing.Point(15, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 85);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Render";
            // 
            // radioGContext2
            // 
            this.radioGContext2.AutoSize = true;
            this.radioGContext2.Location = new System.Drawing.Point(15, 47);
            this.radioGContext2.Name = "radioGContext2";
            this.radioGContext2.Size = new System.Drawing.Size(75, 17);
            this.radioGContext2.TabIndex = 11;
            this.radioGContext2.TabStop = true;
            this.radioGContext2.Text = "GContext2";
            this.radioGContext2.UseVisualStyleBackColor = true;
            // 
            // radioGContext1
            // 
            this.radioGContext1.AutoSize = true;
            this.radioGContext1.Checked = true;
            this.radioGContext1.Location = new System.Drawing.Point(15, 23);
            this.radioGContext1.Name = "radioGContext1";
            this.radioGContext1.Size = new System.Drawing.Size(75, 17);
            this.radioGContext1.TabIndex = 10;
            this.radioGContext1.TabStop = true;
            this.radioGContext1.Text = "GContext1";
            this.radioGContext1.UseVisualStyleBackColor = true;
            // 
            // buttonTeste
            // 
            this.buttonTeste.Location = new System.Drawing.Point(215, 127);
            this.buttonTeste.Name = "buttonTeste";
            this.buttonTeste.Size = new System.Drawing.Size(75, 23);
            this.buttonTeste.TabIndex = 11;
            this.buttonTeste.Text = "teste";
            this.buttonTeste.UseVisualStyleBackColor = true;
            this.buttonTeste.Click += new System.EventHandler(this.buttonTeste_Click);
            // 
            // checkRotate
            // 
            this.checkRotate.AutoSize = true;
            this.checkRotate.Checked = true;
            this.checkRotate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkRotate.Location = new System.Drawing.Point(264, 40);
            this.checkRotate.Name = "checkRotate";
            this.checkRotate.Size = new System.Drawing.Size(53, 17);
            this.checkRotate.TabIndex = 12;
            this.checkRotate.Text = "rotate";
            this.checkRotate.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 477);
            this.Controls.Add(this.checkRotate);
            this.Controls.Add(this.buttonTeste);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkEnableFps);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonSwap);
            this.Controls.Add(this.buttonOneFrame);
            this.Controls.Add(this.panelCanvas);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelCanvas;
        private System.Windows.Forms.Button buttonOneFrame;
        private System.Windows.Forms.Button buttonSwap;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.CheckBox checkEnableFps;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioGContext2;
        private System.Windows.Forms.RadioButton radioGContext1;
        private System.Windows.Forms.Button buttonTeste;
        private System.Windows.Forms.CheckBox checkRotate;
    }
}