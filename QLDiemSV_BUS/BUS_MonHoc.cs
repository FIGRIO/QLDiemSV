using System.Data;
using QLDiemSV_DAL;
using QLDiemSV_DTO;

namespace QLDiemSV_BUS
{
    public class BUS_MonHoc
    {
        DAL_MonHoc dal = new DAL_MonHoc();

        public DataTable GetDS() => dal.GetDS();
        public bool Them(DTO_MonHoc mh) => dal.Them(mh);
        public bool Sua(DTO_MonHoc mh) => dal.Sua(mh);
        public bool Xoa(string ma) => dal.Xoa(ma);
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
    }
}