using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace FortuneTelling
{
    internal class Tarot:Form
    {
        private static Random random = new Random();
        public static string[][] TarotData = { 
            new string[]  { "ace-of-cups", "" },
            new string[]  { "ace-of-pentacles", "" },
            new string[]  { "ace-of-swords", "" },
            new string[]  { "ace-of-wands", "" },
            new string[]  { "death", "" },
            new string[]  { "eight-of-cups", "" },
            new string[]  { "eight-of-pentacles", "" },
            new string[]  { "eight-of-swords", "" },
            new string[]  { "eight-of-wands", "" },
            new string[]  { "five-of-cups", "" },
            new string[]  { "five-of-pentacles", "" },
            new string[]  { "five-of-swords", "" },
            new string[]  { "five-of-wands", "" },
            new string[]  { "four-of-cups", "" },
            new string[]  { "four-of-pentacles", "" },
            new string[]  { "four-of-swords", "" },
            new string[]  { "four-of-wands", "" },
            new string[]  { "judgement", "" },
            new string[]  { "justice", "" },
            new string[]  { "king-of-cups", "" },
            new string[]  { "king-of-pentacles", "" },
            new string[]  { "king-of-swords", "" },
            new string[]  { "king-of-wands", "" },
            new string[]  { "knight-of-cups", "" },
            new string[]  { "knight-of-pentacles", "" },
            new string[]  { "knight-of-swords", "" },
            new string[]  { "knight-of-wands", "" },
            new string[]  { "nine-of-cups", "" },
            new string[]  { "nine-of-pentacles", "" },
            new string[]  { "nine-of-swords", "" },
            new string[]  { "nine-of-wands", "" },
            new string[]  { "page-of-cups", "" },
            new string[]  { "page-of-pentacles", "" },
            new string[]  { "page-of-swords", "" },
            new string[]  { "page-of-wands", "" },
            new string[]  { "queen-of-cups", "" },
            new string[]  { "queen-of-pentacles", "" },
            new string[]  { "queen-of-swords", "" },
            new string[]  { "queen-of-wands", "" },
            new string[]  { "seven-of-cups", "" },
            new string[]  { "seven-of-pentacles", "" },
            new string[]  { "seven-of-swords", "" },
            new string[]  { "seven-of-wands", "" },
            new string[]  { "six-of-cups", "" },
            new string[]  { "six-of-pentacles", "" },
            new string[]  { "six-of-swords", "" },
            new string[]  { "six-of-wands", "" },
            new string[]  { "strength", "" },
            new string[]  { "temperance", "" },
            new string[]  { "ten-of-cups", "" },
            new string[]  { "ten-of-pentacles", "" },
            new string[]  { "ten-of-swords", "" },
            new string[]  { "ten-of-wands", "" },
            new string[]  { "the-chariot", "" },
            new string[]  { "the-devil", "" },
            new string[]  { "the-emperor", "" },
            new string[]  { "the-empress", "" },
            new string[]  { "the-fool", "" },
            new string[]  { "the-hanged-man", "" },
            new string[]  { "the-heirophant", "" },
            new string[]  { "the-hermit", "" },
            new string[]  { "the-high-priestess", "" },
            new string[]  { "the-lovers", "" },
            new string[]  { "the-magician", "" },
            new string[]  { "the-moon", "" },
            new string[]  { "the-star", "" },
            new string[]  { "the-sun", "" },
            new string[]  { "the-tower", "" },
            new string[]  { "the-world", "" },
            new string[]  { "three-of-cups", "" },
            new string[]  { "three-of-pentacles", "" },
            new string[]  { "three-of-swords", "" },
            new string[]  { "three-of-wands", "" },
            new string[]  { "two-of-cups", "" },
            new string[]  { "two-of-pentacles", "" },
            new string[]  { "two-of-swords", "" },
            new string[]  { "two-of-wands", "" },
            new string[]  { "wheel-of-fortune", "" },
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
