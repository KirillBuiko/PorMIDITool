using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace PorMIDITool
{
    class Func
    {
        public static FormC Constructor(String path, FormC result)
        {

            String[] file = File.ReadAllText(path).Split(';');
            for(int i = 0; i < file.Length; i++)
            {   if(file[i]!="")
                if (file[i].Substring(0,2)=="\r\n") file[i] = file[i].Substring(2, file[i].Length-2);
            }
            foreach (String Line in file)
            {
                try
                {
                    String[] SLine = Line.Split(' ');
                    int ccount = 0;
                    switch (SLine[ccount])
                    {
                        case "name":
                            result.Text = SLine[++ccount];
                            break;
                        case "btn":
                            Button btn = new Button();
                            btn.ForeColor = Color.Black;
                            btn = (ControlConstr(btn, SLine[++ccount], Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount])) as Button);
                            while (ccount + 1 < SLine.Length) switch (SLine[++ccount])
                            {
                                case "fontsize":
                                    btn.Font = new Font(btn.Font.Name, Convert.ToInt32(SLine[++ccount]));
                                    break;
                                case "text":
                                    btn.Text = SLine[++ccount];
                                    break;
                            }
                            result.Controls.Add(btn);
                            break;
                        case "cmb":
                            ComboBox cmb = new ComboBox();
                            cmb.ForeColor = Color.Black;
                            cmb = (ControlConstr(cmb, SLine[++ccount], Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount])) as ComboBox);
                            while (ccount + 1 < SLine.Length) switch (SLine[++ccount])
                            {
                                case "fontsize":
                                    cmb.Font = new Font(cmb.Font.Name, Convert.ToInt32(SLine[++ccount]));
                                    break;
                                case "text":
                                    cmb.Text = SLine[++ccount];
                                    break;
                            }
                            result.Controls.Add(cmb);
                            break;
                        case "lbl":
                            Label lbl = new Label();
                            lbl.ForeColor = Color.Black;
                            lbl = (ControlConstr(lbl, SLine[++ccount], Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount])) as Label);
                            while (ccount+1 < SLine.Length)switch (SLine[++ccount])
                            {
                                case "fontsize":
                                    lbl.Font = new Font(lbl.Font.Name, Convert.ToInt32(SLine[++ccount]));
                                    break;
                                case "text":
                                    lbl.Text = SLine[++ccount];
                                    break;
                            }
                            result.Controls.Add(lbl);
                            break;
                        case "txt":
                            TextBox txt = new TextBox();
                            txt = (ControlConstr(txt, SLine[++ccount], Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount])) as TextBox);
                            while (ccount + 1 < SLine.Length) switch (SLine[++ccount])
                            {
                                case "fontsize":
                                    txt.Font = new Font(txt.Font.Name, Convert.ToInt32(SLine[++ccount]));
                                    break;
                                case "readonly":
                                    txt.ReadOnly = true;
                                    break;
                                case "text":
                                    txt.Text = SLine[++ccount];
                                    break;
                            }
                            result.Controls.Add(txt);
                            break;
                        case "about":
                            for(int i=1;i<SLine.Length;i++)
                            result.about = result.about+SLine[i]+" ";
                            break;
                        case "form":
                            result.Size = new Size(Convert.ToInt32(SLine[++ccount]), Convert.ToInt32(SLine[++ccount]));
                            break; 
                        case "for":
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }

            return result;
        }
        private static Control ControlConstr(Control ctrl, String name, int width, int height, int posx, int posy)
        {
            ctrl.Name = name;
            ctrl.Size = new Size(width, height);
            ctrl.Location = new Point(posx, posy);
            return ctrl;
        }
    }
}
