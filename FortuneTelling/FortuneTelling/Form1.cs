using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using timerThreading = System.Threading.Timer;

namespace FortuneTelling
{
    public partial class Form1 : Form
    {
        private timerThreading timerStartProgram;

        void StartLoadProgram(object sender, EventArgs e)
        {
            TimerStartProgram.Start();
            background1.Dispose();
            labelClickStartProgram.Dispose();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelClickStartProgram.Click += StartLoadProgram;
            background1.Click += StartLoadProgram;
        }

        private void TimerStartProgram_Tick(object sender, EventArgs e)
        {
            if (Img.indexImgOne < Img.IndexLastCover)
            {
                Img.changePath();
                //แก้เวลาเปลี่ยนรูปแล้วเกิดสีขาวขึ้น
                if (Img.indexImgOne % 2 == 0)
                {
                    background2.BackgroundImage = Image.FromFile(Img.path);
                    background2.BringToFront();
                }
                else
                {
                    background3.BackgroundImage = Image.FromFile(Img.path);
                    background3.BringToFront();
                }
            }
            else
                TimerStartProgram.Dispose();
        }





        private TabControl tabControl;
        private PictureBox[] pictureBoxes;

    private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void alertforMe(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Name);
            }
        }
        private void pictureBox78_Click(object sender, EventArgs e)
        {
            Tarot tarot = new Tarot();
            try
            {
                for (int i = 1; i <= 78; i++)
                {
                    string pictureBoxName = "pictureBox" + i;
                    PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                    if (pictureBox != null)
                    {
                        pictureBox.Click += new EventHandler(alertforMe);
                        int X = int.Parse(Tarot.TarotData[i - 1][2]);
                        int Y = int.Parse(Tarot.TarotData[i - 1][3]);
                        pictureBox.Location = new Point(X, Y);
                    }
                }
            }
            catch
            {
                Message.messageError();
            }
        }

        private void pictureBox79_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Location.ToString());
            }
        }

        private void pictureBox84_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Location.ToString());
            }
        }

        private void pictureBox82_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Location.ToString());
            }
        }

        private void pictureBox83_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Location.ToString());
            }
        }

        private void pictureBox80_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Location.ToString());
            }
        }
    }
}