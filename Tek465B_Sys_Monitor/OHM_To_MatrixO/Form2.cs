using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Tek465B_Sys_Monitor
{
    

    public partial class Form2 : Form
    {
        SerialPort myPort;

        private List<string> _LCDlist;
        List<TextBox> textBoxList = new List<TextBox>();
        Dictionary<string, SavedCounter> CounterInfo = new Dictionary<string, SavedCounter>();
        Dictionary<string, PerformanceCounter> classes = new Dictionary<string, PerformanceCounter>();

        [DllImport("pdh.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern UInt32 PdhLookupPerfIndexByName(string szMachineName, string szNameBuffer, ref uint pdwIndex);
        private TextBox SeltextBox = null;
        public Form2(SerialPort port, List<string> list, Dictionary<string, PerformanceCounter> classeDict, Dictionary<string, SavedCounter> SavedCounters)

        {
            
            InitializeComponent();
            classes = classeDict;
            _LCDlist = list;
            myPort = port;
            CounterInfo = SavedCounters;
            //this.textBox2.GotFocus += new EventHandler(GetBox);
            this.textBox1.Enter += new EventHandler(GetBox);
            this.textBox2.Enter += new EventHandler(GetBox);
            this.textBox3.Enter += new EventHandler(GetBox);
            this.textBox4.Enter += new EventHandler(GetBox);
            this.textBox5.Enter += new EventHandler(GetBox);
            this.textBox6.Enter += new EventHandler(GetBox);
            this.textBox7.Enter += new EventHandler(GetBox);
            this.textBox8.Enter += new EventHandler(GetBox);
            this.textBox9.Enter += new EventHandler(GetBox);
            this.textBox10.Enter += new EventHandler(GetBox);
            this.textBox11.Enter += new EventHandler(GetBox);
            this.textBox12.Enter += new EventHandler(GetBox);
            this.textBox13.Enter += new EventHandler(GetBox);
            this.textBox14.Enter += new EventHandler(GetBox);
            this.textBox15.Enter += new EventHandler(GetBox);
            this.textBox16.Enter += new EventHandler(GetBox);
            this.textBox17.Enter += new EventHandler(GetBox);
            this.textBox18.Enter += new EventHandler(GetBox);
            this.textBox19.Enter += new EventHandler(GetBox);
            this.textBox20.Enter += new EventHandler(GetBox);
        }
        
        private void GetBox(object sender, EventArgs e)
        {
            SeltextBox = sender as TextBox;

        }
        /*public void setSerialPort(SerialPort port)
        {
            myPort = port;
        }*/

        private void button1_Click(object sender, EventArgs e)
        {

            _LCDlist.Clear();
            textBoxList.Clear();
            textBoxList.Add(new TextBox { Text = textBox1.Text });
            textBoxList.Add(new TextBox { Text = textBox2.Text });
            textBoxList.Add(new TextBox { Text = textBox3.Text });
            textBoxList.Add(new TextBox { Text = textBox4.Text });
            textBoxList.Add(new TextBox { Text = textBox5.Text });
            textBoxList.Add(new TextBox { Text = textBox6.Text });
            textBoxList.Add(new TextBox { Text = textBox7.Text });
            textBoxList.Add(new TextBox { Text = textBox8.Text });
            textBoxList.Add(new TextBox { Text = textBox9.Text });
            textBoxList.Add(new TextBox { Text = textBox10.Text });
            textBoxList.Add(new TextBox { Text = textBox11.Text });
            textBoxList.Add(new TextBox { Text = textBox12.Text });
            textBoxList.Add(new TextBox { Text = textBox13.Text });
            textBoxList.Add(new TextBox { Text = textBox14.Text });
            textBoxList.Add(new TextBox { Text = textBox15.Text });
            textBoxList.Add(new TextBox { Text = textBox16.Text });
            textBoxList.Add(new TextBox { Text = textBox17.Text });
            textBoxList.Add(new TextBox { Text = textBox18.Text });
            textBoxList.Add(new TextBox { Text = textBox19.Text });
            textBoxList.Add(new TextBox { Text = textBox20.Text });

            for (int i = 0; i < textBoxList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(textBoxList[i].Text))
                {
                    if((string.IsNullOrWhiteSpace(textBoxList[i + 1].Text)) && (i % 2 == 0))
                    {
                        MessageBox.Show("Please fill both line", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    else
                    {
                        _LCDlist.Add(textBoxList[i].Text);
                    }
                    
                }
            }

            foreach (PerformanceCounter key in classes.Values)
            {
                key.Close();
            }

            string expr = @"\{(.*?)\}";
            foreach (string text in _LCDlist)
            {
                MatchCollection mc = Regex.Matches(text, expr);
                foreach (Match m in mc)
                {
                    if(CounterInfo[m.Groups[1].Value].Instance == null)
                    {
                        classes[m.Groups[1].Value] = new PerformanceCounter(CounterInfo[m.Groups[1].Value].Category, CounterInfo[m.Groups[1].Value].Counter);
                        //textBox1.Text += (int)classes[m.Groups[1].Value].NextValue();
                    }
                    else
                    {
                        classes[m.Groups[1].Value] = new PerformanceCounter(CounterInfo[m.Groups[1].Value].Category, CounterInfo[m.Groups[1].Value].Counter, CounterInfo[m.Groups[1].Value].Instance);
                    }
                    
                }
            }

            

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            listBox2.Items.Clear();
            string[] instances = categories[listBox1.SelectedIndex].GetInstanceNames();
            if (instances.Any())
            {
                listBox3.Items.Clear();
                foreach (string instance in instances)
                {
                    if (categories[listBox1.SelectedIndex].InstanceExists(instance))
                    {
                        listBox3.Items.Add(instance);
                        
                    }
                }
            }
            else
            {
                
                listBox3.Items.Clear();
                mypc = categories[listBox1.SelectedIndex].GetCounters();
                foreach (PerformanceCounter counter in mypc)
                {
                    listBox2.Items.Add(counter.CounterName);
                }
            }
            
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] instances = categories[listBox1.SelectedIndex].GetInstanceNames();
            listBox2.Items.Clear();
            if (instances.Any() && (categories[listBox1.SelectedIndex].InstanceExists(instances[listBox3.SelectedIndex])))
            {
                mypc = categories[listBox1.SelectedIndex].GetCounters(instances[listBox3.SelectedIndex]);
                foreach (PerformanceCounter counter in mypc)
                {
                    if (categories[listBox1.SelectedIndex].InstanceExists(instances[listBox3.SelectedIndex]))
                    {
                        listBox2.Items.Add(counter.CounterName);
                    }

                }
            }
        }
        
        System.Diagnostics.PerformanceCounter[] mypc;
        PerformanceCounterCategory[] categories = System.Diagnostics.PerformanceCounterCategory.GetCategories();
        private void Form2_Load(object sender, EventArgs e)
        {
            
            foreach (PerformanceCounterCategory cat in categories)
            {
                listBox1.Items.Add(cat.CategoryName.ToString());
            }
               

            if (_LCDlist.Count > 1)
            {

                TextBox[] tbs = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19 };
                for (int i = 0; i < _LCDlist.Count; i++)
                {
                    tbs[i].Text = _LCDlist[i];
                }
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TextBox[] tbs = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19 };
            for (int i = 0; i < tbs.Length; i++)
            {
                tbs[i].Clear();
            }
        }
        
        private void listBox2_MouseClick(object sender, MouseEventArgs e)

        {
            string result = GetEnglishName(listBox1.SelectedItem.ToString());
            string result2 = GetEnglishName(listBox2.SelectedItem.ToString());
            if (listBox3.Items.Count >= 1)
            {
                string result3 = GetEnglishName(listBox3.SelectedItem.ToString());
                if (CounterInfo.ContainsKey(listBox2.SelectedItem.ToString()) == true)
                {
                    CounterInfo[listBox2.SelectedItem.ToString() + "2"] = new SavedCounter(result, result2, result3);
                    if (SeltextBox != null) SeltextBox.Text += "{" + listBox2.SelectedItem.ToString() + "2}";
                    //classes.ContainsKey(listBox2.SelectedItem.ToString());
                }
                
                else
                {
                    CounterInfo[listBox2.SelectedItem.ToString()] = new SavedCounter(result, result2, result3);
                    if (SeltextBox != null) SeltextBox.Text += "{" + listBox2.SelectedItem.ToString() + "}";
                    //classes.ContainsKey(listBox2.SelectedItem.ToString());
                }
                
            }
                
            else
            {
                try
                {
                    /*foreach(SavedCounter counter in CounterInfo)
                    {
                        string test = counter.Counter;
                    }*/
                    if(CounterInfo.ContainsKey(listBox2.SelectedItem.ToString()) == true)
                    {
                        CounterInfo[listBox2.SelectedItem.ToString() + "2"] = new SavedCounter(result, result2);
                        if (SeltextBox != null) SeltextBox.Text += "{" + listBox2.SelectedItem.ToString() + "2}";
                    }
                    else
                    {
                        CounterInfo[listBox2.SelectedItem.ToString()] = new SavedCounter(result, result2);
                        //CounterInfo.Add(new SavedCounter(result, result2));
                        //classes[listBox2.SelectedItem.ToString()] = new PerformanceCounter(result, result2);
                        //textBox1.Text += (int)classes[listBox2.SelectedItem.ToString()].NextValue();
                        if (SeltextBox != null) SeltextBox.Text += "{" + listBox2.SelectedItem.ToString() + "}";
                    }
                    
                } catch (InvalidOperationException errmess)
                {
                    MessageBox.Show("Counter do not exits\nWindows Error:\n" + errmess.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
                
        }

        public string GetEnglishName(string name)
        {
            string buffer2 = name;
            UInt32 iRet3 = new UInt32();
            UInt32 iRet2 = new UInt32();
            iRet3 = PdhLookupPerfIndexByName(null, buffer2, ref iRet2);
            //Console.WriteLine(iRet2.ToString());

            RegistryKey pRegKey = Registry.LocalMachine;
            pRegKey = pRegKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009");
            string[] after;
            after = (string[])pRegKey.GetValue("Counter");
            string value = iRet2.ToString();
            int pos = Array.IndexOf(after, value);
            if (after[pos + 1] == "1")
                return name;
            else
                return after[pos + 1];
        }
    }
    
}
