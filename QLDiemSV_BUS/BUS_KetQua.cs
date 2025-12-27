using System.Data;
using QLDiemSV_DAL;

namespace QLDiemSV_BUS
{
    public class BUS_KetQua
    {
        DAL_KetQua dal = new DAL_KetQua();

        public DataTable GetDS_SV(string maLHP) => dal.GetDS_SinhVienTrongLop(maLHP);
        public bool DangKy(string maLHP, string mssv) => dal.DangKy(maLHP, mssv);
        public bool HuyDangKy(string maLHP, string mssv) => dal.HuyDangKy(maLHP, mssv);

        public DataTable GetBangDiem(string maLHP) => dal.GetBangDiem(maLHP);
        public bool CapNhatDiem(string maLHP, string mssv, float gk, float ck, float tk, string chu)
        {
            return dal.CapNhatDiem(maLHP, mssv, gk, ck, tk, chu);
        }
        public DataTable GetDiemBySinhVien(string mssv) => dal.GetDiemBySinhVien(mssv);
    }
}