using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TesteStreamScanner.compiler;
using TesteStreamScanner.teste;
using System.Diagnostics;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.forms
{
    public partial class FormTeste : Form
    {
        public FormTeste()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveEntrada();

            Debug.WriteLine("---");

            BCScanner scanner = new BCScanner();
            scanner.SetSource(textEntrada.Text);

            BCParser parser = new BCParser();
            parser.SetSource(scanner);
            parser.Init();
            bool ret = parser.Parse();
            Debug.WriteLine(String.Format("ret = {0}", ret));

            // scanner.Init();
            
            // scanner.GetToken();
            // scanner.GetToken();
            // scanner.GetToken();
            // scanner.GetToken();
        }

        private void FormTeste_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            textEntrada.Text = Properties.Settings.Default.TesteEntrada;
        }

        private void FormTeste_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveEntrada();
        }

        private void SaveEntrada()
        {
            if (textEntrada.Text.Length > 0)
            {
                Properties.Settings.Default.TesteEntrada = textEntrada.Text;
                Properties.Settings.Default.Save();
            }
        }

		private void button1_Click_1(object sender, EventArgs e)
		{
			SaveEntrada();

			TesteCScanner scanner = new TesteCScanner();
			scanner.SetInput(textEntrada.Text);
			bool ret = scanner.ProccessInput();
			Debug.WriteLine("--------------------------");
			foreach (TokenInfo token in scanner.Tokens)
			{
				Debug.WriteLine(String.Format(" Token:{0}", token));
			}
			Debug.WriteLine("--------------------------");
		}

		private void buttonProcessarUmAUm_Click(object sender, EventArgs e)
		{
			SaveEntrada();

			TesteCScanner scanner = new TesteCScanner();
			scanner.SetInput(textEntrada.Text);
			scanner.Reset();
			Debug.WriteLine("--------------------------");
			while (true)
			{
				TokenInfo tok = scanner.GetToken();
				if (tok == null) break;
				Debug.WriteLine(String.Format(" Token:{0}", tok));
			}
			Debug.WriteLine("LastState:    " + scanner.LastState);
			Debug.WriteLine("CurrentState: " + scanner.CurrentState);
			Debug.WriteLine("NextState:    " + scanner.NextState);
			Debug.WriteLine("FinalState:   " + scanner.FinalState);
			Debug.WriteLine("--------------------------");
		}
    }
}
