using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace EVGAMCUTOOL
{
    public partial class Form1 : Form
    {
		string EVGAFlash = @"C:\Program Files\EVGA\Precision X1\Library\FirmwareUpdate.exe";
		string PX1Version = "";
		string[] Firmwares;
		string SelectedFirmware = "";

		public Form1()
        {
            InitializeComponent();
		}


		private void SetFlashlocation()
		{
			MessageBox.Show("FirmwareUpdate.exe Not found!" + Environment.NewLine + "Please select this folder." + Environment.NewLine + @"Its normally in C:\Program Files\EVGA\Precision X1\Library\", "Select FirmwareUpdate.exe");
			OpenFileDialog openFileDialog1 = new OpenFileDialog
			{
				DefaultExt = "exe",
				Filter = "FirmwareUpdate.exe|FirmwareUpdate.exe",
				RestoreDirectory = true,
			};

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EVGAFlash = openFileDialog1.FileName;
            }
			else
            {
				this.Close();
            }
		}


        private void Form1_Load(object sender, EventArgs e)
        {
			try
			{
				if (!File.Exists(EVGAFlash))
				{
					SetFlashlocation();
				}
			}
			catch
            {
				SetFlashlocation();
			}
			foreach (string file in Directory.GetDirectories("MCUFirmware"))
			{
				comboBox2.Items.Add(file.Replace(@"MCUFirmware\", ""));
			}
		}

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
			comboBox1.Items.Clear();
			PX1Version = comboBox2.Text;
			if (PX1Version != "")
            {
				comboBox1.Enabled = true;
				Firmwares = File.ReadAllLines(@"MCUFirmware\" + PX1Version + @"\info.txt");

				foreach (string file in Firmwares)
				{
					comboBox1.Items.Add(file.Split(',')[0]);
				}
				comboBox1.SelectedIndex = 0;
			}
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			int comboindex = comboBox1.SelectedIndex;
			string[] firmwareinfo = Firmwares[comboindex].Replace(" ", "").Split(',');
			label4.Text = "ProductID: " + ProductID(firmwareinfo[1]);
			label5.Text = "Version: " + firmwareinfo[2] + "." + firmwareinfo[3];
			SelectedFirmware = Directory.GetCurrentDirectory() + @"\MCUFirmware\" + PX1Version + @"\" + comboBox1.Text;
			button1.Enabled = true;
		}

		public string ProductID (string id)
        {
			switch (id)
            {
				case "1":
					return "E221_XC3";
				case "2":
					return "E227_FTW3";
				case "3":
					return "KINGPIN";
				case "4":
					return "E230_FTW3";
				case "5":
					return "E229_3Fans";
				case "6":
					return "XC3 HYBRID";
				case "7":
					return "XC3 HC";
				case "8":
					return "FTW3 HYBRID";
				case "9":
					return "FTW3 HC";
				case "10":
					return "KINGPIN HC";
				default:
					return id;
			}
        }

        private void button1_Click(object sender, EventArgs e)
        {
			if (SelectedFirmware != "")
			this.UpdateFirmware(uint.Parse(maskedTextBox1.Text), SelectedFirmware, 0u, 0);
		}

		private void UpdateFirmware(uint gpuIndex, string fullname, uint address = 0u, byte version = 0)
		{
			string text = "--gpu " + gpuIndex.ToString() + " --fullname " + fullname +  " --return " + @"C:\Program Files\EVGA\Precision X1\PrecisionX_x64.exe";
			if (address != 0u && version != 0)
			{
				text += string.Format(" --address 0x{0:X8} --version {1}", address, version);
			}
			string fileName = EVGAFlash;
			try
			{
				Process.Start(fileName, text);
			}
			catch (Exception)
			{
			}
		}

	}
}
