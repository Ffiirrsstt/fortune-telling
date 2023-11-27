using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Drawing.Imaging;
using Application = System.Windows.Forms.Application;

namespace FortuneTelling
{
    internal class Img: System.Windows.Forms.Form
    {
        private static string[] fileCover = 
            {"cover1.jpg", "cover2.jpg", "cover3.jpg", "cover4.jpg", "cover5.jpg" , "cover6.jpg"
                , "bgmainL.jpg", "bgmainR.jpg"}; 
        public static string path;

        public static int IndexLastCover = fileCover.Length - 1;
        public static int indexImgOne = -1;

        public static void changePath()
        {
            try{
                indexImgOne++;
                path = Path.Combine(Application.StartupPath, fileCover[indexImgOne]);
                checkImg();
            }
            catch{
                Message.messageError();
            }
        }

        private static void checkImg()
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                path = Path.Combine(Application.StartupPath, "bgmainR.jpg");
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                        Message.messageError();
            }
        }
    }

}
