using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using NAudio.Midi;
using System.Drawing;


namespace PorMIDITool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public MidiOut midiout;
        public MidiIn midiin;
        int channel = 0;
        int k = 0;
        int modulesCounter = 0;
        int octave;
        int profile;
        int sleepTime = 30;
        SerialPort com;
        GroupBox gb;
        FormSettings f2;
        ChooseModule cm;
        FileStream profiles;
        FileStream conf;
        bool formClosing = false;
        Thread SerialTask;
        ColorDialog MyDialog;
        FormC[] modules;
        string[] colorsNames = { "Красный", "Зелёный", "Синий", "Жёлтый", "Голубой", "Сереневый", "Белый" };
        public static Color[] colors = { Color.FromArgb(252, 38, 70)/*Red*/, Color.FromArgb(112, 186, 60)/*Green*/, Color.FromArgb(65, 105, 225)/*Blue*/, Color.FromArgb(225, 204, 79)/*Yellow*/, Color.FromArgb(92, 154, 204)/*Strange Blue*/, Color.FromArgb(153, 102, 204)/*Purple*/, Color.FromArgb(245, 245, 245)/*White*/  };
        byte[] by; string[] profstr; object sd; TextBox[,] tb; TextBox[,] addptb; Button[,] b;

        void tb_Press(object s, KeyPressEventArgs args)
        {
            if ((char.IsDigit(args.KeyChar)) || args.KeyChar == 8)
            {
                if (Char.IsDigit(args.KeyChar) && Convert.ToInt32((s as TextBox).Text + args.KeyChar) > 127)
                {
                    Controls["tb" + ((Convert.ToInt32((s as TextBox).Tag)) + 1).ToString()].Text = "127";
                    args.Handled = true;
                }
                return;
            }
            else args.Handled = true;
        }
        void repair(string channel)
        {
            MessageBox.Show("Проблема с открытием профиля.\nОн будет восстановлен.", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + channel + ".pmt", "-1 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 20 21 22 23 24 25 26 27 28 29");
            profstr = new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29" };
        }
        void prof_Click(object s, EventArgs args)
        {
            profile = Convert.ToInt32((s as ToolStripMenuItem).Text);
            профильToolStripMenuItem.Text = "Профиль (" + profile.ToString() + ")";
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\conf.pmt", profile.ToString());
            try
            {
                profstr = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[0].Split(' ');
                for (int i = 1; i < 27; i++)
                {
                    if (Convert.ToInt32(profstr[i]) > 127 || Convert.ToInt32(profstr[i]) < 0) { profstr[i] = i.ToString(); }
                }
            }
            catch (Exception)
            {
                repair(profile.ToString());
            }
            octave = Convert.ToInt32(profstr[0]);
            (sd as RadioButton).Text = (octave).ToString();
            oct_Click(sd, EventArgs.Empty);
            k = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    k++;
                    tb[j, i].Text = profstr[k];
                    b[j, i]=Func.button_update(i * 4 + j, b[j, i], profile.ToString());
                }
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    k++;
                    addptb[i, j].Text = profstr[k];
                }
            }
            if (com.IsOpen)
            {
                String message = "";
                com.Write("ColorChange");
                Thread.Sleep(sleepTime);
                for (int i = 0; i < 16; i++)
                {
                    try
                    {
                        String[] colset = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[i + 1].Split(' ');
                        com.Write(colset[0] + colset[1]);
                    }
                    catch (Exception)
                    {
                        com.Write("20");
                    }
                    Thread.Sleep(sleepTime);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modules = new FormC[6];
            for (int i = 0; i < 6; i++)
            {
                modules[i] = new FormC();
            }
            ToolTip t = new ToolTip();t.SetToolTip(addmodb3, "Добавить подключённый модуль"); t.SetToolTip(colorb, "Выбрать цвета подсветки");
            MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            SerialTask = new Thread(SerialTaskR);
            com = new SerialPort(); com.DtrEnable = true;
            Directory.CreateDirectory("c:\\ProgramData\\PorMIDITool\\profiles");
            conf = new FileStream("c:\\ProgramData\\PorMIDITool\\conf.pmt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            by = new byte[conf.Length]; conf.Read(by, 0, by.Length); conf.Close();
            string fstr = Encoding.UTF7.GetString(by);//.Split(new char[] { ' ' });
            try
            {
                if (Convert.ToInt32(fstr) < 1 || Convert.ToInt32(fstr) > 12) { fstr = "1"; }
            }
            catch (Exception) { fstr = "1"; }
            profile = Convert.ToInt16(fstr);


            try
            {
                profstr = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + fstr + ".pmt")[0].Split(' ');
                for (int i = 1; i < 27; i++)
                {
                    if (Convert.ToInt32(profstr[i]) > 127 || Convert.ToInt32(profstr[i]) < 0) { profstr[i] = i.ToString(); }
                }
            }
            catch (Exception)
            {
                repair(fstr);
            }
            octave = Convert.ToInt32(profstr[0]);
            профильToolStripMenuItem.Text = "Профиль (" + profile.ToString() + ")";
            tb = new TextBox[4, 4];
            ////////////////////////////////////k=0////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tb[j, i] = new TextBox();
                    tb[j, i].Tag = k++;
                    tb[j, i].Name = "tb" + k;
                    tb[j, i].Size = new System.Drawing.Size(30, 8);
                    tb[j, i].Text = profstr[k];
                    tb[j, i].KeyPress += new KeyPressEventHandler(tb_Press);
                    tb[j, i].Location = new Point(j * 68 + 45, i * 73 + 75);
                    Controls.Add(tb[j, i]);
                    tb[j, i].BringToFront();
                }
            }
            
            b = new Button[4, 4];
            ////////////////////////////////////k=15////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b[j, i] = new Button();
                    b[j, i].Tag = k++;
                    b[j, i].Name = "button" + (k - 16).ToString();
                    b[j, i].Size = new System.Drawing.Size(65, 70);
                    b[j, i].Location = new Point(j * 68 + 30, i * 73 + 30);
                    b[j, i].MouseDown += new MouseEventHandler(this.button_MouseDown);
                    b[j, i].MouseUp += new MouseEventHandler(this.button_MouseUp);
                    Func.button_update(i*4+j,b[j,i],fstr);
                    Controls.Add(b[j, i]);
                }
            }
            Label[,] addpl = new Label[2, 5];
            {
                int col = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        col++;
                        addpl[i, j] = new Label();
                        addpl[i, j].Name = "button" + (k - 32).ToString();
                        addpl[i, j].Size = new System.Drawing.Size(56, 20);
                        addpl[i, j].Font = new Font(addpl[i, j].Font.Name, 14, addpl[i, j].Font.Style);
                        addpl[i, j].Text = col.ToString() + "CC";
                        addpl[i, j].Location = new Point(330 + j * 60, i * 55 + 150);
                        Controls.Add(addpl[i, j]);
                    }
                }
            }
            ////////////////////////////////////k=32////////////////////////
            addptb = new TextBox[2, 5];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    addptb[i, j] = new TextBox();
                    addptb[i, j].Tag = k++;
                    addptb[i, j].Name = "addtb" + (k - 32).ToString();
                    addptb[i, j].Size = new System.Drawing.Size(sleepTime, 20);
                    addptb[i, j].Text = profstr[k - 16];
                    addptb[i, j].Location = new Point(335 + j * 60, i * 55 + 180);
                    Controls.Add(addptb[i, j]);
                }
            }
            RadioButton[] oct = new RadioButton[11];
            gb = new GroupBox();
            gb.Location = new System.Drawing.Point(0, -10);
            gb.Size = new System.Drawing.Size(20, 150);
            for (int j = 0; j < 11; j++)
            {
                oct[j] = new RadioButton();
                oct[j].Text = (j - 1).ToString();
                oct[j].Name = "octrb" + (j + 1).ToString();
                oct[j].Click += new EventHandler(oct_Click);
                oct[j].Location = new System.Drawing.Point(10, j * 20 + 10);
                if (octave == (j - 1)) oct[j].Checked = true;
                gb.Controls.Add(oct[j]);
            }
            октаваToolStripMenuItem.DropDownItems.Insert(0, new ToolStripControlHost(gb));
            sd = new RadioButton();
            oct[octave + 1].Checked = true;
            ToolStripMenuItem[] prof = new ToolStripMenuItem[12];
            for (int j = 0; j < 12; j++)
            {
                prof[j] = new ToolStripMenuItem();
                prof[j].Text = (j + 1).ToString();
                prof[j].Click += new EventHandler(prof_Click);
                prof[j].ShortcutKeys = ((Keys)(Keys.Control | (Keys)j + 49));
                профильToolStripMenuItem.DropDownItems.Add(prof[j]);
            }
            prof[9].ShortcutKeys = ((Keys)(Keys.Control | (Keys)48));
            prof[10].ShortcutKeys = ((Keys)(Keys.Control | (Keys)189));
            prof[11].ShortcutKeys = ((Keys)(Keys.Control | (Keys)187));
            //prof_Click();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            f2 = new FormSettings();
            cm = new ChooseModule();
            int k = 0;
            k = MidiOut.NumberOfDevices;
            for (int i = 0; i < k; i++)
            {
                f2.comboBox1.Items.Add(MidiOut.DeviceInfo(i).ProductName.ToString());
            }
            if (k > 0) f2.comboBox1.SelectedIndex = 0;
            k = 0;
            k = MidiIn.NumberOfDevices;
            for (int i = 0; i < k; i++)
            {
                f2.comboBox3.Items.Add(MidiIn.DeviceInfo(i).ProductName.ToString());
            }
            if (k > 0) f2.comboBox3.SelectedIndex = 0;
            f2.comboBox2.SelectedIndex = 0;
            f2.StartPosition = FormStartPosition.CenterParent;
            f2.FormClosing += SettingsClose;
            f2.ShowDialog();
            cm.StartPosition = FormStartPosition.CenterScreen;
            cm.FormClosing += ChooseModuleClose;
        }
        public void SettingsClose(object sender, FormClosingEventArgs e)
        {
            if (f2.open == true)
            {
                
                try
                {
                    //midiin.MessageReceived += null;
                    //midiin.Close();
                    midiout.Close();
                }
                catch (NullReferenceException) { }
                try
                {
                    //midiin = new MidiIn(f2.deviceIDIn);
                    //midiin.MessageReceived += Midiin_MessageReceived;
                    //midiin.Start();
                }
                catch (Exception) { MessageBox.Show("Устройство на вход не было открыто. Попробуйте изменить настройки"); }
                try
                {
                    midiout = new MidiOut(f2.deviceIDOut);
                }

                catch (Exception) { MessageBox.Show("Устройство на выход не было открыто. Попробуйте изменить настройки"); }
                try
                { 
                    com.Close();
                }
                catch (Exception) { }
                try
                {
                    com.BaudRate = 115200;
                    com.PortName = f2.deviceCOM;
                    com.Open();
                    Thread.Sleep(2000);
                    String message = "I hear you!";
                    for (int i = 0; i < 16; i++)
                    {
                        try
                        {
                            String[] colset = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[i + 1].Split(' ');
                            message = message + colset[0] + colset[1] + "_";
                        }
                        catch (Exception)
                        {
                            message = message + "20_";
                        }
                    }com.Write(message);
                    SerialTask.Start();
                }
                catch (Exception ex) { }
                channel = f2.channel;
            }
            else Close();
        }
        public void ChooseModuleClose(object sender, FormClosingEventArgs e)
        {

            if (cm.open == true)
            {
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i].Text == "")
                    {
                        modulesCounter++;
                        modules[i] = new FormC();
                        modules[i] = Func.Constructor(cm.Text, modules[i]);
                        modules[i].FormClosing += ModuleClose;
                        modules[i].Show();
                        modules[i].Left = this.Left;modules[i].Top = this.Top + 481;
                        break;
                    }
                }
            }
            
        }
        public void ModuleClose(object sender, FormClosingEventArgs e) { modulesCounter--; (sender as Form).Text = ""; }

        void SerialTaskR()
        {
            while (!formClosing)
                try
                {
                    if (com.IsOpen)
                    {
                        string[] rbuf;
                        if (com.BytesToRead>0)
                        {
                            rbuf = com.ReadLine().Split(new char[] { ' ' });
                            int note = 0;
                            bool flag = false;
                            if (rbuf[0] == "ModuleConnected")
                            {
                                Object b = new Object();EventArgs a = new EventArgs();
                                cm.chosenMod=rbuf[1];
                                addmodb3.Invoke((MethodInvoker)delegate { addmodb3_Click(b, a); });
                                flag = true;
                            }else if (rbuf[0] == "A1")
                            {flag = true;
                                if (rbuf[1] == "b") note = Convert.ToInt32(this.Controls["tb" + rbuf[2]].Text);
                                if (rbuf[1] == "cc") note = Convert.ToInt32(Controls["addtb" + rbuf[2]].Text);
                            }else
                            {
                                foreach (FormC form in modules)
                                {
                                    if (rbuf[0] == form.Text)
                                    {
                                        note = Convert.ToInt32(form.Controls.Find(rbuf[2], false)[0].Text);
                                        flag = true;
                                        break;
                                    }
                                }
                                
                            }
                            if (flag)
                            {
                                if (rbuf[1] == "b")
                                {
                                    //MessageBox.Show(rbuf[2].ToCharArray()[2].ToString());
                                    if (rbuf[3] == ("on" + '\r'))
                                    {
                                        midi_Send(note, "9", 127);
                                    }
                                    else midi_Send(note, "9", 0);
                                }
                                if (rbuf[1] == "cc")
                                {
                                    midi_Send(note, "B", Convert.ToInt32(rbuf[3].Substring(0, rbuf[3].Length - 1)));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { }
        }
        void button_MouseDown(object s, MouseEventArgs args)
        {
            int tag = Convert.ToInt32((s as Button).Tag);
            int note = Convert.ToInt32(this.Controls["tb" + (tag - 15).ToString()].Text);
            midi_Send(note, "9", 127);
        }
        void button_MouseUp(object s, MouseEventArgs args)
        {
            int tag = Convert.ToInt32((s as Button).Tag);
            int note = Convert.ToInt32(this.Controls["tb" + (tag - 15).ToString()].Text);
            midi_Send(note, "9", 0);
        }
        void midi_Send(int note, string type, int velocity)
        {
            String strnote;
            if (note < 16) strnote = "0" + note.ToString("X");
            else strnote = note.ToString("X");
            String vel;
            if (velocity < 16) vel = "0" + velocity.ToString("X");
            else vel = velocity.ToString("X");
            String channel = this.channel.ToString("X");
            midiout.Send(Convert.ToInt32(vel + strnote + type + channel, 16));
            switch (type)
            {
                case "8":
                    richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = DateTime.Now.ToString().Substring(11, DateTime.Now.ToString().Length - 11) + ", " + note + " Note Off, " + velocity + '\n' + richTextBox1.Text; });
                    break;
                case "9":
                    richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = DateTime.Now.ToString().Substring(11, DateTime.Now.ToString().Length - 11) + ", " + note + " Note On, " + velocity + '\n' + richTextBox1.Text; });
                    break;
                    case "B":
                        richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = DateTime.Now.ToString().Substring(11, DateTime.Now.ToString().Length - 11) + ", " + note + " CC, " + velocity + '\n' + richTextBox1.Text; });
                        break;
            }
        }

        private void Midiin_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            String midimes = e.RawMessage.ToString("X"); String type = ""; int note = 0; int velocity = 0;
            if (midimes != "") type = midimes.Substring(midimes.Length - 2);
            while (midimes.Length < 6) midimes.Insert(0, "0");
            note = Convert.ToInt32(midimes.Substring(midimes.Length - 4, 2), 16);
            velocity = note = Convert.ToInt32(midimes.Substring(midimes.Length - 6, 1), 16);
            if (type.Equals("8"))
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", " + note.ToString() + " Note Off, " + velocity.ToString() + '\n' + richTextBox2.Text; }));
            if (type.Equals("9"))
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", " + note.ToString() + " Note On, " + velocity.ToString() + '\n' + richTextBox2.Text; }));
            if (type.Equals("B"))//MessageBox.Show(midimes,"",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", CC " + note.ToString() + ", " + velocity.ToString() + '\n' + richTextBox2.Text; }));

            MessageBox.Show(e.RawMessage.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                midiout.Close();
                //midiin.Close();
                com.Close();
                SerialTask.Abort();
            }
            catch (Exception) { }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                com.Close();
            }
            catch (Exception) { }
            f2.ShowDialog();
        }

        private void поднятьНа1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (octave < 9) octave++;
            object s = new RadioButton();
            (s as RadioButton).Text = (octave).ToString();
            oct_Click(s, EventArgs.Empty);
        }

        private void опуститьНа1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (octave > -1) octave--;
            object s = new RadioButton();
            (s as RadioButton).Text = (octave).ToString();
            oct_Click(s, EventArgs.Empty);
        }
        //сохранить
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string save = ""; save += octave.ToString();
            String []Lines = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile.ToString()+".pmt");
            for (int i = 1; i < 17; i++)
            {
                save += " " + this.Controls["tb" + (i).ToString()].Text;
            }
            for (int i = 1; i < 11; i++)
            {
                save += " " + this.Controls["addtb" + (i).ToString()].Text;
            }Lines[0] = save;
            File.WriteAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile.ToString() + ".pmt", Lines);
        }
        void oct_Click(object s, EventArgs args)
        {
            octave = Convert.ToInt32((s as RadioButton).Text);
            (gb.Controls["octrb" + (octave + 2).ToString()] as RadioButton).Checked = true;
            for (int i = 0; i < 16; i++)
            {
                this.Controls["tb" + (i + 1).ToString()].Text = ((Convert.ToInt32((s as RadioButton).Text) + 1) * 12 + i).ToString();
                if ((Convert.ToInt32((s as RadioButton).Text) + 1) * 12 + i > 127) this.Controls["tb" + (i + 1).ToString()].Text = "127";
            }
        }

        private void загрузитьВКонтроллерToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void colorb_Click(object sender, EventArgs e)
        {
            Form colorc = new Form();
            colorc.Size = new Size(490, 420);
            colorc.Icon = this.Icon;
            colorc.FormBorderStyle = FormBorderStyle.FixedSingle;
            colorc.StartPosition = FormStartPosition.CenterScreen;
            colorc.Text = "Определение цветов";
            colorc.FormClosed += coloc_Closed;
            string[] colorsNames = { "Красный", "Зелёный", "Синий", "Жёлтый", "Голубой", "Сереневый", "Белый" };
            string[] typesNames = { "Включён всегда", "Включён при нажатие", "Выключен" };
            Color[] colors = { Color.FromArgb(252, 38, 70)/*Red*/, Color.FromArgb(112, 186, 60)/*Green*/, Color.FromArgb(65, 105, 225)/*Blue*/, Color.FromArgb(225, 204, 79)/*Yellow*/, Color.FromArgb(92, 154, 204)/*Strange Blue*/, Color.FromArgb(153, 102, 204)/*Purple*/, Color.FromArgb(245, 245, 245)/*White*/  };
            ComboBox[,] cbc = new ComboBox[4, 4]; ComboBox[,] cbt = new ComboBox[4, 4]; Button[,] b2 = new Button[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    String[] colset = { "2", "1" };
                    try
                    {
                        colset = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[i * 4 + j + 1].Split(' ');
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        cbc[j, i] = new ComboBox();
                        cbc[j, i].Name = (i * 4 + j).ToString();
                        cbc[j, i].Size = new System.Drawing.Size(100, 20);
                        cbc[j, i].Location = new Point(j * 105 + 30, i * 75 + 30);
                        cbc[j, i].Items.AddRange(colorsNames);
                        cbc[j, i].DrawItem += cb_DrawItem;
                        cbc[j, i].DrawMode = DrawMode.OwnerDrawFixed;
                        cbc[j, i].DropDownStyle = ComboBoxStyle.DropDownList;
                        colorc.Controls.Add(cbc[j, i]); cbc[j, i].BringToFront();
                        cbc[j, i].SelectedIndex = Convert.ToInt32(colset[1]) - 1;
                        cbc[j, i].SelectedIndexChanged += cb_Selected;
                    }
                    catch (Exception) { cbc[j, i].SelectedIndex = 0; cbc[j, i].SelectedIndexChanged += cb_Selected; }
                    try
                    {
                        cbt[j, i] = new ComboBox();
                        cbt[j, i].Name = (i * 4 + j).ToString();
                        cbt[j, i].Size = new System.Drawing.Size(100, 20);
                        cbt[j, i].Location = new Point(j * 105 + 30, i * 75 + 80);
                        cbt[j, i].Items.AddRange(typesNames);
                        cbt[j, i].DropDownStyle = ComboBoxStyle.DropDownList;
                        colorc.Controls.Add(cbt[j, i]); cbc[j, i].BringToFront();
                        cbt[j, i].SelectedIndex = Convert.ToInt32(colset[0]);
                        cbt[j, i].SelectedIndexChanged += cb_Selected;
                    }
                    catch (Exception) { cbt[j, i].SelectedIndex = 2; cbt[j, i].SelectedIndexChanged += cb_Selected; }
                    try
                    {
                        b2[j, i] = new Button();
                        b2[j, i].Name = "b" + (i * 4 + j).ToString();
                        b2[j, i].Size = new System.Drawing.Size(100, 70);
                        b2[j, i].Location = new Point(j * 105 + 30, i * 75 + 30);
                        colorc.Controls.Add(b2[j, i]);
                        Func.button_update(colset[1], b2[j, i], colset[0]);
                    }
                    catch (Exception) { Func.button_update("1", b2[j, i], "2"); }
                }
            }
            void cb_DrawItem(object s, DrawItemEventArgs f)
            {
                using (Brush br = new SolidBrush(colors[f.Index]))
                {
                    f.Graphics.FillRectangle(br, f.Bounds);
                }
            }
            void cb_Selected(object es, EventArgs args)
            {
                String[] Lines = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt"); int num = Convert.ToInt32((es as ComboBox).Name);
                if (Lines.Length < 17)
                {
                    String[] Lines2 = new String[17];
                    Array.Copy(Lines, Lines2, Lines.Length);
                    Lines = new String[17]; Array.Copy(Lines2, Lines, 17);
                }
                Lines[num + 1] = cbt[num % 4, num / 4].SelectedIndex.ToString() + " " + (cbc[num % 4, num / 4].SelectedIndex + 1).ToString();
                Func.button_update((cbc[num % 4, num / 4].SelectedIndex + 1).ToString(), b[num % 4, num / 4], cbt[num % 4, num / 4].SelectedIndex.ToString());
                Func.button_update((cbc[num % 4, num / 4].SelectedIndex + 1).ToString(), b2[num % 4, num / 4], cbt[num % 4, num / 4].SelectedIndex.ToString());
                File.WriteAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt", Lines);
            }
            void coloc_Closed(object es, FormClosedEventArgs args)
            {
                if (com.IsOpen)
                {
                    String message = "";
                    com.Write("ColorChange");
                    Thread.Sleep(sleepTime);
                    for (int i = 0; i < 16; i++)
                    {
                        try
                        {
                            String[] colset = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[i + 1].Split(' ');
                            com.Write(colset[0] + colset[1]);
                        }
                        catch (Exception)
                        {
                            com.Write("20");
                        }
                        Thread.Sleep(sleepTime);
                    }
                    
                }

            }
            colorc.ShowDialog();
        }
        
        private void addmodb3_Click(object sender, EventArgs e)
        {
            if (modulesCounter < 6)
            {
                cm.Icon = this.Icon;
                cm.ShowDialog();
            }
            else MessageBox.Show("Одновременно открытыми могут быть только 6 модулей", "Отказ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void загрузитьВМодульToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            загрузитьВМодульToolStripMenuItem.DropDownItems.Clear();
            foreach (FormC form in modules)
            {
                if (form.Text != "")
                {
                    ToolStripMenuItem tsi = new ToolStripMenuItem();
                    tsi.Text = "На " + form.Text;
                    tsi.Click += moduleSaveClick;
                    загрузитьВМодульToolStripMenuItem.DropDownItems.Add(tsi);
                }
            }
        }
        private void moduleSaveClick(object sender, EventArgs e)
        {
            if (com.IsOpen)
                foreach (FormC form in modules)
                {
                    if (form.Text == (sender as ToolStripMenuItem).Text.Substring(3, (sender as ToolStripMenuItem).Text.Length-3))
                    {
                        com.Write("write_midiinfo" + form.Text);
                        Thread.Sleep(sleepTime);
                        com.Write((form.Controls.Count+1).ToString());
                        Thread.Sleep(sleepTime);
                        com.Write(channel.ToString());
                        Thread.Sleep(sleepTime);
                        foreach (Control ctrl in form.Controls)
                        {
                            com.Write(ctrl.Name+"_"+ctrl.Text);
                            Thread.Sleep(sleepTime);
                        }
                    }
                }
        }

        private void загрузитьВКонтроллерToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (com.IsOpen)
            {
                String []message=new string[59]; message[0]=channel.ToString();//"write_midiinfoA1"
                for (int i = 1; i < 17; i++)
                {
                    message[i]= this.Controls["tb" + (i).ToString()].Text;
                }
                for (int i = 17; i < 27; i++)
                {
                    message[i] = this.Controls["addtb" + (i -16).ToString()].Text;
                }
                for (int i = 27; i < 43; i++)
                {
                    try
                    {
                        String[] colset = File.ReadAllLines("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile + ".pmt")[i -26].Split(' ');
                        message[27 + (i - 27) * 2] = colset[0]; message[27 + (i - 27) * 2 + 1]= colset[1];
                    }
                    catch (Exception) {
                        message[27 + (i - 27) * 2] = "2"; message[27 + (i - 27) * 2 + 1]="0";
                    }
                }
                com.Write("write_midiinfoA1");
                Thread.Sleep(sleepTime);
                for (int i = 0; i < 15; i++)
                {
                    String messageStr="";
                    for (int j = 0; j < 4; j++)
                        if (!((i == 14) && (j == 3))) messageStr = messageStr + message[i * 4 + j] + "_";
                        com.Write(messageStr);
                    Thread.Sleep(sleepTime);
                }
                
            }
        }
        public void Form1Closed(object sender, FormClosingEventArgs e)
        {
            formClosing = true;
        }

        private void загрузитьВМодульToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}