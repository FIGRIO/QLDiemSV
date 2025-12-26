using System.Data;
using QLDiemSV_DAL;
using QLDiemSV_DTO;

namespace QLDiemSV_BUS
{
    public class BUS_SinhVien
    {
        DAL_SinhVien dalSV = new DAL_SinhVien();

        public DataTable GetDanhSachSV()
        {
            return dalSV.GetSinhVien();
        }

        public bool ThemSV(DTO_SinhVien sv)
        {
            return dalSV.ThemSinhVien(sv);
        }

        public bool SuaSV(DTO_SinhVien sv)
        {
            return dalSV.SuaSinhVien(sv);
        }

        public bool XoaSV(string mssv)
        {
            return dalSV.XoaSinhVien(mssv);
        }
    }
}