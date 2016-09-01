using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Dynamic;
using System.IO;
using System.Diagnostics;
using TesteObjetoDinamico.json;
using TesteObjetoDinamico.json.dom;
using TesteObjetoDinamico.json.scanner;
using TesteJson.Properties;

namespace TesteObjetoDinamico
{
    public partial class FormTeste : Form
    {
        public FormTeste()
        {
            InitializeComponent();
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            JsonScanner scanner = new JsonScanner();
            scanner.SetInput(textJson.Text);

            Debug.WriteLine("---------------------------");

            while (true)
            {
                JsonScannerToken token = scanner.GetToken();
                Debug.WriteLine("Token: {0} (pos {1}, {2})", token, token.NumeroLinha, token.NumeroColuna);
                if (token.Type == eTokenType.Erro)
                {
                    Debug.WriteLine("SCAN Error: {0}", token.Mensagem, null);
                    break;
                }
                if (token.Type == eTokenType.Eof) break;
            }
        }

        private void FormTeste_Load(object sender, EventArgs e)
        {
            textJson.Text = Settings.Default.Teste;
        }

        private void FormTeste_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.Teste = textJson.Text;
            Settings.Default.Save();
        }

        private void textJson_TextChanged(object sender, EventArgs e)
        {
        }

        private JsonScanner GetTokenizer()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(textJson.Text);
            sw.Flush();
            ms.Position = 0;

            JsonScanner tokenizer = new JsonScanner();
            tokenizer.SetInput(new StreamReader(ms));

            return tokenizer;
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            JsonScanner scanner = new JsonScanner();
            scanner.SetInput(textJson.Text);

            JsonParser parser = new JsonParser(scanner);
            parser.ThrowError = false;
            bool ret = parser.ParseObject();

            if (ret)
            {
                Debug.WriteLine("Parse OK");
            }
            else
            {
                Debug.WriteLine("Parse error: MSG={0} TOKEN={1}", parser.ParseErrorMessage, parser.ParseErrorToken);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            List<JsonScannerToken> tokens = GetTokens();
            if (tokens == null) return;
            JsonDomReader reader = new JsonDomReader();
            reader.Tokens = tokens;
            JsonObject result = reader.ReadObject();

            string msg = String.Format("Dados: {0}", result, null);

            Debug.WriteLine(msg);

            Class1 c = new Class1();
            c.Teste();
             * */
        }
    }
}
