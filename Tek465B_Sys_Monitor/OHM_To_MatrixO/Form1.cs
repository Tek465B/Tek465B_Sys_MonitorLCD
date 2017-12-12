using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tek465B_Sys_Monitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts2();
        }
        Dictionary<string, PerformanceCounter> classesF1 = new Dictionary<string, PerformanceCounter>();
        List<string> textLCD = new List<string>();
        Dictionary<string, SavedCounter> SavedCounters = new Dictionary<string, SavedCounter>();
        byte bufind = 0;
        int count = 0;
        int nbrRepeat = 4;
        byte scrollTXT = 2;

        void getAvailablePorts2()
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            button2.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Tek465B\nemail: gamegunsZ28@gmail.com", "About");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*if (MessageBox.Show("Are You Sure to quit?", "Close", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }*/
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            Properties.Settings.Default.Port = comboBox1.SelectedIndex;
            Properties.Settings.Default.Baud = comboBox2.SelectedIndex;
            Properties.Settings.Default.nbrRepeat = domainUpDown2.SelectedIndex;
            Properties.Settings.Default.rptDelay = domainUpDown1.SelectedIndex;
            Properties.Settings.Default.scrolling = checkBox5.Checked;
            Properties.Settings.Default.AutoCnct = checkBox6.Checked;

            Properties.Settings.Default.L1.Clear();
            foreach(string lcdsave in textLCD)
                Properties.Settings.Default.L1.Add(lcdsave);

            Properties.Settings.Default.counterSave.Clear();
            foreach (KeyValuePair<string, SavedCounter> test in SavedCounters)
            {
                string counterString = test.Key + "," + test.Value.Category + "," + test.Value.Counter + "," + test.Value.Instance;
                Properties.Settings.Default.counterSave.Add(counterString);
            }
            Properties.Settings.Default.Save();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.ShowInTaskbar) this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            domainUpDown2.SelectedIndex = Properties.Settings.Default.nbrRepeat;
            domainUpDown1.SelectedIndex = Properties.Settings.Default.rptDelay;
            comboBox1.SelectedIndex = Properties.Settings.Default.Port;
            comboBox2.SelectedIndex = Properties.Settings.Default.Baud;
            checkBox5.Checked = Properties.Settings.Default.scrolling;
            checkBox6.Checked = Properties.Settings.Default.AutoCnct;

            

            if (checkBox5.CheckState == CheckState.Checked)
            {
                scrollTXT = 1;
            }
            else
            {
                scrollTXT = 2;
            }

            foreach (string lcdload in Properties.Settings.Default.L1)
            {
                textLCD.Add(lcdload);
            }

            string expr = @"([^,]+),|([^,]+)";
            foreach (string text in Properties.Settings.Default.counterSave)
            {
                string[] initiateMyCounter = new string[5];
                MatchCollection mc = Regex.Matches(text, expr);
                //foreach (Match m in mc)
                if (mc.Count < 4)
                {
                    for (int i = 0; i < mc.Count; i++)
                    {

                        initiateMyCounter[i] = mc[i].Groups[1].Value;

                    }
                }
                else
                {
                    for (int i = 0; i < (mc.Count - 1); i++)
                    {

                        initiateMyCounter[i] = mc[i].Groups[1].Value;

                    }
                    initiateMyCounter[mc.Count - 1] = mc[mc.Count - 1].Groups[2].Value;
                }
                SavedCounters[initiateMyCounter[0]] = new SavedCounter(initiateMyCounter[1], initiateMyCounter[2], initiateMyCounter[3]);
            }

            foreach(var MyCounter in SavedCounters)
            {
                if(MyCounter.Value.Instance == null)
                {
                    classesF1[MyCounter.Key] = new PerformanceCounter(MyCounter.Value.Category, MyCounter.Value.Counter);
                }
                else
                {
                    classesF1[MyCounter.Key] = new PerformanceCounter(MyCounter.Value.Category, MyCounter.Value.Counter, MyCounter.Value.Instance);
                }
            }

            /*foreach(string nameload in Properties.Settings.Default.CounterName)
            {
                SavedCounters[nameload] = new SavedCounter(Properties.Settings.Default.CounterCategory[0], Properties.Settings.Default.CounterEName[0], Properties.Settings.Default.CounterInstance[0]);
            }*/
            //System.Diagnostics.PerformanceCounterCategory.GetCategories();
            if (checkBox6.CheckState == CheckState.Checked)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    //progressBar1.Value = 100;
                    toolStripProgressBar1.Value = 100;
                    toolStripStatusLabel1.Text = "Connected";
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                    trackBar1.Enabled = true;
                    timer1.Enabled = true;
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                }
                catch (Exception ex)
                {
                    timer1.Enabled = false;
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (this.WindowState == FormWindowState.Minimized && cursorNotInBar)
            {
                this.Hide();
                //notifyIcon1.ShowBalloonTip(1000, "App is here", "Still runing just minimized", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(!this.ShowInTaskbar)this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.Open();
                //progressBar1.Value = 100;
                toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "Connected";
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                trackBar1.Enabled = true;
                timer1.Enabled = true;
            }
            catch(Exception ex)
            {
                timer1.Enabled = false;
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                serialPort1.Close();
                //progressBar1.Value = 0;
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Disconnected";
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                trackBar1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int trackbar = Convert.ToInt32(trackBar1.Value);
            byte[] send_buffer = new byte[3];
            send_buffer[0] = 254;
            send_buffer[1] = 153;
            send_buffer[2] = Convert.ToByte(trackbar);
            serialPort1.Write(send_buffer, 0, 3);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            int trackbar = Convert.ToInt32(trackBar1.Value);
            byte[] send_buffer = new byte[3];
            send_buffer[0] = 254;
            send_buffer[1] = 152;
            send_buffer[2] = Convert.ToByte(trackbar);
            serialPort1.Write(send_buffer, 0, 3);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.CheckState == CheckState.Checked)
            {
                byte[] send_buffer = new byte[3];
                send_buffer[0] = 254;
                send_buffer[1] = 87;
                send_buffer[2] = 1;
                serialPort1.Write(send_buffer, 0, 3);
            }
            else
            {
                byte[] send_buffer = new byte[3];
                send_buffer[0] = 254;
                send_buffer[1] = 86;
                send_buffer[2] = 0;
                serialPort1.Write(send_buffer, 0, 3);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
            {
                byte[] send_buffer = new byte[2];
                send_buffer[0] = 254;
                send_buffer[1] = 83;
                serialPort1.Write(send_buffer, 0, 2);
            }
            else
            {
                byte[] send_buffer = new byte[2];
                send_buffer[0] = 254;
                send_buffer[1] = 84;
                serialPort1.Write(send_buffer, 0, 2);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Checked)
            {
                byte[] send_buffer = new byte[3];
                send_buffer[0] = 254;
                send_buffer[1] = 66;
                send_buffer[2] = 1;
                serialPort1.Write(send_buffer, 0, 3);
            }
            else
            {
                byte[] send_buffer = new byte[2];
                send_buffer[0] = 254;
                send_buffer[1] = 70;
                serialPort1.Write(send_buffer, 0, 2);
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.CheckState == CheckState.Checked)
            {
                byte[] send_buffer = new byte[2];
                send_buffer[0] = 254;
                send_buffer[1] = 74;
                serialPort1.Write(send_buffer, 0, 2);
            }
            else
            {
                byte[] send_buffer = new byte[2];
                send_buffer[0] = 254;
                send_buffer[1] = 75;
                serialPort1.Write(send_buffer, 0, 2);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            Form2 f2 = new Form2(serialPort1, textLCD, classesF1, SavedCounters);
            if(!Application.OpenForms.OfType<Form2>().Any())
                { f2.Show(); }
            

            /*foreach (Form a in Application.OpenForms)
            {
                if (a is Form2)
                {
                    break;
                }
                
            }
            f2.Show();*/

        }


        int repeat = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {

            List<string> textLCD2 = new List<string>(textLCD);
            string expr = @"\{(.*?)\}";
            for (int i = 0; i < textLCD2.Count; i++)
            {
                MatchCollection mc = Regex.Matches(textLCD2[i], expr);
                if(mc.Count >= 1)
                {
                    foreach (Match m in mc)
                    {
                        string input = textLCD2[i];
                        string pattern = @"\{" + m.Groups[1].Value + @"\}";
                        string replacement = classesF1[m.Groups[1].Value].NextValue().ToString("F2");
                        Regex rgx = new Regex(pattern);
                        string temp = rgx.Replace(input, replacement);
                        textLCD2.RemoveAt(i);
                        textLCD2.Insert(i, temp);

                    }
                }
                
            }

            if (count != (textLCD2.Count - 1))
                {
                count = (textLCD2.Count - 1);
                bufind = 0;
                }
            
            byte[] send_buffer = new byte[4];
            if(bufind > count)
            {
                bufind = 0;
            }
            
            if (textLCD2.Count > 0 && serialPort1.IsOpen)
            {
                send_buffer[0] = 254;
                send_buffer[1] = 88;
                serialPort1.Write(send_buffer, 0, 2);
                /*if (scrollTXT)
                {
                    send_buffer[0] = 254;
                    send_buffer[1] = 200;
                    serialPort1.Write(send_buffer, 0, 2);
                }*/
                serialPort1.Write(textLCD2[bufind % (count + 1)]);
                send_buffer[0] = 254;
                send_buffer[1] = 71;
                send_buffer[2] = 1;
                send_buffer[3] = 2;
                serialPort1.Write(send_buffer, 0, 4);
                serialPort1.Write(textLCD2[(bufind + 1) % (count + 1)]);
                repeat++;
                if(repeat >= nbrRepeat)
                {
                    repeat = 0;
                    if ((bufind) < (count))
                    {
                        bufind += scrollTXT;
                    }
                    else
                    {
                        bufind = 0;
                    }
                }
                
            }
            else
            {
                timer1.Enabled = false;
                MessageBox.Show("Please Connect", "Not Connected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            

        }
        
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            timer1.Interval = int.Parse(domainUpDown1.Text);
        }

        private void domainUpDown2_SelectedItemChanged(object sender, EventArgs e)
        {
            nbrRepeat = int.Parse(domainUpDown2.Text);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            byte[] send_buffer = new byte[2];
            if (checkBox5.CheckState == CheckState.Checked)
            {
                scrollTXT = 1;
            }
            else
            {
                scrollTXT = 2;
            }
        }
    }
    public class SavedCounter
    {
        private string category;
        private string counter;
        private string instance;

        public SavedCounter(string Category, string counter, string instance)
        {
            this.category = Category;
            this.counter = counter;
            this.instance = instance;
        }
        public SavedCounter(string Category, string Counter)
        {
            this.category = Category;
            this.counter = Counter;
        }

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        public string Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }
}
