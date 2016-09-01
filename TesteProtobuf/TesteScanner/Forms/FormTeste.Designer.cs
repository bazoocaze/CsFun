namespace TesteScanner
{
    partial class FormTeste
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
            this.buttonStartServer = new System.Windows.Forms.Button();
            this.buttonStopServer = new System.Windows.Forms.Button();
            this.buttonClientConnect = new System.Windows.Forms.Button();
            this.buttonClientStop = new System.Windows.Forms.Button();
            this.buttonClientSendPing = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStartServer
            // 
            this.buttonStartServer.Location = new System.Drawing.Point(12, 12);
            this.buttonStartServer.Name = "buttonStartServer";
            this.buttonStartServer.Size = new System.Drawing.Size(75, 23);
            this.buttonStartServer.TabIndex = 0;
            this.buttonStartServer.Text = "start server";
            this.buttonStartServer.UseVisualStyleBackColor = true;
            this.buttonStartServer.Click += new System.EventHandler(this.buttonStartServer_Click);
            // 
            // buttonStopServer
            // 
            this.buttonStopServer.Location = new System.Drawing.Point(93, 12);
            this.buttonStopServer.Name = "buttonStopServer";
            this.buttonStopServer.Size = new System.Drawing.Size(75, 23);
            this.buttonStopServer.TabIndex = 1;
            this.buttonStopServer.Text = "stop server";
            this.buttonStopServer.UseVisualStyleBackColor = true;
            this.buttonStopServer.Click += new System.EventHandler(this.buttonStopServer_Click);
            // 
            // buttonClientConnect
            // 
            this.buttonClientConnect.Location = new System.Drawing.Point(12, 95);
            this.buttonClientConnect.Name = "buttonClientConnect";
            this.buttonClientConnect.Size = new System.Drawing.Size(103, 23);
            this.buttonClientConnect.TabIndex = 2;
            this.buttonClientConnect.Text = "client connect";
            this.buttonClientConnect.UseVisualStyleBackColor = true;
            this.buttonClientConnect.Click += new System.EventHandler(this.buttonClientConnect_Click);
            // 
            // buttonClientStop
            // 
            this.buttonClientStop.Location = new System.Drawing.Point(12, 124);
            this.buttonClientStop.Name = "buttonClientStop";
            this.buttonClientStop.Size = new System.Drawing.Size(103, 23);
            this.buttonClientStop.TabIndex = 3;
            this.buttonClientStop.Text = "client stop";
            this.buttonClientStop.UseVisualStyleBackColor = true;
            // 
            // buttonClientSendPing
            // 
            this.buttonClientSendPing.Location = new System.Drawing.Point(12, 153);
            this.buttonClientSendPing.Name = "buttonClientSendPing";
            this.buttonClientSendPing.Size = new System.Drawing.Size(103, 23);
            this.buttonClientSendPing.TabIndex = 4;
            this.buttonClientSendPing.Text = "client send ping";
            this.buttonClientSendPing.UseVisualStyleBackColor = true;
            this.buttonClientSendPing.Click += new System.EventHandler(this.buttonClientSendPing_Click);
            // 
            // FormTeste
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonClientSendPing);
            this.Controls.Add(this.buttonClientStop);
            this.Controls.Add(this.buttonClientConnect);
            this.Controls.Add(this.buttonStopServer);
            this.Controls.Add(this.buttonStartServer);
            this.Name = "FormTeste";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormTeste_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartServer;
        private System.Windows.Forms.Button buttonStopServer;
        private System.Windows.Forms.Button buttonClientConnect;
        private System.Windows.Forms.Button buttonClientStop;
        private System.Windows.Forms.Button buttonClientSendPing;
    }
}

