using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Torrents
{
    public partial class Initialize : Form
    {
        MySettings settings = MySettings.Load("Initialize.jsn");
        public Initialize()
        {
            InitializeComponent();
        }

        private void Initialize_Load(object sender, EventArgs e)
        {
            try
            {
                Ip.Text = settings.IPAddress;
                textBox3.Text = settings.GateWay;
                textBox2.Text = settings.MacAddress;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if(checkBox1.Checked)
            {
                settings.IPAddress = Ip.Text;
                settings.MacAddress = textBox2.Text;
                settings.GateWay = textBox3.Text;
                settings.Save("Initialize.jsn");
            }
            this.Close();


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
    class MySettings : AppSettings<MySettings>
    {
        public string IPAddress =null;
        public string MacAddress = null;
        public string GateWay = null;
    }
}
