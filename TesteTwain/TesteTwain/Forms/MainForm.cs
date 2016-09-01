using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwainDotNet;
using TwainDotNet.TwainNative;
using TwainDotNet.WinFroms;

namespace TesteTwain
{
    public partial class MainForm : Form
    {
        private static AreaSettings AreaSettings = new AreaSettings(Units.Centimeters, 0.1f, 5.7f, 0.1F + 2.6f, 5.7f + 2.6f);

        MyTwain m_Twain;
        ScanSettings m_Settings;

        public MainForm()
        {
            InitializeComponent();

            m_Twain = new MyTwain(new WinFormsWindowMessageHook(this));

            m_Twain.TransferImage += delegate (Object sender, TransferImageEventArgs args)
            {
                if (args.Image != null)
                {
                    pictureBox1.Image = args.Image;

                    widthLabel.Text = "Width: " + pictureBox1.Image.Width;
                    heightLabel.Text = "Height: " + pictureBox1.Image.Height;
                }
            };

            m_Twain.ScanningComplete += delegate
            {
                Enabled = true;
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void selectSource_Click(object sender, EventArgs e)
        {
            m_Twain.SelectSource();
        }

        private void scan_Click(object sender, EventArgs e)
        {
            Enabled = false;

            m_Settings = new ScanSettings();
            m_Settings.UseDocumentFeeder = useAdfCheckBox.Checked;
            m_Settings.ShowTwainUI = useUICheckBox.Checked;
            m_Settings.ShowProgressIndicatorUI = showProgressIndicatorUICheckBox.Checked;
            m_Settings.UseDuplex = useDuplexCheckBox.Checked;
            m_Settings.Resolution =
                blackAndWhiteCheckBox.Checked
                ? ResolutionSettings.Fax : ResolutionSettings.ColourPhotocopier;
            m_Settings.Area = !checkBoxArea.Checked ? null : AreaSettings;
            m_Settings.ShouldTransferAllPages = true;

            m_Settings.Resolution.Dpi = 50;

            m_Settings.Rotation = new RotationSettings()
            {
                AutomaticRotate = autoRotateCheckBox.Checked,
                AutomaticBorderDetection = autoDetectBorderCheckBox.Checked
            };

            try
            {
                var scanning = m_Twain.StartScanning(m_Settings);
                if (!scanning) Enabled = true;
            }
            catch (TwainException ex)
            {
                MessageBox.Show(ex.Message);
                Enabled = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(sfd.FileName);
                }
            }
        }

        private void diagnostics_Click(object sender, EventArgs e)
        {
            var diagnostics = new Diagnostics(new WinFormsWindowMessageHook(this));
        }
    }
}
