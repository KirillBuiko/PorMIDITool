using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading.Tasks;

namespace PorMIDITool
{
    public partial class FormSettings : Form
    {
        public bool open=false;
        public int channel = 0;
        public int deviceIDOut = 0;
        public int deviceIDIn = 0;
        public string deviceCOM = "";
        bool portfound = false;
        Task thr;


        public FormSettings()
        {
            InitializeComponent();
        }
        void t()
        {   int count = 0;SerialPort com = new SerialPort();
            try
            {
                label5.Invoke((MethodInvoker)delegate { label5.Text = "Поиск порта..."; });
                button2.Invoke((MethodInvoker)delegate { button2.Enabled = false; });
                
                com.DtrEnable = true;
                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Maximum = SerialPort.GetPortNames().Length * 3; });
                //MessageBox.Show(SerialPort.GetPortNames().Length.ToString());
                
                portfound = false;
            }
            catch (Exception) { }
            for (int i = SerialPort.GetPortNames().Length - 1; i >= 0; i--)
            {
                try
                {
                    string s = SerialPort.GetPortNames()[i];
                    if (portfound)
                    {
                        progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progressBar1.Maximum; });
                        return;
                    }
                    progressBar1.Invoke((MethodInvoker)delegate { count += 3; progressBar1.Value = count; });
                    //MessageBox.Show(count.ToString());
                    com.Close(); // To handle the exception, in case the port isn't found and then they try again...
                    com.PortName = s;
                    com.BaudRate = 115200;

                    //MessageBox.Show("try con " + com.PortName);
                    com.Open();
                }
                catch (Exception) { }

                try
                {
                    if (!portfound)
                    {
                        if (com.IsOpen) // Port has been opened properly...
                        {
                            //MessageBox.Show("opn " + com.PortName);
                            com.ReadTimeout = 5000; com.ReadLine();
                            string comms = com.ReadLine();
                            //MessageBox.Show("red " + com.PortName + " "+comms);
                            if (comms.Substring(0, 8).Equals("I'm PMT!")) // We have found the arduino!
                            {
                                //MessageBox.Show("fnd " + com.PortName);
                                progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progressBar1.Maximum; });
                                label5.Invoke((MethodInvoker)delegate { label5.Text = "Порт найден!"; });
                                deviceCOM = com.PortName;
                                button1.Invoke((MethodInvoker)delegate { button1.Enabled = true; });
                                portfound = true;
                                button2.Invoke((MethodInvoker)delegate { button2.Enabled = true; });
                                try
                                {
                                    com.Close();
                                }
                                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                                return;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("er red " + com.PortName+" "+ex.ToString());
                }
            }
            try
            {
                com.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            try
            {
                if (!portfound)
                {

                    label5.Invoke((MethodInvoker)delegate { label5.Text = "Порт не найден!"; });
                    MessageBox.Show("Порт не найден!\nПроверьте, подключено ли устройство и повторите", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                }
                button2.Invoke((MethodInvoker)delegate { button2.Enabled = true; });
            }
            catch (Exception) { }
            return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count != 0) open = true;
            this.Close();
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
            thr = new Task(t);
            thr.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            thr = new Task(t);
            thr.Start();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
