using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using NAudio.Midi;

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
        SerialPort com;
        GroupBox gb;
        FormSettings f2;
        ChooseModule cm;
        FileStream profiles;
        FileStream conf;
        Thread SerialTask;
        ColorDialog MyDialog;
        FormC[] modules;

        byte[] by; string[] profstr; object sd; TextBox[,] tb; TextBox[,] addptb;

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
            profiles.Close();
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + channel + ".pmt", "-1 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 20 21 22 23 24 25 26 27 28 29");
            profstr = new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29" };
        }
        void prof_Click(object s, EventArgs args)
        {
            profiles.Close();
            profile = Convert.ToInt32((s as ToolStripMenuItem).Text);
            профильToolStripMenuItem.Text = "Профиль (" + profile.ToString() + ")";
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\conf.pmt", profile.ToString());
            profiles = new FileStream("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile.ToString() + ".pmt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            by = new byte[profiles.Length]; profiles.Read(by, 0, by.Length);
            profstr = Encoding.UTF8.GetString(by).Split(new char[] { ' ' });
            try
            {
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modules = new FormC[6];
            for (int i = 0; i < 6; i++)
            {
                modules[i] = new FormC();
            }
            ToolTip t = new ToolTip();t.SetToolTip(addmodb3, "Добавить подключённый модуль"); t.SetToolTip(colorb, "Выбрать цвет подсветки");
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
            profiles = new FileStream("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + fstr + ".pmt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            by = new byte[profiles.Length]; profiles.Read(by, 0, by.Length);
            profstr = Encoding.UTF8.GetString(by).Split(new char[] { ' ' });
            try
            {
                int i = 1;
                for (; i < 27; i++)
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
                    tb[j, i].Location = new Point(j * 65 + 53, i * 73 + 75);
                    Controls.Add(tb[j, i]);
                    tb[j, i].BringToFront();
                }
            }


            Button[,] b = new Button[4, 4];
            ////////////////////////////////////k=15////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b[j, i] = new Button();
                    b[j, i].Tag = k++;
                    b[j, i].Name = "button" + (k - 16).ToString();
                    b[j, i].Size = new System.Drawing.Size(65, 70);
                    b[j, i].Text = (k - 16).ToString();
                    b[j, i].Location = new Point(j * 65 + 30, i * 73 + 30);
                    b[j, i].MouseDown += new MouseEventHandler(this.button_MouseDown);
                    b[j, i].MouseUp += new MouseEventHandler(this.button_MouseUp);
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
                    addptb[i, j].Size = new System.Drawing.Size(40, 20);
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
            //f2.FormClosing += SettingsClose;
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
                    midiin.Close();
                    midiout.Close();
                }
                catch (NullReferenceException) { }
                try
                {
                    midiin = new MidiIn(f2.deviceIDIn);
                    midiin.MessageReceived += Midiin_MessageReceived;
                    midiin.Start();
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
                    com.BaudRate = 9600;
                    com.PortName = f2.deviceCOM;
                    com.Open();
                    Thread.Sleep(2000);
                    com.Write("I hear you!");
                    SerialTask.Start();
                }
                catch (Exception ex) { MessageBox.Show(ex.HelpLink); }
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
                        modules[i].Show();modules[i].Left = this.Left;modules[i].Top = this.Top + 481;
                        break;
                    }
                }
            }

        }
        public void ModuleClose(object sender, FormClosingEventArgs e) { modulesCounter--; (sender as Form).Text = ""; }

        void SerialTaskR()
        {
            while (true) try
                {
                    if (com.IsOpen)
                    {
                        string[] rbuf;
                        if (com.ReadBufferSize > 0)
                        {
                            rbuf = com.ReadLine().Split(new char[] { ' ' });
                            int note = 0;
                            if (rbuf[0] == "A1")
                            {
                                if (rbuf[1] == "b") note = Convert.ToInt32(this.Controls["tb" + rbuf[2]].Text);
                                if (rbuf[1] == "cc") note = Convert.ToInt32(Controls["addtb" + rbuf[2]].Text);
                            }
                            else
                            {
                                bool flag = false;
                                foreach (FormC form in modules)
                                {
                                    if (rbuf[0] == form.Text)
                                    {
                                        note = Convert.ToInt32(form.Controls.Find(rbuf[2], false)[0].Text); flag = true; break;
                                    }
                                }
                                if (!flag) break;
                            }

                            {
                                if (rbuf[1] == "b")
                                {
                                    //MessageBox.Show(rbuf[2].ToCharArray()[2].ToString());
                                    if (rbuf[3] == ("on" + '\r'))
                                    {
                                        midi_Send(note, "9", 127);
                                    }
                                    else midi_Send(note, "8", 127);
                                }
                                if (rbuf[1] == "cc")
                                {
                                    midi_Send(note, "B", Convert.ToInt32(rbuf[2].Substring(0, rbuf[4].Length - 1)));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
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
            midi_Send(note, "8", 127);
        }
        void midi_Send(int note, string type, int velocity)
        {
            String strnote;
            if (note < 16) strnote = "0" + note.ToString("X");
            else strnote = note.ToString("X");
            String vel = velocity.ToString("X");
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
                    /*case "B":
                        richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = DateTime.Now.ToString().Substring(11, DateTime.Now.ToString().Length - 11) + ", " + note + " CC, " + velocity + '\n' + richTextBox1.Text; });
                        break;*/
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

            //midiout.Send(e.RawMessage);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                midiout.Dispose();
                midiin.Dispose();
                com.Close();
                SerialTask.Abort();
            }
            catch (Exception) { }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            for (int i = 1; i < 17; i++)
            {
                save += " " + this.Controls["tb" + (i).ToString()].Text;
            }
            for (int i = 1; i < 11; i++)
            {
                save += " " + this.Controls["addtb" + (i).ToString()].Text;
            }
            profiles.Close();
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile.ToString() + ".pmt", save);
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
            colorc.Icon = this.Icon;
            colorc.Text = "Определение цветов";
            Button[,] b = new Button[4, 4];
            ////////////////////////////////////k=15////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b[j, i] = new Button();
                    b[j, i].Name = "colorb" + (i * 4 + j).ToString();
                    b[j, i].Size = new System.Drawing.Size(100, 70);
                    b[j, i].Text = (i*4+j+1).ToString();
                    b[j, i].Location = new Point(j * 105 + 30, i * 75 + 30);
                    colorc.Controls.Add(b[j, i]);
                }
            }
            string[] colorsNames = { "Красный", "Зелёный", "Синий","Жёлтый","Голубой","Сереневый","Белый"};
            Color[] colors = { Color.FromArgb(252, 38, 70)/*Red*/, Color.FromArgb(112,186,60)/*Green*/, Color.FromArgb(65, 105, 225)/*Blue*/, Color.FromArgb(225, 204, 79)/*Yellow*/, Color.FromArgb(92, 154, 204)/*Strange Blue*/, Color.FromArgb(153, 102, 204)/*Purple*/, Color.FromArgb(245, 245, 245)/*White*/  };
            ComboBox[,] cb = new ComboBox[4, 4];
            ////////////////////////////////////k=15////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cb[j, i] = new ComboBox();
                    cb[j, i].Name = "colorcb" + (i * 4 + j).ToString();
                    cb[j, i].Size = new System.Drawing.Size(100, 20);
                    cb[j, i].Location = new Point(j * 105 + 30, i * 75 + 30);
                    cb[j, i].Items.AddRange(colorsNames);
                    cb[j, i].DrawItem += cb_DrawItem;
                    cb[j, i].DrawMode = DrawMode.OwnerDrawFixed;
                    
                    colorc.Controls.Add(cb[j, i]);cb[j, i].BringToFront();
                }
            }
            string[] typesNames = {"Включён всегда","Включён при нажатие","Включён при нажатие"};
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cb[j, i] = new ComboBox();
                    cb[j, i].Name = "colorcb" + (i * 4 + j).ToString();
                    cb[j, i].Size = new System.Drawing.Size(100, 20);
                    cb[j, i].Location = new Point(j * 105 + 30, i * 75 + 80);
                    cb[j, i].Items.AddRange(colorsNames);
                    cb[j, i].DrawItem += cb_DrawItem;
                    cb[j, i].DrawMode = DrawMode.OwnerDrawFixed;

                    colorc.Controls.Add(cb[j, i]); cb[j, i].BringToFront();
                }
            }
            void cb_DrawItem(object s, DrawItemEventArgs f)
            {
                using (Brush br = new SolidBrush(colors[f.Index]))
                {
                    f.Graphics.FillRectangle(br, f.Bounds);
                }
            }
            colorc.ShowDialog();
        }

        private void addmodb2_Click(object sender, EventArgs e)
        {

        }

        private void addmodb3_Click(object sender, EventArgs e)
        {
            if (modulesCounter < 6)
            {
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
                    if (form.Text == (sender as ToolStripMenuItem).Text)
                    {
                        com.Write("write_midiinfo" + form.Text);String message = "";
                        foreach (Control ctrl in form.Controls)
                        {
                            message=message+ctrl.Name+"&"+ctrl.Text+"_";
                        }com.Write(message);
                    }
                }
        }

        private void загрузитьВКонтроллерToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (com.IsOpen)
            {
                com.Write("write_midiinfoA1");String message="";
                for (int i = 0; i < 16; i++)
                {
                    message = message + this.Controls["tb" + (i + 1).ToString()].Text+"_";
                }
                for (int i = 0; i < 10; i++)
                {
                    message = message + this.Controls["addtb" + (i + 1).ToString()].Text + "_";
                }com.Write(message);
            }
        }

        private void загрузитьВМодульToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}