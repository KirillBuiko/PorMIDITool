using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PorMIDITool
{
    public partial class ChooseModule : Form
    {
        public bool open = false;
        public String chosenMod="";
        public ChooseModule()
        {
            InitializeComponent();
        }

        private void ChooseModule_Open(object sender, EventArgs e)
        {
            open = false;
            comboBox1.Items.Clear();
            if (!Directory.Exists("C:/ProgramData/PorMIDITool/modules"))
            {
                MessageBox.Show("Директория с модулями отсутствует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            foreach (var file in Directory.GetFiles("C:/ProgramData/PorMIDITool/modules", "*.pmtm"))
            {
                String[] input = File.ReadAllText(file).Split(';');
                foreach(String Line in input)
                if (Line.Split(' ')[0] == "name")
                {
                    bool flag = false;
                    foreach (String comp in comboBox1.Items)
                    {
                        if (comp == Line.Split(' ')[1]) flag = true;
                    }
                        if (!flag)
                        {
                            comboBox1.Items.Add(Line.Split(' ')[1]);
                            if (Line.Split(' ')[1] == chosenMod)
                                comboBox1.SelectedItem=chosenMod;
                        }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog modChoose = new OpenFileDialog();
            modChoose.Filter = "Файлы модулей|*.pmtm";
            if (modChoose.ShowDialog() == DialogResult.OK)
            {
                string newMod;
                String[] input = File.ReadAllText(modChoose.FileName).Split(';');
                foreach (String Line in input) 
                {
                    if (Line.Split(' ')[0] == "name"|| Line.Split(' ')[0] == "\r\nname")
                    {
                        newMod = Line.Split(' ')[1];
                        bool flag = false;
                        foreach (String comp in comboBox1.Items)
                        {
                            if (comp == newMod) flag = true;
                        }
                        if (flag)
                        {
                            MessageBox.Show("Модуль с таким именем уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }else comboBox1.Items.Add(newMod);
                        try
                        {
                            File.Copy(modChoose.FileName, "C:/ProgramData/PorMIDITool/modules/"+modChoose.SafeFileName);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Файл с таким именем уже есть в директории", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Имя в модуле нет, либо оно прописано некорректно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var file in Directory.GetFiles("C:/ProgramData/PorMIDITool/modules", "*.pmtm"))
            {
                String[] sr = File.ReadAllText(file).Split(';');
                foreach (String Line in sr)
                {
                    if ((Line.Split(' ')[0] == "name") && (comboBox1.SelectedItem.ToString() == Line.Split(' ')[1]))
                    {
                        this.Text = file;
                        open = true;
                        Close();
                    }
                }
            }
        }

        private void ChooseModule_Load(object sender, EventArgs e)
        {

        }
    }
}
