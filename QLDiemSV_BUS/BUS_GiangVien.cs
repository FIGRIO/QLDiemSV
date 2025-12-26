using System.Data;
using QLDiemSV_DAL;
using QLDiemSV_DTO;

namespace QLDiemSV_BUS
{
    public class BUS_GiangVien
    {
        DAL_GiangVien dal = new DAL_GiangVien();

        public DataTable GetDS() => dal.GetDS();
        public bool Them(DTO_GiangVien gv) => dal.Them(gv);
        public bool Sua(DTO_GiangVien gv) => dal.Sua(gv);
        public bool Xoa(string ma) => dal.Xoa(ma);
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
    }
}