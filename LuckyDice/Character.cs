using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyDice
{
    public partial class Character
    {
        public String maNV { get; set; }
        public String tenNV { get; set; }
        public String anhNV { get; set; }
        public int diemNV { get; set; }

        public Character()
        { }
        public Character(String ma, String ten, String anh, int diem)
        {
            maNV = ma;
            tenNV = ten;
            anhNV = anh;
            diemNV = diem;
        }
    }
}
