using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
namespace Torrents
{
    public partial class Torrents : Form
    {
        private const string baseReg =@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}\\";
        private volatile bool _shouldstop;
        private volatile bool _finished;
        private volatile bool _close=false;
        private static string id;
        public delegate void delupdateUi(ListViewItem item);
        public delegate void delupdateStatus();
        static string[] MAC;
        static string Macstring;
        public void updateButton()
        {
            if(_finished)
            {
                RequesStop();
            }
        }
        public void updateUi(ListViewItem item)
        {
            ListView1.Items.Add(item);
            this.Refresh();
        }
        
        
        public Torrents()
        {
            InitializeComponent();
            ListAdapter();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text=="Scan")
            {
                _finished = false;
                button1.Text = "Stop";
                ListView1.Items.Clear();
                _shouldstop = false;
                Thread Scanthread = new Thread(scan1);
                Scanthread.Start();
               
            
            }
            else if(button1.Text=="Stop")
            {
                button1.Text = "Scan";
                
                RequesStop();
            }
           
        }
        public string GetHostName(IPAddress pAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(pAddress);
                return entry.HostName;
            }
            catch(Exception)
            {
                return pAddress.ToString();
            }
        }
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
        private static string GetMacUsingARP(string IPAddr)
        {
            IPAddress IP = IPAddress.Parse(IPAddr);
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;

            if (SendARP((int)IP.Address, 0, macAddr, ref macAddrLen) != 0)
            throw new Exception("ARP command failed");

            string[] str = new string[(int)macAddrLen];
            MAC = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
            {
                str[i] = macAddr[i].ToString("x2");
                MAC[i] = str[i].ToUpper();
            }

            Macstring = string.Concat(MAC);
            
            return Macstring;
            

        }
        public void ListAdapter()
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach(ManagementObject objMO in objMOC)
            {
                
                AdapterList.Items.Add(objMO["Caption"]);

            }
             
        }
        public void GetIp()
        {
            try
            {
              
                ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection objMOC = objMC.GetInstances();
                foreach (ManagementObject objMO in objMOC)
                {
              
                    if (objMO["Caption"].Equals(AdapterList.SelectedItem))
                    {
                        string[] ipaddresses = (string[])objMO["IPAddress"];
                        textBox1.Text = ipaddresses[0];
                        string[] gateways = (string[])objMO["DefaultIPGateway"];
                        textBox2.Text = gateways[0];
                        textBox6.Text = objMO["MACAddress"].ToString();

                        string StartIP = textBox2.Text;
                        int l = StartIP.LastIndexOf(".", StartIP.Length);
                        string startip = StartIP.Substring(0, l);
                        string EndIP = startip + "." + "255";
                        textBox3.Text = EndIP;
                        id=objMO["Index"].ToString();
                        
                    }

                }
     
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not Connected Or Plugged in Check for Connection");
            }
        }
        public void scan1()
        {
            string StartIP = textBox2.Text;
            int l = StartIP.LastIndexOf(".", StartIP.Length);
            string startip = StartIP.Substring(0, l);
            string EndIP = startip + "." + "255";
     foo:       while (!_shouldstop)
            {


                try
                {

                    IPAddress ip = IPAddress.Parse(StartIP);
                    IPAddress ip2 = IPAddress.Parse(EndIP);
                    IPAddress ipScan = null;
                    string hostname = null;
                    int[] aa = new int[4], bb = new int[4];

                    //Get first, second, … byte of address
                    aa[0] = Convert.ToInt32(ip.GetAddressBytes()[0]);
                    aa[1] = Convert.ToInt32(ip.GetAddressBytes()[1]);
                    aa[2] = Convert.ToInt32(ip.GetAddressBytes()[2]);
                    aa[3] = Convert.ToInt32(ip.GetAddressBytes()[3]);
                    bb[0] = Convert.ToInt32(ip2.GetAddressBytes()[0]);
                    bb[1] = Convert.ToInt32(ip2.GetAddressBytes()[1]);
                    bb[2] = Convert.ToInt32(ip2.GetAddressBytes()[2]);
                    bb[3] = Convert.ToInt32(ip2.GetAddressBytes()[3]);
                    for (int a = aa[0]; a <= bb[0]; a++)
                    {


                        for (int b = aa[1]; b <= bb[1]; b++)
                        {
                            for (int c = aa[2]; c <= bb[2]; c++)
                            {
                                for (int d = aa[3]; d <= bb[3]; d++)
                                {
                                    if (!_shouldstop)
                                    {

                                        ipScan = IPAddress.Parse(a.ToString() + "." + b.ToString() + "." + c.ToString() + "." + d.ToString());
                                        Ping pingSender = new Ping();
                                        PingOptions options = new PingOptions();
                                        options.DontFragment = true;
                                        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                                        byte[] buffer = Encoding.ASCII.GetBytes(data);
                                        int timeout = 150;
                                        PingReply reply = pingSender.Send(ipScan, timeout, buffer, options);
                                        hostname = null;
                                        string macaddr = null;
                                        if (reply.Status == IPStatus.Success)
                                        {
                                            //hostname = GetHostName(ipScan);
                                            macaddr = GetMacUsingARP(ipScan.ToString());
                                            ListViewItem itm = new ListViewItem(ipScan.ToString());
                                            //itm.SubItems.Add(hostname);
                                            itm.SubItems.Add(macaddr);
                                            delupdateUi updatelist = new delupdateUi(updateUi);
                                            this.ListView1.Invoke(updatelist, itm);
                                            

                                            //ListView1.Items.Add(itm);


                                        }
                                    }
                                    else
                                        goto foo;
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                RequesStop();
                _finished = true;
                delupdateStatus delupdatebutton = new delupdateStatus(updateButton);
                this.button1.Invoke(delupdatebutton);
                
            }
                
            
        }
        public void RequesStop()
        {
            _shouldstop = true;
            
        }
        
        
        


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AdapterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
            GetIp();
            
        }

       

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void ListView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            foreach (ListViewItem listviewitem in ListView1.SelectedItems)
            {
                textBox3.Text = listviewitem.SubItems[0].Text;
                textBox5.Text = listviewitem.SubItems[1].Text;
                textBox4.Text = textBox2.Text;
            }
        }
        void setIp(string ip_address, string subnet_mask, string gateway)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances(); 
            foreach (ManagementObject objMO in objMOC)
            {
                
                    if (objMO["Caption"].Equals(AdapterList.SelectedItem))
                    {
                        try
                        {
                            ManagementBaseObject setGateway;
                            ManagementBaseObject newGateway = objMO.GetMethodParameters("SetGateways");
                            ManagementBaseObject setIP;
                            ManagementBaseObject newIP =
                                objMO.GetMethodParameters("EnableStatic");

                            newIP["IPAddress"] = new string[] { ip_address };
                            newIP["SubnetMask"] = new string[] { subnet_mask };

                            setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
                            newGateway["DefaultIPGateway"] = new string[] { gateway };
                            newGateway["GatewayCostMetric"] = new int[] { 1 };

                            setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);

                        }
                        catch (Exception)
                        {
                            throw;
                        }  
                    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RequesStop();
            button1.Text = "Scan";
            setIp(textBox3.Text, "255.255.255.0", textBox4.Text);
            SetMAC(textBox5.Text);
        }
        public static bool SetMAC(string Mac)
        {
            bool ret = false;
            //using (RegistryKey bkey = Registry.LocalMachine.OpenSubKey())
            RegistryKey key = Registry.LocalMachine.OpenSubKey(baseReg + "000" + id,true);
    
        
        if (key != null)
        {
            key.SetValue("NetworkAddress", Mac, RegistryValueKind.String);
            

            ManagementObjectSearcher mos = new ManagementObjectSearcher(
                new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " +id));

            foreach (ManagementObject o in mos.Get().OfType<ManagementObject>())
            {
                o.InvokeMethod("Disable", null);
                o.InvokeMethod("Enable", null);
                ret = true;
            }
        }

    
            return ret;
            

            
            

         
        }

        private void initializeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void restoreSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Initialize form2 = new Initialize();
            form2.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Restore form3 = new Restore();
            form3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            MySettings settings = MySettings.Load("Initialize.jsn");
            string Ip = settings.IPAddress;
            string Gateway = settings.GateWay;
            string Mac = settings.MacAddress;
            RequesStop();
            button1.Text = "Scan";
            setIp(Ip, "255.255.255.0", Gateway);
            SetMAC(Mac);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RestoreSettings settings = RestoreSettings.Load("Resore.jsn");
            string Ip = settings.IPAddress;
            string Gateway = settings.GateWay;
            setIp(Ip, "255.255.255.0", Gateway);
            resetMac();
        }
        private static void resetMac()
        {
            System.Diagnostics.Process.Start("\\macreset.bat");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdapterList.Items.Clear();
            ListAdapter();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void Torrents_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = true;
            this.Hide();
        }

        private void Torrents_FormClosing_1(object sender, FormClosingEventArgs e)
        {

            if (_close==false)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
                
            }

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _close = true;
            Application.Exit();
        }

        private void backToNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

       
    }
}
