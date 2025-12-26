using System;

namespace QLDiemSV_DTO
{
    public class DTO_MonHoc
    {
        public string MaMH { get; set; }
        public string TenMH { get; set; }
        public int SoTinChi { get; set; }

        public DTO_MonHoc(string ma, string ten, int stc)
        {
            this.MaMH = ma;
            this.TenMH = ten;
            this.SoTinChi = stc;
        }
    }
}