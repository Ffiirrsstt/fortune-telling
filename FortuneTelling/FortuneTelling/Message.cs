﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FortuneTelling
{
    internal class Message
    {
        public static void messageError() {
            MessageBox.Show("ระบบมีปัญหา โปรดแจ้งเพื่อทำการแก้ไข. . .");
        }

        public static void messageErrorShuffle()
        {
            MessageBox.Show("ป้อนโปรดตัวเลขสำหรับการสับไพ่. . .");
        }

        public static void messageErrorNoZero()
        {
            MessageBox.Show("ป้อนโปรดตัวเลขที่ไม่ใช่เลข 0 สำหรับเป็นจำนวนในการสับไพ่. . .");
        }
    }
}
