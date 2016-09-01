namespace TesteStreamScanner.forms
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
			this.buttonProcessar = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textEntrada = new System.Windows.Forms.TextBox();
			this.buttonProcessarTodos = new System.Windows.Forms.Button();
			this.buttonProcessarUmAUm = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonProcessar
			// 
			this.buttonProcessar.Location = new System.Drawing.Point(12, 133);
			this.buttonProcessar.Name = "buttonProcessar";
			this.buttonProcessar.Size = new System.Drawing.Size(75, 23);
			this.buttonProcessar.TabIndex = 0;
			this.buttonProcessar.Text = "Processar";
			this.buttonProcessar.UseVisualStyleBackColor = true;
			this.buttonProcessar.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Entrada:";
			// 
			// textEntrada
			// 
			this.textEntrada.Location = new System.Drawing.Point(12, 25);
			this.textEntrada.Multiline = true;
			this.textEntrada.Name = "textEntrada";
			this.textEntrada.Size = new System.Drawing.Size(686, 102);
			this.textEntrada.TabIndex = 2;
			// 
			// buttonProcessarTodos
			// 
			this.buttonProcessarTodos.Location = new System.Drawing.Point(93, 133);
			this.buttonProcessarTodos.Name = "buttonProcessarTodos";
			this.buttonProcessarTodos.Size = new System.Drawing.Size(75, 44);
			this.buttonProcessarTodos.TabIndex = 3;
			this.buttonProcessarTodos.Text = "Processar todos";
			this.buttonProcessarTodos.UseVisualStyleBackColor = true;
			this.buttonProcessarTodos.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// buttonProcessarUmAUm
			// 
			this.buttonProcessarUmAUm.Location = new System.Drawing.Point(174, 133);
			this.buttonProcessarUmAUm.Name = "buttonProcessarUmAUm";
			this.buttonProcessarUmAUm.Size = new System.Drawing.Size(75, 44);
			this.buttonProcessarUmAUm.TabIndex = 4;
			this.buttonProcessarUmAUm.Text = "Processar um a um";
			this.buttonProcessarUmAUm.UseVisualStyleBackColor = true;
			this.buttonProcessarUmAUm.Click += new System.EventHandler(this.buttonProcessarUmAUm_Click);
			// 
			// FormTeste
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(710, 273);
			this.Controls.Add(this.buttonProcessarUmAUm);
			this.Controls.Add(this.buttonProcessarTodos);
			this.Controls.Add(this.textEntrada);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonProcessar);
			this.Name = "FormTeste";
			this.Text = "FormTeste";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTeste_FormClosed);
			this.Load += new System.EventHandler(this.FormTeste_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonProcessar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textEntrada;
		private System.Windows.Forms.Button buttonProcessarTodos;
		private System.Windows.Forms.Button buttonProcessarUmAUm;
    }
}