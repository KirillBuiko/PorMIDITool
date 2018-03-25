using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
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
        int channel=0;
        int k=0;
        int octave;
        int profile;
        GroupBox gb;
        Form2 f2;
        FileStream profiles;
        FileStream conf;


        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory("c:\\ProgramData\\PorMIDITool\\profiles");
            conf = new FileStream("c:\\ProgramData\\PorMIDITool\\conf.pmt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] by=new byte[conf.Length]; conf.Read(by,0,by.Length);conf.Close();
            string fstr = Encoding.UTF7.GetString(by);//.Split(new char[] { ' ' });
            try
            {
                if (Convert.ToInt32(fstr) < 1 || Convert.ToInt32(fstr) > 12) { fstr = "1"; }
            }
            catch (Exception) { fstr = "1"; }
            profile = Convert.ToInt16(fstr);
            profiles = new FileStream("c:\\ProgramData\\PorMIDITool\\profiles\\profile"+fstr+".pmt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            by = new byte[profiles.Length]; profiles.Read(by, 0, by.Length);
            string[] profstr = Encoding.UTF8.GetString(by).Split(new char[] { ' ' });
            try
            { int i = 1;
                for (; i < 27; i++)
                {
                    if (Convert.ToInt32(profstr[i]) > 127|| Convert.ToInt32(profstr[i])<0) { profstr[i] = i.ToString(); }
                }
            }
            catch (Exception) { MessageBox.Show("Проблема с открытием профиля.\nОн будет восстановлен.","",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                profiles.Close();
                File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + fstr + ".pmt","-1 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 0 1 2 3 4 5 6 7 8 9");
                profstr = new string[] { "-1", "0", "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","0","1","2","3","4","5","6","7","8","9"};
            }
            octave = Convert.ToInt32(profstr[0]);
            профильToolStripMenuItem.Text = "Профиль ("+profile.ToString()+")";
            TextBox[,] tb = new TextBox[4, 4];
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

            void tb_Press(object s, KeyPressEventArgs args)
            {
                if ((Char.IsDigit(args.KeyChar)) || args.KeyChar == 8)
                {
                    if (Char.IsDigit(args.KeyChar)&&Convert.ToInt32((s as TextBox).Text+args.KeyChar) > 127)
                    {
                        Controls["tb" + ((Convert.ToInt32((s as TextBox).Tag))+1).ToString()].Text = "127";
                        args.Handled = true;
                    }
                    return;
                }
                else args.Handled = true;
                    
                
            }
            Button [,]b = new Button[4,4];
            ////////////////////////////////////k=15////////////////////////
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j<4; j++)
                {
                    b[j, i] = new Button();
                    b[j, i].Tag = k++;
                    b[j, i].Name = "button" + (k - 16).ToString();
                    b[j, i].Size = new System.Drawing.Size(65, 70);
                    b[j, i].Text = (k-16).ToString();
                    b[j, i].Location = new Point(j*65+30, i*73+30);
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
            TextBox[,] addptb = new TextBox[2, 5];
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
            RadioButton []oct = new RadioButton[11];
            gb = new GroupBox();
            gb.Location = new System.Drawing.Point(0, -10);
            gb.Size = new System.Drawing.Size(20,150);
            for (int j = 0; j < 11; j++)
            {
                oct[j] = new RadioButton();
                oct[j].Text = (j-1).ToString();
                oct[j].Name = "octrb" + (j + 1).ToString();
                oct[j].Click += new EventHandler(oct_Click);
                oct[j].Location = new System.Drawing.Point(10,j*20+10);
                if(octave == (j-1))oct[j].Checked = true;
                gb.Controls.Add(oct[j]);
            }
            октаваToolStripMenuItem.DropDownItems.Insert(0, new ToolStripControlHost(gb));
            object sd = new RadioButton();
            oct[octave + 1].Checked = true;
            ToolStripMenuItem[] prof = new ToolStripMenuItem[12];
            for (int j = 0; j < 12; j++)
            {
                prof[j] = new ToolStripMenuItem();
                prof[j].Text = (j +1).ToString();
                prof[j].Click += new EventHandler(prof_Click);
                prof[j].ShortcutKeys = ((Keys)(Keys.Control|(Keys)j+49));
                профильToolStripMenuItem.DropDownItems.Add(prof[j]);
            }
            prof[9].ShortcutKeys = ((Keys)(Keys.Control | (Keys)48));
            prof[10].ShortcutKeys = ((Keys)(Keys.Control | (Keys)189));
            prof[11].ShortcutKeys = ((Keys)(Keys.Control | (Keys)187));
            void prof_Click(object s, EventArgs args)
            {
                profiles.Close();
                profile = Convert.ToInt32((s as ToolStripMenuItem).Text);
                профильToolStripMenuItem.Text = "Профиль (" + profile.ToString() + ")";
                File.WriteAllText("c:\\ProgramData\\PorMIDITool\\conf.pmt",profile.ToString());
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
                    MessageBox.Show("Проблема с открытием профиля.\nОн будет восстановлен.", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    profiles.Close();
                    File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile" + profile.ToString() + ".pmt", "-1 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 0 1 2 3 4 5 6 7 8 9");
                    profstr = new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
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
            
        }
        void oct_Click(object s, EventArgs args)
            {
            octave = Convert.ToInt32((s as RadioButton).Text);
            (gb.Controls["octrb"+(octave+2).ToString()]as RadioButton).Checked = true;
                for (int i = 0; i < 16; i++)
                {
                    this.Controls["tb" + (i+1).ToString()].Text = ((Convert.ToInt32((s as RadioButton).Text) + 1) * 12 + i).ToString();
                    if((Convert.ToInt32((s as RadioButton).Text) + 1) * 12 + i>127) this.Controls["tb" + (i + 1).ToString()].Text = "127";
                }
            }
        private void Form1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            f2 = new Form2();
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
            f2.FormClosing += Open;
            f2.ShowDialog();
        }
        public void Open(object sender, FormClosingEventArgs e)
        {
            if (f2.open == true)
            {
                try
                {
                    midiin.Close();
                    midiout.Close();
                }
                catch (NullReferenceException) { }
                midiin = new MidiIn(f2.deviceIDIn);
                midiout = new MidiOut(f2.deviceIDOut);
                channel = f2.channel;
                midiin.MessageReceived += Midiin_MessageReceived;
                midiin.Start();
            } else Close();
        }
        void button_MouseDown(object s, MouseEventArgs args)
        {
            int tag = Convert.ToInt32((s as Button).Tag);
            richTextBox1.Text = DateTime.Now.ToString().Substring(11,8) + ", " + Convert.ToInt32(this.Controls["tb" + (tag - 15).ToString()].Text) + " Note On, " + "127" + '\n' + richTextBox1.Text;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            midi_Send(s, args, "9", 127);
        }
        void button_MouseUp(object s, MouseEventArgs args)
        {
            int tag = Convert.ToInt32((s as Button).Tag);
            richTextBox1.Text = DateTime.Now.ToString().Substring(11, 8) + ", " + Convert.ToInt32(this.Controls["tb" + (tag - 15).ToString()].Text) + " Note Off, " + "127" + '\n' + richTextBox1.Text;
            midi_Send(s, args, "8", 127);
        }
        void midi_Send(object s, EventArgs args,string type, int velocity)
        {
            int tag = Convert.ToInt32((s as Button).Tag);
            int note = Convert.ToInt32(this.Controls["tb"+ (tag-15).ToString()].Text);
            String strnote;
            if (note < 16) strnote = "0"+note.ToString("X");
            else strnote = note.ToString("X");
            String vel = velocity.ToString("X");
            String channel = this.channel.ToString("X");
            midiout.Send(Convert.ToInt32(vel+strnote + type + channel,16));
        }

        private void Midiin_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            String midimes = e.RawMessage.ToString("X");String type = "";int note = 0; int velocity = 0;
            if(midimes!="")type = midimes.Substring(midimes.Length-2);
            while (midimes.Length < 6) midimes.Insert(0, "0");
            note = Convert.ToInt32(midimes.Substring(midimes.Length - 4, 2), 16);
            velocity = note = Convert.ToInt32(midimes.Substring(midimes.Length - 6, 2), 16);
            if (type.Equals("80"))
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", " + note.ToString() + " Note Off, " + velocity.ToString() + '\n' + richTextBox2.Text; }));
            if (type.Equals("90"))
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", " + note.ToString() + " Note On, " + velocity.ToString() + '\n' + richTextBox2.Text; }));
            if (type.Equals("B0"))//MessageBox.Show(midimes,"",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                this.Invoke(new EventHandler(delegate { richTextBox2.Text = DateTime.Now.ToString().Substring(11, 8) + ", CC " + note.ToString() + ", " + velocity.ToString() + '\n' + richTextBox2.Text; }));

            //midiout.Send(e.RawMessage);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            { 
                midiout.Dispose();
                midiin.Dispose();
            }
            catch (NullReferenceException) { }
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
            oct_Click(s,EventArgs.Empty);
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
            string save =""; save += octave.ToString();
            for (int i = 1; i < 17; i++)
            {
                save += " "+this.Controls["tb" + (i).ToString()].Text;
            }
            for (int i = 1; i < 11; i++)
            {
                save += " " + this.Controls["addtb" + (i).ToString()].Text;
            }
            profiles.Close();
            File.WriteAllText("c:\\ProgramData\\PorMIDITool\\profiles\\profile"+profile.ToString()+".pmt", save);
        }
    }
}
