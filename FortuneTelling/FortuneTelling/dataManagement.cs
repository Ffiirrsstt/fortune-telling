using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneTelling
{
    internal class dataManagement
    {
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
    }
}
