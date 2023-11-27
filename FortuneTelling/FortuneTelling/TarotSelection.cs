using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneTelling
{
    internal class TarotSelection
    {

        public Boolean[] selection;

        public TarotSelection(){
            // สร้างอาเรย์บูลีนขนาด 78 และกำหนดค่าเริ่มต้นเป็น false
            selection = new bool[78];
        }
    }
}
