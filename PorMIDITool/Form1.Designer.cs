using System;
namespace PorMIDITool
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception) { }
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.timerColor = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.октаваToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.поднятьНа1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.опуститьНа1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.профильToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьВКонтроллерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьВМодульToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.addmodb3 = new System.Windows.Forms.Button();
            this.colorb = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 346);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Лог на выход:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 285);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(358, 346);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Лог на вход:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(564, 375);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = " ";
            // 
            // timerColor
            // 
            this.timerColor.Interval = 1000;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.октаваToolStripMenuItem,
            this.профильToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(717, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // октаваToolStripMenuItem
            // 
            this.октаваToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.поднятьНа1ToolStripMenuItem,
            this.опуститьНа1ToolStripMenuItem});
            this.октаваToolStripMenuItem.Name = "октаваToolStripMenuItem";
            this.октаваToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.октаваToolStripMenuItem.Text = "Октава";
            // 
            // поднятьНа1ToolStripMenuItem
            // 
            this.поднятьНа1ToolStripMenuItem.Name = "поднятьНа1ToolStripMenuItem";
            this.поднятьНа1ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.поднятьНа1ToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.поднятьНа1ToolStripMenuItem.Text = "Поднять на 1";
            this.поднятьНа1ToolStripMenuItem.Click += new System.EventHandler(this.поднятьНа1ToolStripMenuItem_Click);
            // 
            // опуститьНа1ToolStripMenuItem
            // 
            this.опуститьНа1ToolStripMenuItem.Name = "опуститьНа1ToolStripMenuItem";
            this.опуститьНа1ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.опуститьНа1ToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.опуститьНа1ToolStripMenuItem.Text = "Опустить на 1";
            this.опуститьНа1ToolStripMenuItem.Click += new System.EventHandler(this.опуститьНа1ToolStripMenuItem_Click);
            // 
            // профильToolStripMenuItem
            // 
            this.профильToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.загрузитьВКонтроллерToolStripMenuItem,
            this.загрузитьВМодульToolStripMenuItem});
            this.профильToolStripMenuItem.Name = "профильToolStripMenuItem";
            this.профильToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.профильToolStripMenuItem.Text = "Профиль";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.toolStripMenuItem1.Size = new System.Drawing.Size(248, 22);
            this.toolStripMenuItem1.Text = "Сохранить";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // загрузитьВКонтроллерToolStripMenuItem
            // 
            this.загрузитьВКонтроллерToolStripMenuItem.Name = "загрузитьВКонтроллерToolStripMenuItem";
            this.загрузитьВКонтроллерToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.загрузитьВКонтроллерToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.загрузитьВКонтроллерToolStripMenuItem.Text = "Загрузить в контроллер";
            this.загрузитьВКонтроллерToolStripMenuItem.ToolTipText = "Требуется для использования как MIDI устройства с теми же настройками";
            this.загрузитьВКонтроллерToolStripMenuItem.Click += new System.EventHandler(this.загрузитьВКонтроллерToolStripMenuItem_Click_1);
            // 
            // загрузитьВМодульToolStripMenuItem
            // 
            this.загрузитьВМодульToolStripMenuItem.Name = "загрузитьВМодульToolStripMenuItem";
            this.загрузитьВМодульToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.загрузитьВМодульToolStripMenuItem.Text = "Загрузить в модуль";
            this.загрузитьВМодульToolStripMenuItem.Click += new System.EventHandler(this.загрузитьВМодульToolStripMenuItem_Click);
            this.загрузитьВМодульToolStripMenuItem.MouseEnter += new System.EventHandler(this.загрузитьВМодульToolStripMenuItem_MouseEnter);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(129, 346);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(223, 64);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(466, 346);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(223, 64);
            this.richTextBox2.TabIndex = 9;
            this.richTextBox2.Text = "";
            // 
            // addmodb3
            // 
            this.addmodb3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addmodb3.BackgroundImage")));
            this.addmodb3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.addmodb3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addmodb3.Location = new System.Drawing.Point(362, 412);
            this.addmodb3.Name = "addmodb3";
            this.addmodb3.Size = new System.Drawing.Size(69, 18);
            this.addmodb3.TabIndex = 11;
            this.addmodb3.UseVisualStyleBackColor = true;
            this.addmodb3.Click += new System.EventHandler(this.addmodb3_Click);
            // 
            // colorb
            // 
            this.colorb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.colorb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorb.Location = new System.Drawing.Point(362, 27);
            this.colorb.Name = "colorb";
            this.colorb.Size = new System.Drawing.Size(53, 16);
            this.colorb.TabIndex = 13;
            this.colorb.UseVisualStyleBackColor = true;
            this.colorb.Click += new System.EventHandler(this.colorb_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(717, 442);
            this.Controls.Add(this.colorb);
            this.Controls.Add(this.addmodb3);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PortableMIDITool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem октаваToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem профильToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поднятьНа1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem опуститьНа1ToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem загрузитьВКонтроллерToolStripMenuItem;
        private System.Windows.Forms.Button addmodb3;
        private System.Windows.Forms.Button colorb;
        private System.Windows.Forms.Timer timerColor;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem загрузитьВМодульToolStripMenuItem;
    }
}

