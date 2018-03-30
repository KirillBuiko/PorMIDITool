using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace PorMIDITool
{
    public partial class Form2 : Form
    {
        public bool open=false;
        public int channel = 0;
        public int deviceIDOut = 0;
        public int deviceIDIn = 0;
        public string deviceCOM = "";
        bool portfound = false;
        Thread thr;


        public Form2()
        {
            InitializeComponent();
        }
        void t()
        {
            button2.Enabled = false;
            SerialPort com = new SerialPort();
            com.DtrEnable = true;
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Maximum = SerialPort.GetPortNames().Length * 3; });
            //MessageBox.Show(SerialPort.GetPortNames().Length.ToString());
            int count = 0;

            for (int i = SerialPort.GetPortNames().Length - 1; i > 0; i--)
            {
                string s = SerialPort.GetPortNames()[i];
                if (portfound)
                {
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progressBar1.Maximum; });
                    return;
                }
                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = count += 3; });
                //MessageBox.Show(count.ToString());
                com.Close(); // To handle the exception, in case the port isn't found and then they try again...
                com.PortName = s;
                com.BaudRate = 9600;
                try
                {
                    //MessageBox.Show("try con " + com.PortName);
                    com.Open();
                }
                catch (Exception)
                {
                    //MessageBox.Show("er try con " + com.PortName);
                }
                if (!portfound)
                {
                    if (com.IsOpen) // Port has been opened properly...
                    {
                        //MessageBox.Show("opn " + com.PortName);
                        try
                        {
                            com.ReadTimeout = 1000; com.ReadLine();
                            string comms = com.ReadLine();
                            //MessageBox.Show("red " + com.PortName + " "+comms);
                            if (comms.Substring(0, 8).Equals("I'm PMT!")) // We have found the arduino!
                            {
                                //MessageBox.Show("fnd " + com.PortName);
                                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progressBar1.Maximum; });
                                label5.Invoke((MethodInvoker)delegate { label5.Text = "Порт найден!"; });
                                com.Write("I hear you!");
                                deviceCOM = com.PortName;
                                button1.Invoke((MethodInvoker)delegate { button1.Enabled = true; });
                                com.Close();
                                portfound = true;
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("er red " + com.PortName+" "+ex.ToString());
                        }
                    }
                }
            }
            com.Close();
            if (!portfound)
            {
                label5.Invoke((MethodInvoker)delegate { Text = "Порт не найден!"; });
                MessageBox.Show("Порт не найден!\nПроверьте, подключено ли устроство и повторите", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            button2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count != 0) open = true;
            this.Close();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            thr.Abort();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            deviceIDOut = comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            channel = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            deviceIDIn = comboBox3.SelectedIndex;
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            thr = new Thread(t);
            thr.Start();
        }
    }
}
