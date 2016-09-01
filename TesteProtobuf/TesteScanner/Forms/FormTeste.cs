using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TesteScanner.Network;

namespace TesteScanner
{
    public partial class FormTeste : Form
    {
        protected Server m_Server;
        protected Client m_Client;

        public FormTeste()
        {
            InitializeComponent();
        }

        private void buttonStartServer_Click(object sender, EventArgs e)
        {
            m_Server.Start();
        }

        private void FormTeste_Load(object sender, EventArgs e)
        {
            m_Server = new Server(12345);
            m_Server.OnPacketReceived = Protocol.ServerPacketReceiver;
        }

        private void buttonStopServer_Click(object sender, EventArgs e)
        {
            m_Server.Stop();
        }

        private void buttonClientConnect_Click(object sender, EventArgs e)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, 12345);
            m_Client = Client.Conectar(remoteEP);
            m_Client.Start(Protocol.ClientPacketReceiver);
        }

        private void buttonClientSendPing_Click(object sender, EventArgs e)
        {
            m_Client.Send(Protocol.NewPingMsg());
        }
    }
}
