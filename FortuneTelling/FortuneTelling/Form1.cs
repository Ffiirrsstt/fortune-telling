using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using timerThreading = System.Threading.Timer;

namespace FortuneTelling
{
    public partial class Form1 : Form
    {
        private const int speedX1Value = 20, speedX4Value = 5;
        private Boolean passShuffle = false;
        private Boolean cardLayout = false;
        private Boolean closedMode = true; //โหมดใช้ไพ่ใบที่ 78 สับไพ่และคลี่ไพ่
        private Boolean originalPosition = false; //ใช้ตอนที่เลือกไพ่ยิบซี เลือกที่รอกดยืนยันอีกครั้ง 
        private int speedDelayShuffle = speedX1Value; //เริ่มต้นตั้งค่าให้ความเร็วการสับ คือ 1 เท่า
        private int selectedQuantity = 0;

        private PreviouslySelectedData previouslySelectedData = new PreviouslySelectedData();
        private TarotSelection tarotSelection;

        private PictureBox prePictureBox;


        private void settingShowBtnShuffle()
        {
            if (speedDelayShuffle == speedX1Value) //ความเร็วหนึ่งเท่าให้แสดงปุ่มให้กดเพื่อปรับเป็นความเร็วสี่เท่า
                speedX4.Show();
            else
                speedX1.Show();
            btnPassShuffle.Show();
        }
        private void settingHideBtnShuffle()
        {
            speedX1.Hide();
            speedX4.Hide();
            btnPassShuffle.Hide();
        }

        private void cardsDestination(int start, int fromnumberAddforShuffle, int[] destination)
        {
            int sequence = start + 1;
            int end = start + fromnumberAddforShuffle - 1;
            /*start มาจาก i และ fromnumberAddforShuffle มาจาก numberAddforShuffle 
             ใช้ start + 1 เพราะไพ่ที่ i ถูกทำงานไปแล้ว เริ่มตั้งแต่ไพ่ที่ i+1 จึงจะถูกข้ามไป
             ใช้ start + fromnumberAddforShuffle - 1 
                เพราะเดิมทีต่อจาก i ไพ่ใบถัดไปที่เคลื่อนที่ คือ ไพ่ใบที่ i + numberAddforShuffle
             เนื่องจาก i + numberAddforShuffle ถูกทำงานแล้ว 
             ไพ่ที่ถูกข้ามไปคือตั้งแต่ไพ่ใบที่ i+1,i+2,...,i+numberAddforShuffle-1
             */
            for (; sequence <= end; sequence++)
            {
                string pictureBoxName = "pictureBox" + sequence;
                PictureBox pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBoxforFind != null)
                    pictureBoxforFind.Location = new Point(destination[0], destination[1]);
            }
        }

