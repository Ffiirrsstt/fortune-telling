using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FortuneTelling
{
    internal class Tarot:Form
    {
        Random random = new Random();
        public static string[][] TarotData = { 
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
            new string[]  { "", "" },
        };
        private int lengthTarotData = TarotData.Length;

        public Tarot()
        {
            coordinates();
        }

        //พิกัดของไพ่
        public void coordinates(){
            int X, nNew;
            for (int n = 0;n<= lengthTarotData - 1; n++)
            {
                /*
                 ไพ่ใบแรกเริ่มต้นขวามือสุดที่พิกัด 1230,750
                ไพ่ใบสุดท้ายของแถวแรกมีพิกัด 10,750
                ทำการขยับไพ่ทีละ 40 ตามแนวพิกัดแกน X เป็นดังสมการ
                1230 - 40n เมื่อ n = 0,1,2,3,...,30 (n+1 = ไพ่ใบที่...)
                หมายเหตุ : 1230 - 40n เมื่อ n = 0 ได้1230 คือพิกัดแกน x ของไพ่ใบแรกสุด
                หนึ่งแถวจะมีได้สูงสุด 1230 - 40n = 10
                ได้ 30.5 = 30 (ปัดเศษลง เนื่องจากหาก n = 31 จะได้พิกัด X เป็นติดลบ)
                ดังนั้นมีทั้งหมด 31 ใบ ตั้งแต่ 0,1,2,3,...,30
                 */
                if(n<=25)
                { 
                    X = 925 - (37 * (n));
                    AddData(TarotData, n, new string[] { X.ToString(), "615" });
                }
                else if(n<=51) //จำนวนแถวที่สองเป็นจำนวน 31 ใบ ตั้งแต่ 31 ถึง 61
                {
                    nNew = n % 26; //แถวที่สอง
                    /*ตามสมการที่คำนวณเอาไว้ 1230 - 40n เมื่อ n = 0,1,2,3,...,30
                     ดังนั้นจึงต้องทำให้ n ในที่นี้กลายเป็น n ตามสมการ*/
                    X = 925 - (37 * (nNew));
                    AddData(TarotData, n, new string[] { X.ToString(), "453" });
                }
                 //77-61 = 16 ใบ
                 else if(n<=74)
                {
                    nNew = n % 52; //4 ใบ ทำชิดฝั่งขวามือ
                    X = 925 - (37 * (nNew));
                    AddData(TarotData, n, new string[] { X.ToString(), "291" });
                }
                else
                {
                    nNew = n % 74; //4 ใบ ทำชิดฝั่งขวามือ
                    X =  111 - (37 * (nNew));
                    AddData(TarotData, n, new string[] { X.ToString(), "291" });
                }
            }
        }

        public static void AddData(string[][] dataArray, int rowIndex, string[] newData)
        {
            if (rowIndex >= 0 && rowIndex < dataArray.Length)
            {
                int lengthTotalRow = dataArray[rowIndex].Length + newData.Length;
                int indexStart = dataArray[rowIndex].Length;
                // ปรับขนาดของแถว
                Array.Resize(ref dataArray[rowIndex], lengthTotalRow);

                // เพิ่มข้อมูลในตำแหน่งที่กำหนด
                Array.Copy(newData, 0, dataArray[rowIndex], indexStart, newData.Length);
            }
            else
                Message.messageError();
        }

        //สลับไพ่ สลับส่วนของชื่อและคำอธิบาย
        public void shuffleDetail(){
            int randomNumber;
            string nameTarot, explanation;
            for (int i = 0; i <= lengthTarotData - 1; i++)
            {
                randomNumber = random.Next();
                nameTarot = TarotData[randomNumber][0];
                explanation = TarotData[randomNumber][1];
                TarotData[randomNumber][0] = TarotData[i][0];
                TarotData[randomNumber][1] = TarotData[i][1];
                TarotData[i][0] = nameTarot;
                TarotData[i][1] = explanation;
            }
        }
    }
}
