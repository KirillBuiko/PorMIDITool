using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PorMIDITool
{
    public partial class FormC : Form
    {
        public String about;
        public String path;
        public FormC()
        {
            InitializeComponent();
        }

        private void FormC_Load(object sender, EventArgs e)
        {

        }

        private void оМодулеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(about, "О модуле", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog modChoose = new OpenFileDialog();
            modChoose.Filter = "Файлы модулей|*.pmtm"; String path = "";
            if (modChoose.ShowDialog() == DialogResult.OK) path = modChoose.FileName;
            String[] file = File.ReadAllText(path).Split(';');

            for (int i = 0; i < file.Length; i++)
            {
                try
                {
                    String Line = file[i];
                    String[] SLine = Line.Split(' ');
                    if (SLine[0] == "\r\ntxt")
                    {
                        String name = Line.Split(' ')[1];
                        bool flag = false;
                        for (int j = 1; j < SLine.Length; j++)
                            if (SLine[j] == "text")
                            {
                                 SLine[j + 1]=this.Controls.Find(name, false)[0].Text; flag = true; break;
                            }
                        if (!flag)
                        {
                            String[] SLine2 = new String[SLine.Length + 2];
                            Array.Copy(SLine, SLine2, SLine.Length);
                            SLine = new String[SLine2.Length];
                            Array.Copy(SLine2, SLine, SLine.Length);
                            SLine[SLine.Length - 2] = "text"; SLine[SLine.Length - 1] = this.Controls.Find(name, false)[0].Text;
                        }String NewLine="";
                        for(int k = 0; k < SLine.Length; k++)
                        {
                            if (k != 0)
                                NewLine = NewLine + " " + SLine[k];
                            else NewLine = NewLine + SLine[k];
                        }
                        file[i] = NewLine;
                    }
                }
                catch (Exception) { }
            }
            String NewFile = "";
            for (int k = 0; k < file.Length; k++)
            {
                if (k != 0)
                    NewFile = NewFile + ";" + file[k];
                else NewFile = file[k];
            }File.WriteAllText(path, NewFile);
        }
    }
}