        private async Task<int[]> sendCartDestination(PictureBox pictureBoxforFind, int[] start, int[] end, int step)
        {
            pictureBoxforFind.BringToFront();
            //จากมุมขวาล่างไปยังจุดหมาย
            while (start[0] > end[0] || start[1] > end[1])
            {
                if (passShuffle) //เพื่อให้เมื่อกดปุ่มข้ามแล้วข้ามการสับไพ่ทันที
                    break;
                if (start[0] > end[0])
                    start[0] -= step;
                if (start[1] > end[1])
                    start[1] -= step;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
            return start;
        }

        //รูปแบบขึ้นบน ไปขวา ไปขวาพร้อมลงล่าง ลงล่างจนอยู่ในระดับเดียวกับตอนแรกในแกน Y และไปซ้ายจนกลับมาอยู่จุดเดิมในตอนแรก
        private async Task cardShufflePattern(PictureBox pictureBoxforFind, int[] start, int[] end, int step)
        {
            pictureBoxforFind.SendToBack();
            while (start[1] >= end[1] - 250) //ขึ้นบน
            {
                if (passShuffle) //เพื่อให้เมื่อกดปุ่มข้ามแล้วข้ามการสับไพ่ทันที
                    break;
                start[1] -= step;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
            while (start[0] <= end[0] + 50) //ไปขวา
            {
                if (passShuffle) //เพื่อให้เมื่อกดปุ่มข้ามแล้วข้ามการสับไพ่ทันที
                    break;
                start[0] += step;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
            while ((start[0] <= (end[0] + 100)) || (start[1] < end[1])) //ไปด้านขวาพร้อม ๆ กับเคลื่อนลงด้านล่าง หรือเคลื่อนลงด้านล่าง
            {
                if (passShuffle) //เพื่อให้เมื่อกดปุ่มข้ามแล้วข้ามการสับไพ่ทันที
                    break;
                if (start[0] <= (end[0] + 100))
                    start[0] += step;
                if (start[1] < end[1])
                    start[1] += step / 2;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
            while (start[0] > end[0]) //ไปซ้าย
            {
                if (passShuffle) //เพื่อให้เมื่อกดปุ่มข้ามแล้วข้ามการสับไพ่ทันที
                    break;
                start[0] -= step;
                pictureBoxforFind.Location = new Point(start[0], start[1]);
                await Task.Delay(speedDelayShuffle);
            }
        }

        private void immediatelydestination(int i, int[] end)
        {
            PictureBox pictureBoxforFind;
            for (; i <= 78; i ++)
            {
                string pictureBoxName = "pictureBox" + i;
                pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                if (pictureBoxforFind != null)
                {
                    pictureBoxforFind.Location = new Point(end[0], end[1]);
                    //ถ้าการหา pictureBox78 ไม่เท่ากับ null
                    //pictureBoxforFind สุดท้ายที่เกิดขึ้นจะเป็นไพ่ใบที่ 78 ซึ่งต้องการให้ขึ้นหน้าสุดเพื่อทำการกดคลี่ไพ่
                    pictureBoxforFind.BringToFront();
                }
                else
                    Message.messageError();
            }
        }

        private async Task animatedCardShuffle(int[] start, int[] end, int step, int numberAddforShuffle)
        {
            PictureBox pictureBoxforFind;
            int[] dataStartCopy = new int[] { 0, 0 };
            dataStartCopy[0] = start[0];
            dataStartCopy[1] = start[1];

            settingShowBtnShuffle(); //แสดงปุ่มปรับความเร็วการสับไพ่ยิบซี

            for (int i = 1; i <= 78; i += numberAddforShuffle)
            {
                //เมื่อกดปุ่มข้าม
                if (passShuffle)
                {
                    immediatelydestination(i, end); //เนื่องจากกดข้าม จึงจัดการส่งรูปที่เหลือไปยังจุดหมายที่ต้องการโดยทันที
                    passShuffle = false;
                    break;
                }

                string pictureBoxName = "pictureBox" + i;
                pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                if (pictureBoxforFind != null)
                {
                    //string path = Path.Combine(Application.StartupPath, Tarot.TarotData[i][0].ToLower().Replace(" ", "-") + ".png");
                    string path = Path.Combine(Application.StartupPath, Tarot.TarotData[i][0] + ".png");
                    pictureBoxforFind.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxforFind.Image = Image.FromFile(path);

                    start = await sendCartDestination(pictureBoxforFind, start, end, step);
                    if (i != 1)
                        await cardShufflePattern(pictureBoxforFind, start, end, step);

                    //ป้องกันการเกิดอาการไพ่ค้างไม่ไปจุดที่ต้องการ (เช่น เกิดขึ้นในตอนที่กดปุ่มข้ามการสับไพ่)
                    pictureBoxforFind.Location = new Point(end[0], end[1]);
                }
                else
                    Message.messageError();

                /*จัดการในกรณีที่จำนวนไพ่ที่ต้องสับในแต่ละครั้งมีจำนวนที่นำไปหาร 78 ใบไม่ลงตัว
                มีไพ่ 78 ใบ ถ้าต้องการสับไพ่จำนวน 4 ครั้ง ต้องสับครั้งละ 78/4 (ใช้ฟังก์ชันเพดานได้ 20)
                เท่ากับสับไพ่ 4 ครั้ง สับครั้งละ 20 ใบ 78 - 20 - 20 - 20 = 18
                ครั้งแรก ครั้งสอง และครั้งที่สามสามารถสับทีละ 20 ใบได้ แต่ครั้งสุดท้ายไม่สามารถทำได้
                */

            //เขียนจัดการไพ่ที่ถูกข้ามไป จัดการให้ไพ่ดังกล่าวไปยังจุดหมายในทันที
            if (i != 78 && numberAddforShuffle != 1)
                   cardsDestination(i, numberAddforShuffle, end);

                if ((78 - i) < numberAddforShuffle)
                    numberAddforShuffle = 78 - i;

                start[0] = dataStartCopy[0];
                start[1] = dataStartCopy[1];

                //เมื่อจัดการกับไพ่ใบสุดท้ายหรือใบที่ 78 เสร็จสิ้น ให้จบการทำงานของ loop
                if (i == 78) //ตั้งเงื่อนไขไว้ด้านล่าง เพื่อให้จัดการไพ่ใบที่ 78 ก่อน
                {
                    //pictureBoxforFind จะตรงกับไพ่ใบที่ 78 ซึ่งต้องการให้ขึ้นหน้าสุดเพื่อทำการกดคลี่ไพ่
                    pictureBoxforFind.BringToFront(); 
                    break;
                }
            }

            settingHideBtnShuffle(); //ซ่อนปุ่มปรับความเร็วการสับไพ่ยิบซี
        }

        private void sort()
        {
            for (int i = 1; i <= 78; i++)
            {
                string pictureBoxName = "pictureBox" + i;
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                if (pictureBox != null)
                {
                    pictureBox.Click += new EventHandler(clickTarot);
                    int X = int.Parse(Tarot.TarotData[i - 1][2]);
                    int Y = int.Parse(Tarot.TarotData[i - 1][3]);
                    pictureBox.Location = new Point(X, Y);
                    pictureBox.BringToFront();
                }
            }

            tarotSelection = new TarotSelection();
            closedMode = false;
        }

        private async void RunTarot() {
            Tarot tarot = new Tarot();
            /*try
            {*/
                int shuffleNumber = 10; //จำนวนครั้งการสับไพ่
                int numberAddforShuffle = (int)Math.Ceiling(78.0/ shuffleNumber); //สับไพ่ครั้งละกี่ใบ ใช้ในอนิเมชั่นการสับไพ่
                
                tarot.shuffleDetail(shuffleNumber); //สับไพ่(ข้อมูล)

                //คุมให้กดไพ่ครั้งแรกเป็นการสับไพ่ กดครั้งที่สองเป็นการคลี่ไพ่ (วางเรียงให้หยิบไพ่ง่าย ๆ)
                if(cardLayout)
                {
                    sort(); //คลี่ไพ่
                    cardLayout = false;
            }
            else
            {
                //อนิเมชั่นสับไพ่
                await animatedCardShuffle(new int[] {925,620}, new int[] { 460, 310 }, 10, numberAddforShuffle);
                /*460 มาจาก 925/2 แล้วปัดเป็นเลขกลม ๆ และ 310 มาจาก 620/2
                 925 และ 620 คือขนาดโดยประมาณ ที่หารสองเพื่อให้อยู่ตรงกลางหน้าโปรแกรม
                 */
                cardLayout = true;
            }
            /*}
            catch
            {
                Message.messageError();
            }*/
        }

        private void checkSelectedQuantity()
        {
            selectedQuantity = 0;
            foreach (Boolean check in tarotSelection.selection)
            {
                if (check)
                    selectedQuantity++;
            }
        }

        private void tarotHide()
        {
            Boolean dataforShow;
            for (int i = 1; i <= 78; i++)
            {
                dataforShow = tarotSelection.selection[i - 1];
                if (!dataforShow) //ไพ่ที่ไม่ได้เลือก จัดการซ่อนทั้งหมด
                {
                    string pictureBoxName = "pictureBox" + i;
                    PictureBox pictureBoxHide = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                    if (pictureBoxHide != null)
                        pictureBoxHide.Hide();
                }
            }
        }

        private async Task selectedConfirmMotion(PictureBox pictureBox, int index, int X, int Y)
        {
            checkSelectedQuantity();
            int positionX = 925 - (37 * selectedQuantity - 1);
            while (X <= positionX)
            {
                X += 15;
                pictureBox.Location = new Point(X, 90);
                pictureBox.BringToFront();
                await Task.Delay(5);
            }
            if (selectedQuantity == 10)
                tarotHide();

            originalPosition = false; //ยืนยันไพ่เรียบร้อยแล้ว ยังไม่ต้องการส่งไพ่กลับที่เดิม
        }

        private async Task selectedNotConfirmYetMotion(PictureBox pictureBox, int index, int X, int Y)
        {
            if (originalPosition) //ต้องการส่งไพ่ใบที่เลือกก่อนหน้านี้กลับที่เดิม (ที่เลือกและยังไม่ได้ยืนยัน)
            {
                int preX = previouslySelectedData.X;
                int preY = previouslySelectedData.Y;
                prePictureBox.Location = new Point(preX, preY);
                label2.Text = prePictureBox.Name;
            }
            prePictureBox = pictureBox;
            previouslySelectedData.X = X;
            previouslySelectedData.Y = Y;

            while (Y > 100)
            {
                Y -= 10;
                pictureBox.Location = new Point(X, Y);
                await Task.Delay(5);
            }
            while (X > 470 || X < 450)
            {
                if (X > 470)
                    X -= 10;
                else
                    X += 10;
                pictureBox.Location = new Point(X, Y);
                await Task.Delay(5);
            }

            tarotSelection.selection[index] = true;
            originalPosition = true; //ตั้งค่าพร้อมส่งกลับที่เดิมหากไปกดเลือกไพ่ใบใหม่
        }

        private async void tarotPositionSelection(int pictureBoxNumber, int index)
        {
            Boolean confirm = tarotSelection.selection[index];
            /*confirm = false คือคลิกเลือกไพ่ครั้งแรก
             confirm = true คือยืนยันการเลือกไพ่*/
            string pictureBoxName = "pictureBox" + pictureBoxNumber;
            PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
            if (pictureBox != null)
            {
                int X = pictureBox.Location.X;
                int Y = pictureBox.Location.Y;
                if (confirm)
                    selectedConfirmMotion(pictureBox, index, X, Y);
                else
                    selectedNotConfirmYetMotion(pictureBox, index, X, Y);
            }
            else
                Message.messageError();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelClickStartProgram.Click += StartLoadProgram;
            background1.Click += StartLoadProgram;

            settingHideBtnShuffle(); //จัดการกับปุ่มจัดการความเร็วและข้ามอนิเมชั่นการสับไพ่
        }

        private void clickTarot(object sender, EventArgs e)
        {
            if(sender is PictureBox)
            {
                int pictureBoxNumber;
                PictureBox pictureBox = (PictureBox)sender;
                if (int.TryParse(pictureBox.Name.Replace("pictureBox", ""), out pictureBoxNumber))
                    tarotPositionSelection(pictureBoxNumber, pictureBoxNumber-1); 
                //pictureBoxNumber-1 จาก pictureBox1 = index0 เป็นต้น
                else
                    Message.messageError();
            }
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
            if(closedMode)
                RunTarot();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void speedShuffleX1(object sender, EventArgs e)
        {
            speedX4.Show();
            speedX1.Hide();
            speedDelayShuffle = speedX1Value;
        }

        private void btnPassShuffle_Click(object sender, EventArgs e)
        {
            passShuffle = true;
        }

        private void speedShuffleX4(object sender, EventArgs e)
        {
            speedX1.Show();
            speedX4.Hide();
            speedDelayShuffle = speedX4Value;
        }
    }
}