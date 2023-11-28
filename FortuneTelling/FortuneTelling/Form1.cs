using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FortuneTelling
{
    public partial class Form : System.Windows.Forms.Form
    {
        private const int speedX1Value = 8, speedX4Value = 2;
        private Boolean passShuffle = false;
        private Boolean cardLayout = false;
        private Boolean closedMode; //โหมดใช้ไพ่ใบที่ 78 สับไพ่และคลี่ไพ่
        private Boolean originalPosition = false; //ใช้ตอนที่เลือกไพ่ยิบซี เลือกที่รอกดยืนยันอีกครั้ง 
        private int speedDelayShuffle = speedX1Value; //เริ่มต้นตั้งค่าให้ความเร็วการสับ คือ 1 เท่า
        private string mode;
        private TabPage deletedPage;

        private PreviouslySelectedData previouslySelectedData = new PreviouslySelectedData();
        private TarotSelection tarotSelection;

        private PictureBox prePictureBox;


        private void settingRunTarotforEnterORClick()
        {
            if (int.TryParse(DataNumShuffle.Text, out int result))
            {
                if (result != 0)
                {
                    RunTarot(result);
                    DataNumShuffle.Hide();
                }
                else
                {
                    Message.messageErrorNoZero();
                    DataNumShuffle.Clear();
                }
            }
            else
            {
                Message.messageErrorShuffle();
                DataNumShuffle.Clear();
            }
        }

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

        private void settingTarotData()
        {
            btnOpenCard.Hide();
            textHead.Hide();
            textDataTarot.Hide();
        }

        private void settingHidePageMain()
        {
            bgMain.Hide();
            btnModeLove.Hide();
            btnModeDay.Hide();
        }

        private void settingShowPageMain()
        {
            bgMain.Show();
            btnModeLove.Show();
            btnModeDay.Show();

            bgMain.BringToFront();
            btnModeLove.BringToFront();
            btnModeDay.BringToFront();
            closedMode = true; //โหมดใช้ไพ่ใบที่ 78 สับไพ่และคลี่ไพ่
        }

        private void settingTarotStart() //เซ็ตให้ไพ่อยู่ที่จุดเริ่มต้น
        {
            PictureBox pictureBoxforFind;
            for (int i = 1; i <= 78; i++)
            {
                string pictureBoxName = "pictureBox" + i;
                pictureBoxforFind = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBoxforFind != null)
                {
                    pictureBoxforFind.Location = new Point(925, 620);
                }
                else
                    Message.messageError();
            }
        }

        private void settingProgram()
        {
            deletedPage = tabControl1.TabPages[1];
            tabControl1.TabPages.RemoveAt(1);

            labelClickStartProgram.Click += StartLoadProgram;
            background1.Click += StartLoadProgram;

            settingHideBtnShuffle(); //จัดการกับปุ่มจัดการความเร็วและข้ามอนิเมชั่นการสับไพ่
            settingTarotData();
        }

        private void settingSelectTabpage2()
        {
            tabControl1.TabPages.Add(deletedPage);
            tabControl1.SelectTab(deletedPage);
            settingTarotStart();
            description.Show();
            DataNumShuffle.Show();
            DataNumShuffle.Focus();
            tabControl1.TabPages.RemoveAt(0);
            settingTarotSetup();
        }

        private void settingTarotSetup()
        {
            //Initial card setup
            for (int i = 1; i <= 78; i++)
            {
                string pictureBoxName = "pictureBox" + i;
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Cursor = Cursors.Hand;
                }
                else
                    Message.messageError();
            }
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
                    int X = int.Parse(Tarot.TarotData[i - 1][3]);
                    int Y = int.Parse(Tarot.TarotData[i - 1][4]);
                    pictureBox.Location = new Point(X, Y);
                    pictureBox.BringToFront();
                }
            }

            tarotSelection = new TarotSelection();
            closedMode = false;
        }

        private async void RunTarot(int result) {
            Tarot tarot = new Tarot();
            try {
                int shuffleNumber = result; //จำนวนครั้งการสับไพ่
                int numberAddforShuffle = (int)Math.Ceiling(78.0/ shuffleNumber); //สับไพ่ครั้งละกี่ใบ ใช้ในอนิเมชั่นการสับไพ่
                
                tarot.shuffleDetail(shuffleNumber); //สับไพ่(ข้อมูล)

                //คุมให้กดไพ่ครั้งแรกเป็นการสับไพ่ กดครั้งที่สองเป็นการคลี่ไพ่ (วางเรียงให้หยิบไพ่ง่าย ๆ)
                if (cardLayout)
                {
                    sort(); //คลี่ไพ่
                    description.Text = "เลือกไพ่เพื่อรับคำทำนาย. . .";
                    cardLayout = false;
                }
                else
                {
                    //ตอนกำลังสับไพ่
                    description.Text = "กำลังทำการสับไพ่. . . กรุณาอย่าคลิกที่ไพ่จนกว่ากระบวนการสับไพ่จะเสร็จสิ้น\nผู้ใช้งานสามารถกดปุ่มข้ามหรือกดปุ่มเพิ่มความเร็วการสับไพ่ได้";
                    description.Location = new Point(190, 480);
                    description.Size = new Size(600, 60);

                    //อนิเมชั่นสับไพ่
                    await animatedCardShuffle(new int[] { 925, 620 }, new int[] { 460, 310 }, 10, numberAddforShuffle);
                    /*460 มาจาก 925/2 แล้วปัดเป็นเลขกลม ๆ และ 310 มาจาก 620/2
                     925 และ 620 คือขนาดโดยประมาณ ที่หารสองเพื่อให้อยู่ตรงกลางหน้าโปรแกรม
                     */
                    cardLayout = true;
                    
                    //หลังสับไพ่เสร็จ
                    description.Text = "กรุณาคลิกที่ไพ่. . .";
                    description.Location = new Point(220, 220);
                }
            }
            catch
            {
                Message.messageError();
            }
        }

        private async Task mangePositionY3(PictureBox pictureBoxHide, int X,int Y,int Yfinal , int stepY)
        {
            while (Y < 453) //เริ่มเคลื่อนแถวสาม
            {
                Y += stepY;
                pictureBoxHide.SendToBack();
                pictureBoxHide.Location = new Point(X, Y);
                await Task.Delay(5);
            }
            pictureBoxHide.Location = new Point(925, Yfinal); //เอาไปซ้อนหลังสองแล้วเคลื่อนไป 1 ทันที
        }

        private async Task mangePositionY2(PictureBox pictureBoxHide, int X, int Y, int dataValueY, int stepY)
        {
            while (Y < dataValueY) //แถวสองเคลื่อนไปแถว 1 
            {
                Y += stepY;
                pictureBoxHide.SendToBack();
                pictureBoxHide.Location = new Point(X, Y);
                await Task.Delay(5);
            }
        }

        private async Task tarotHideX(PictureBox pictureBoxSelected)
        {
            for (int i = 1; i <= 26; i++) //26+26+26 = 78 ใบ
            {
                string pictureBoxName1 = "pictureBox" + i; //แถวหนึ่ง
                string pictureBoxName2 = "pictureBox" + (i + 26); //แถวสอง
                string pictureBoxName3 = "pictureBox" + (i + (26 * 2)); //แถวสาม
                PictureBox pictureBoxHide1 = Controls.Find(pictureBoxName1, true).FirstOrDefault() as PictureBox;
                PictureBox pictureBoxHide2 = Controls.Find(pictureBoxName2, true).FirstOrDefault() as PictureBox;
                PictureBox pictureBoxHide3 = Controls.Find(pictureBoxName3, true).FirstOrDefault() as PictureBox;
                if (pictureBoxHide1 != null && pictureBoxHide2 != null && pictureBoxHide3 != null)
                {
                    int X1 = pictureBoxHide1.Location.X;
                    int X2 = pictureBoxHide2.Location.X;
                    int X3 = pictureBoxHide3.Location.X;
                    int Y1 = pictureBoxHide1.Location.Y;
                    int Y2 = pictureBoxHide2.Location.Y;
                    int Y3 = pictureBoxHide3.Location.Y;
                    int X = 925, stepX = 60;
                    //X ที่ต้องการนำไปเทียบเพื่อเพิ่มค่า X ของแต่ละรูป , Y ที่ต้องการนำไปเทียบเพื่อเพิ่มค่า Y ของแต่ละรูป
                    while (X1 < X || X2 < X || X3 < X)
                    {
                        if (X1 < X)
                            X1 += stepX;
                        if (X2 < X)
                            X2 += stepX;
                        if (X3 < X)
                            X3 += stepX;
                        pictureBoxHide1.Location = new Point(X1, Y1);
                        pictureBoxHide2.Location = new Point(X2, Y2);
                        pictureBoxHide3.Location = new Point(X3, Y3);
                        await Task.Delay(5);
                    }
                    //ใช้ step เยอะจะได้ไว ๆ และตั้งตรงนี้ให้ภาพไพ่มาที่จุดที่ต้องการแบบพอดี ๆ
                    pictureBoxHide1.Location = new Point(925, Y1);
                    pictureBoxHide2.Location = new Point(925, Y2);
                    pictureBoxHide3.Location = new Point(925, Y3);
                }
                else
                    Message.messageError();
            }
        }

        private async Task tarotHideY(PictureBox pictureBoxSelected)
        {
            for (int i = 1; i <= 26; i++) //26+26+26 = 78 ใบ
            {
                string pictureBoxName1 = "pictureBox" + i; //แถวหนึ่ง
                string pictureBoxName2 = "pictureBox" + (i + 26); //แถวสอง
                string pictureBoxName3 = "pictureBox" + (i + (26 * 2)); //แถวสาม
                PictureBox pictureBoxHide1 = Controls.Find(pictureBoxName1, true).FirstOrDefault() as PictureBox;
                PictureBox pictureBoxHide2 = Controls.Find(pictureBoxName2, true).FirstOrDefault() as PictureBox;
                PictureBox pictureBoxHide3 = Controls.Find(pictureBoxName3, true).FirstOrDefault() as PictureBox;
                if (pictureBoxHide1 != null && pictureBoxHide2 != null && pictureBoxHide3 != null)
                {
                    int X2 = pictureBoxHide2.Location.X;
                    int X3 = pictureBoxHide3.Location.X;
                    int Y2 = pictureBoxHide2.Location.Y;
                    int Y3 = pictureBoxHide3.Location.Y;
                    int X = 925, Y = 615, stepY = 5;
                    if (i != 26) //แถวหนึ่งไม่ตรงเคลื่อนลงแล้ว 
                    {
                        if (i == 25)
                        {
                            string pictureBoxName26for2 = "pictureBox" + 52; //เช็กแถว 2 ไพ่ที่ 26
                            string pictureBoxName26for3 = "pictureBox" + 78; //เช็กแถว 3 ไพ่ที่ 26
                            PictureBox pictureBoxHide26for2 = Controls.Find(pictureBoxName26for2, true).FirstOrDefault() as PictureBox;
                            PictureBox pictureBoxHide26for3 = Controls.Find(pictureBoxName26for3, true).FirstOrDefault() as PictureBox;
                            if (pictureBoxHide26for2 != pictureBoxSelected)
                                pictureBoxHide2.Location = new Point(X, Y);
                            if (pictureBoxHide26for3 != pictureBoxSelected)
                                pictureBoxHide3.Location = new Point(X, Y);
                            /*ถ้าไพ่ที่ 26 ตรงกับไพ่ที่เลือกให้ดึงไพ่ที่ 25 มาแทนที่เพื่อทำแอนิเมชั่น แต่ถ้าไม่ตรงก็ส่งไปแถวหนึ่งเลย*/
                        }
                        else
                        {
                            if (pictureBoxHide2 != pictureBoxSelected)
                                pictureBoxHide2.Location = new Point(X, Y);
                            if (pictureBoxHide3 != pictureBoxSelected)
                                pictureBoxHide3.Location = new Point(X, Y);
                        }
                    }
                    else //ไพ่ที่ 26
                    {
                        if (pictureBoxHide3 == pictureBoxSelected) //ถ้าไพ่ที่ 26 แถว 3 ตรง ให้ใช้ไพ่ที่ 25 แถว 3 แทน
                        {
                            string pictureBoxName = "pictureBox" + 77;
                            PictureBox pictureBoxHide25for3 = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                            if (pictureBoxHide25for3 != null)
                            {
                                int X25for3 = pictureBoxHide25for3.Location.X;
                                int Y25for3 = pictureBoxHide25for3.Location.Y;
                                await mangePositionY3(pictureBoxHide25for3, X25for3, Y25for3, Y, stepY);
                                await mangePositionY2(pictureBoxHide2, X2, Y2, Y, stepY);
                            }
                        }
                        else if (pictureBoxHide2 == pictureBoxSelected) //ถ้าไพ่ที่ 26 แถว 2 ตรง ให้ใช้ไพ่ที่ 25 แถว 2 แทน
                        {
                            string pictureBoxName = "pictureBox" + 51;
                            PictureBox pictureBoxHide25for2 = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                            if (pictureBoxHide25for2 != null)
                            {
                                int X25for2 = pictureBoxHide25for2.Location.X;
                                int Y25for2 = pictureBoxHide25for2.Location.Y;
                                await mangePositionY3(pictureBoxHide3, X3, Y3, Y, stepY);
                                await mangePositionY2(pictureBoxHide25for2, X25for2, Y25for2, Y, stepY);
                            }
                        }
                        else
                        {
                            await mangePositionY3(pictureBoxHide3, X3, Y3, Y, stepY);
                            await mangePositionY2(pictureBoxHide2, X2, Y2, Y, stepY);
                        }
                    }
                }
                else
                    Message.messageError();
            }
        }

        private void tarotHide(PictureBox pictureBoxSelected)
        {
            for (int i = 1; i <= 78; i++)
            {
                string pictureBoxName = "pictureBox" + i; //แถวหนึ่ง
                PictureBox pictureBoxHide = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBoxHide != null)
                {
                    if (pictureBoxHide != pictureBoxSelected)
                        pictureBoxHide.Hide();
                }
            }
        }
        private async Task manageSelected(PictureBox pictureBoxSelected,int index)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string path;

            description.Hide();
            pictureBoxSelected.Location = new Point(310, 100); //ส่งไพ่ที่เลือกเอาไว้ไปตรงกลางหน้าโปรแกรม
            //ขยายขนาดรูปภาพ
            pictureBoxSelected.Size = new Size(400, 600);
            await Task.Delay(10);

            path = Path.Combine(Application.StartupPath, Tarot.TarotData[index][0].ToLower().Replace(" ", "-") + ".png");
            pictureBoxSelected.SizeMode = PictureBoxSizeMode.StretchImage; //ให้รูปเต็มพอดีกับการ์ด
            pictureBoxSelected.Image = Image.FromFile(path); //ให้มีขอบลายของไพ่อยู่

            textHead.Text = textInfo.ToTitleCase(Tarot.TarotData[index][0].Replace('-', ' '));
            textHead.Location = new Point(310,710); //ให้อยู่ด้านล่างรูปตรงกึ่งกลาง
            textHead.Size = new Size(400, 40);
            textHead.TextAlign = ContentAlignment.MiddleCenter;
            textHead.Show();

            btnOpenCard.Show();
            btnOpenCard.Location = new Point(400, 40);

            btnOpenCard.Click += (sender, e) => openCard(pictureBoxSelected,index);
        }

        private void openCard(PictureBox pictureBoxSelected, int index)
        {
            pictureBoxSelected.Location = new Point(10, 100);
            textHead.Size = new Size(600, 150);
            textHead.Location = new Point(410, 100);
            textDataTarot.Location = new Point(410, 200);
            if (mode == "ความรัก")
                textDataTarot.Text = Tarot.TarotData[index][1];
            else
                textDataTarot.Text = Tarot.TarotData[index][2];
            textDataTarot.ForeColor = Color.White;
            textDataTarot.Size = new Size(600, 400);
            textDataTarot.Show();

            btnOpenCard.Hide();
        }

        private async Task selectedConfirmMotion(PictureBox pictureBox, int index, int X)
        {
            int positionX = 925, Y = 90;
            while (X <= positionX)
            {
                X += 10;
                pictureBox.Location = new Point(X, Y);
                await Task.Delay(5);
            }
            await tarotHideX(pictureBox);
            await tarotHideY(pictureBox);
            await Task.Delay(5);
            tarotHide(pictureBox);
            await Task.Delay(5);
            await manageSelected(pictureBox, index);

            originalPosition = false; 
            //ยืนยันไพ่เรียบร้อยแล้ว หากเริ่มการจับไพ่ใหม่อีกครั้ง เมื่อมีการจับไพ่จะไม่มีการส่งไพ่ใบใด ๆ กลับที่เดิม
        }

        private async Task selectedNotConfirmYetMotion(PictureBox pictureBox, int index, int X, int Y)
        {
            if (originalPosition) //ต้องการส่งไพ่ใบที่เลือกก่อนหน้านี้กลับที่เดิม (ที่เลือกและยังไม่ได้ยืนยัน)
            {
                int preX = previouslySelectedData.X;
                int preY = previouslySelectedData.Y;
                prePictureBox.Location = new Point(preX, preY);
            }

            prePictureBox = pictureBox;
            previouslySelectedData.X = X;
            previouslySelectedData.Y = Y;

            pictureBox.BringToFront();

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

        private async void tarotPositionSelection(PictureBox pictureBox, int index)
        {
            Boolean confirm = tarotSelection.selection[index];
            /*confirm = false คือคลิกเลือกไพ่ครั้งแรก
             confirm = true คือยืนยันการเลือกไพ่*/

            int X = pictureBox.Location.X;
            int Y = pictureBox.Location.Y;

            if (confirm)
                await selectedConfirmMotion(pictureBox, index, X);
            else
                await selectedNotConfirmYetMotion(pictureBox, index, X, Y);
        }

        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            settingProgram();
            settingHidePageMain();
        }

        private void clickTarot(object sender, EventArgs e)
        {
            if(sender is PictureBox)
            {
                description.Text = "คลิกที่ไพ่อีกครั้ง เพื่อยืนยันการตัดสินใจ\nหรือหากต้องการเปลี่ยนไพ่ที่เลือกให้กดคลิกที่ไพ่ใบอื่น ๆ";
                description.Location = new Point(190, 15);
                int pictureBoxNumber;
                PictureBox pictureBox = (PictureBox)sender;
                textDataTarot.Hide();
                textHead.Hide();
                if (int.TryParse(pictureBox.Name.Replace("pictureBox", ""), out pictureBoxNumber))
                    tarotPositionSelection(pictureBox, pictureBoxNumber-1);
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
            {
                settingShowPageMain();
                TimerStartProgram.Dispose();
            }
        }

        private void speedShuffleX1(object sender, EventArgs e)
        {
            speedX4.Show();
            speedX1.Hide();
            speedDelayShuffle = speedX1Value;
        }

        private void speedShuffleX4(object sender, EventArgs e)
        {
            speedX1.Show();
            speedX4.Hide();
            speedDelayShuffle = speedX4Value;
        }

        private void btnPassShuffle_Click(object sender, EventArgs e)
        {
            passShuffle = true;
        }

        private void btnModeLove_Click(object sender, EventArgs e)
        {
            settingSelectTabpage2();
            mode = "ความรัก";
        }

        private void btnModeDay_Click(object sender, EventArgs e)
        {
            settingSelectTabpage2();
            mode = "รายวัน";
        }

        private void DataNumShuffle_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                settingRunTarotforEnterORClick();
            }
        }

        private void pictureBox78_Click(object sender, EventArgs e)
        {
            if (closedMode)
                settingRunTarotforEnterORClick();
        }
    }
}