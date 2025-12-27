using System.Data;
using QLDiemSV_DTO;
using System.Globalization;

namespace QLDiemSV_DAL
{
    public class DAL_KetQua : DBConnect
    {
        public DataTable GetDS_SinhVienTrongLop(string maLHP)
        {
            string sql = string.Format(@"
                SELECT k.MSSV, s.HoTen, s.MaLop, k.MaLHP 
                FROM KetQua k 
                JOIN SinhVien s ON k.MSSV = s.MSSV 
                WHERE k.MaLHP = '{0}'", maLHP);
            return GetDataTable(sql);
        }

        public bool DangKy(string maLHP, string mssv)
        {
            string check = string.Format("SELECT COUNT(*) FROM KetQua WHERE MaLHP = '{0}' AND MSSV = '{1}'", maLHP, mssv);
            if ((int)ExecuteScalar(check) > 0) return false;

            string sql = string.Format("INSERT INTO KetQua (MaLHP, MSSV) VALUES ('{0}', '{1}')", maLHP, mssv);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool HuyDangKy(string maLHP, string mssv)
        {
            string sql = string.Format("DELETE FROM KetQua WHERE MaLHP = '{0}' AND MSSV = '{1}'", maLHP, mssv);
            return ExecuteNonQuery(sql) > 0;
        }

        public DataTable GetBangDiem(string maLHP)
        {
            // SỬA: DiemGK, DiemCK, DiemTK (Theo DB mới)
            string sql = string.Format(@"
                SELECT k.MSSV, s.HoTen, 
                       k.DiemGK, k.DiemCK, 
                       k.DiemTK, k.DiemChu, k.MaLHP
                FROM KetQua k
                JOIN SinhVien s ON k.MSSV = s.MSSV
                WHERE k.MaLHP = '{0}'", maLHP);
            return GetDataTable(sql);
        }

        public bool CapNhatDiem(string maLHP, string mssv, float gk, float ck, float tk, string chu)
        {
            // SỬA: DiemGK, DiemCK, DiemTK
            string sql = string.Format(CultureInfo.InvariantCulture,
                @"UPDATE KetQua 
                  SET DiemGK = {2}, 
                      DiemCK = {3}, 
                      DiemTK = {4}, 
                      DiemChu = '{5}'
                  WHERE MaLHP = '{0}' AND MSSV = '{1}'",
                maLHP, mssv, gk, ck, tk, chu);

            return ExecuteNonQuery(sql) > 0;
        }

        public DataTable GetDiemBySinhVien(string mssv)
        {
            // SỬA: Join m.MaMH, DiemGK...
            string sql = string.Format(@"
                SELECT m.MaMH, m.TenMH, m.SoTinChi, l.HocKy, l.NamHoc,
                       k.DiemGK, k.DiemCK, 
                       k.DiemTK, k.DiemChu
                FROM KetQua k
                JOIN LopHocPhan l ON k.MaLHP = l.MaLHP
                JOIN MonHoc m ON l.MaMon = m.MaMH
                WHERE k.MSSV = '{0}'", mssv);

            return GetDataTable(sql);
        }
    }
}