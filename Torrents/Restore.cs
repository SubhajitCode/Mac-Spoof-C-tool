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
    public partial class Restore : Form
    {
        RestoreSettings settings = RestoreSettings.Load("Resore.jsn");
        public Restore()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (checkBox1.Checked)
            {
                settings.IPAddress = Ip.Text;
                settings.GateWay = textBox2.Text;

                settings.Save("Resore.jsn");
            }
            this.Close();
        }

        private void Restore_Load(object sender, EventArgs e)
        {
            try
            {
                Ip.Text = settings.IPAddress;
                textBox2.Text = settings.GateWay;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
    class RestoreSettings : AppSettings<RestoreSettings>
    {
        public string IPAddress = null;
        
        public string GateWay = null;
    }
}
