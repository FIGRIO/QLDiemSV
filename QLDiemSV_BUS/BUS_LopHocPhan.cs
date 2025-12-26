using System.Data;
using QLDiemSV_DAL;
using QLDiemSV_DTO;

namespace QLDiemSV_BUS
{
    public class BUS_LopHocPhan
    {
        DAL_LopHocPhan dal = new DAL_LopHocPhan();

        public DataTable GetDS() => dal.GetDS();
        public bool Them(DTO_LopHocPhan lhp) => dal.Them(lhp);
        public bool Sua(DTO_LopHocPhan lhp) => dal.Sua(lhp);
        public bool Xoa(string ma) => dal.Xoa(ma);
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);

        public DataTable GetLopByGV(string maGV) => dal.GetLopByGV(maGV);
    }
}