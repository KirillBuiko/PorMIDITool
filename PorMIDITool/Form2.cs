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

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count != 0) open = true;
            this.Close();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            Thread thr = new Thread(t);
            void t() {
                SerialPort com = new SerialPort();
                progressBar1.Invoke((MethodInvoker)delegate{progressBar1.Maximum = SerialPort.GetPortNames().Length;});
                //MessageBox.Show(SerialPort.GetPortNames().Length.ToString());
                int count = 0;
                foreach (string s in SerialPort.GetPortNames())
                {
                    progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = ++count; });
                    //MessageBox.Show(count.ToString());
                    com.Close(); // To handle the exception, in case the port isn't found and then they try again...
                    bool portfound = false;
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
                            MessageBox.Show("opn " + com.PortName);
                            com.ReadTimeout = 500; // 500 millisecond timeout...
                            try
                            {
                                //MessageBox.Show("red " + com.PortName);
                                com.ReadTimeout = 200;
                                string comms = com.ReadLine();
                                MessageBox.Show("red " + com.PortName);
                                if (comms.Equals("I'm PMT!")) // We have found the arduino!
                                {
                                    MessageBox.Show("fnd " + com.PortName);
                                    //progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progressBar1.Maximum; });
                                    //label5.Invoke((MethodInvoker)delegate { label5.Text = "Порт найден!"; });
                                    //com.Write("I hear you!");
                                    //deviceCOM = com.PortName;
                                    //button1.Invoke((MethodInvoker)delegate { button1.Enabled = true; });
                                    return;
                                }
                            }
                            catch (Exception ex)
                            { 
                                MessageBox.Show("er red " + com.PortName+" "+ex.ToString());
                            }
                        }
                    }
                }
                if (!com.IsOpen)
                {
                    label5.Invoke((MethodInvoker)delegate { Text = "Порт не найден!"; });
                    MessageBox.Show("Порт не найден!\nПроверьте, подключено ли устроство и перезапустите программу", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.Invoke((MethodInvoker)delegate { Close(); });
                }
            }
            thr.Start();
        }
    }
}
