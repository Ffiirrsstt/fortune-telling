using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using timerThreading = System.Threading.Timer;

namespace FortuneTelling
{
    public partial class Form1 : Form
    {
        private const int speedX1Value = 12, speedX4Value = 3;

        private int speedDelayShuffle = speedX1Value; //เริ่มต้นตั้งค่าให้ความเร็วการสับ คือ 1 เท่า

        private void alertforMe(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                MessageBox.Show(pictureBox.Name);
            }
        }

        private void settingShowBtnSpeedShuffle()
        {
            if (speedDelayShuffle == 10)
                speedX3.Show();
            else
                speedX1.Show();
        }
        private void settingHideBtnSpeedShuffle()
        {
            speedX3.Hide();
            speedX1.Hide();
        }

        private void allCardsExceptLastOne(int[] end)
        {
            for (int i = 1; i <= 77; i++)
            {
                string pictureBoxName = "pictureBox" + i;
                PictureBox pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBoxforFind != null)
                    pictureBoxforFind.Location = new Point(end[0], end[1]);
            }
        }

        private async Task cardMovement(PictureBox pictureBoxforFind, int[] start, int[] end, int step)
        {
            pictureBoxforFind.BringToFront();
            while (start[0] > end[0] || start[1] > end[1])
            {
                if (start[0] > end[0])
                    start[0] -= step;
                if (start[1] > end[1])
                    start[1] -= step;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
        }
        public async Task animatedCardShuffle(int[] start, int[] end, int step, int numberAddforShuffle)
        {
            PictureBox pictureBoxforFind;
            int[] dataStartCopy = new int[] { 0, 0 };
            dataStartCopy[0] = start[0];
            dataStartCopy[1] = start[1];

            settingShowBtnSpeedShuffle(); //แสดงปุ่มปรับความเร็วการสับไพ่ยิบซี

            for (int i = 1; i <= 78; i += numberAddforShuffle)
            {
                string pictureBoxName = "pictureBox" + i;
                pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                //เขียนจัดการเพื่อให้มั่นใจว่าไพ่ทุกใบยกเว้นใบสุดท้ายถูกย้ายไปยังปลายทางทั้งหมด
                if (i == 78 && numberAddforShuffle != 1)
                    allCardsExceptLastOne(end);

                if (pictureBoxforFind != null)
                    await cardMovement(pictureBoxforFind, start, end, step);

                /*จัดการในกรณีที่จำนวนไพ่ที่ต้องสับในแต่ละครั้งมีจำนวนที่นำไปหาร 78 ใบไม่ลงตัว
                มีไพ่ 78 ใบ ถ้าต้องการสับไพ่จำนวน 4 ครั้ง ต้องสับครั้งละ 78/4 (ใช้ฟังก์ชันเพดานได้ 20)
                เท่ากับสับไพ่ 4 ครั้ง สับครั้งละ 20 ใบ 78 - 20 - 20 - 20 = 18
                ครั้งแรก ครั้งสอง และครั้งที่สามสามารถสับทีละ 20 ใบได้ แต่ครั้งสุดท้ายไม่สามารถทำได้
                */
                if ((78 - i) < numberAddforShuffle)
                    numberAddforShuffle = 78 - i;

                start[0] = dataStartCopy[0];
                start[1] = dataStartCopy[1];

                //เมื่อจัดการกับไพ่ใบสุดท้ายหรือใบที่ 78 เสร็จสิ้น ให้จบการทำงานของ loop
                if (i == 78)
                    break;
            }

            settingHideBtnSpeedShuffle(); //ซ่อนปุ่มปรับความเร็วการสับไพ่ยิบซี
        }
        private void data(PictureBox pictureBoxName, int[] start, int[] end, int step)
        {
            if (start[0] < end[0] && start[1] < end[1])
            {
                start[0] += step;
                start[1] += step;
                pictureBoxName.Location = new Point(start[0], start[1]);
            }
            else if (start[0] < end[0] && start[1] >= end[1])
            {
                start[0] += step;
                pictureBoxName.Location = new Point(start[0], start[1]);
            }
            else if (start[0] >= end[0] && start[1] < end[1])
            {
                start[1] += step;
                pictureBoxName.Location = new Point(start[0], start[1]);
            }
        }

        private void sort()
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

        private async void RunTarot() {
            Tarot tarot = new Tarot();
            /*try
            {*/
                int shuffleNumber = 10; //จำนวนครั้งการสับไพ่
                int numberAddforShuffle = (int)Math.Ceiling(78.0/ shuffleNumber); //สับไพ่ครั้งละกี่ใบ ใช้ในอนิเมชั่นการสับไพ่
                tarot.shuffleDetail(shuffleNumber); //สับไพ่(ข้อมูล)
                await animatedCardShuffle(new int[] {925,620}, new int[] { 460, 310 }, 10, numberAddforShuffle);
                /*460 มาจาก 925/2 แล้วปัดเป็นเลขกลม ๆ และ 310 มาจาก 620/2
                 925 และ 620 คือขนาดโดยประมาณ ที่หารสองเพื่อให้อยู่ตรงกลางหน้าโปรแกรม
                 */
                //อนิเมชั่นสับไพ่
                //sort(); //คลี่ไพ่
            /*}
            catch
            {
                Message.messageError();
            }*/
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

        void StartLoadProgram(object sender, EventArgs e)
        {
            TimerStartProgram.Start();
            background1.Dispose();
            labelClickStartProgram.Dispose();
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

        private void pictureBox78_Click(object sender, EventArgs e)
        {
            RunTarot();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void speedShuffleX1(object sender, EventArgs e)
        {
            speedX3.Show();
            speedX1.Hide();
            speedDelayShuffle = speedX1Value;
        }

        private void speedShuffleX4(object sender, EventArgs e)
        {
            speedX1.Show();
            speedX3.Hide();
            speedDelayShuffle = speedX4Value;
        }
    }
}