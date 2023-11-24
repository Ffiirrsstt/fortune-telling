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
        private static Random random = new Random();
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
        private static int lengthTarotData = TarotData.Length;

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
                 ไพ่ใบแรกเริ่มต้นขวามือสุดที่พิกัด 925,615
                ไพ่ใบสุดท้ายของแถวแรกมีพิกัด 0,615
                ทำการขยับไพ่ทีละ 37 ตามแนวพิกัดแกน X เป็นดังสมการ
                925 -37n เมื่อ n = 0,1,2,3,...,25 (n+1 = ไพ่ใบที่...)
                หมายเหตุ : 925 - 37n เมื่อ n = 0 ได้925 คือพิกัดแกน x ของไพ่ใบแรกสุด
                หนึ่งแถวจะมีได้สูงสุด 925 - 37n = 0
                ได้ 25
                ดังนั้นมีทั้งหมด 26 ใบ ตั้งแต่ 0,1,2,3,...,25
                 */
                if(n<=25)
                { 
                    X = 925 - (37 * (n));
                    dataManagement.AddData(TarotData, n, new string[] { X.ToString(), "615" });
                }
                else if(n<=51) //จำนวนแถวที่สองเป็นจำนวน 26 ใบ ตั้งแต่ 26 ถึง 51
                {
                    nNew = n % 26; //แถวที่สอง
                    /*ตามสมการที่คำนวณเอาไว้ 925 - 37n เมื่อ n = 0,1,2,3,...,25
                     ดังนั้นจึงต้องทำให้ n ในที่นี้กลายเป็น n ตามสมการ*/
                    X = 925 - (37 * (nNew));
                    dataManagement.AddData(TarotData, n, new string[] { X.ToString(), "453" });
                }
                 else
                {
                    nNew = n % 52;
                    X = 925 - (37 * (nNew));
                    dataManagement.AddData(TarotData, n, new string[] { X.ToString(), "291" });
                }
            }
        }

        //สลับไพ่ สลับส่วนของชื่อและคำอธิบาย
        public void shuffleDetail(int shuffleNumber)
        {
            int randomNumber;
            string nameTarot, explanation;
            for(int round = 1; round <= shuffleNumber; round++) {
                for (int i = 0; i <= lengthTarotData - 1; i++)
                {
                    randomNumber = random.Next(lengthTarotData);
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
}
