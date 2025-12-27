using System.Data;
using QLDiemSV_DTO;
using System.Globalization;

namespace QLDiemSV_DAL
{
    public class DAL_KetQua : DBConnect
    {
        // 1. Lấy danh sách sinh viên CỦA MỘT LỚP CỤ THỂ
        public DataTable GetDS_SinhVienTrongLop(string maLHP)
        {
            // Sửa LopSinhHoat thành MaLop cho đúng với bảng SinhVien
            string sql = string.Format(@"
        SELECT k.MSSV, s.HoTen, s.MaLop, k.MaLHP 
        FROM KetQua k 
        JOIN SinhVien s ON k.MSSV = s.MSSV 
        WHERE k.MaLHP = '{0}'", maLHP);
            return GetDataTable(sql);
        }

        // 2. Đăng ký môn học (Thêm sinh viên vào lớp)
        public bool DangKy(string maLHP, string mssv)
        {
            // Kiểm tra xem đã đăng ký chưa để tránh lỗi trùng khóa
            string check = string.Format("SELECT COUNT(*) FROM KetQua WHERE MaLHP = '{0}' AND MSSV = '{1}'", maLHP, mssv);
            if ((int)ExecuteScalar(check) > 0) return false; // Đã tồn tại

            // Chỉ insert MaLHP và MSSV, các điểm số để null/default
            string sql = string.Format("INSERT INTO KetQua (MaLHP, MSSV) VALUES ('{0}', '{1}')", maLHP, mssv);
            return ExecuteNonQuery(sql) > 0;
        }

        // 3. Hủy đăng ký (Xóa sinh viên khỏi lớp)
        public bool HuyDangKy(string maLHP, string mssv)
        {
            string sql = string.Format("DELETE FROM KetQua WHERE MaLHP = '{0}' AND MSSV = '{1}'", maLHP, mssv);
            return ExecuteNonQuery(sql) > 0;
        }

        // 4. Lấy bảng điểm đầy đủ của một lớp
        public DataTable GetBangDiem(string maLHP)
        {
            string sql = string.Format(@"
        SELECT k.MSSV, s.HoTen, 
               k.DiemGiuaKy, k.DiemCuoiKy, 
               k.DiemTongKet, k.DiemChu, k.MaLHP
        FROM KetQua k
        JOIN SinhVien s ON k.MSSV = s.MSSV
        WHERE k.MaLHP = '{0}'", maLHP);
            return GetDataTable(sql);
        }

        // 5. Cập nhật điểm số (Lưu điểm)
        public bool CapNhatDiem(string maLHP, string mssv, float gk, float ck, float tk, string chu)
        {
            string sql = string.Format(CultureInfo.InvariantCulture,
                @"UPDATE KetQua 
          SET DiemGiuaKy = {2}, 
              DiemCuoiKy = {3}, 
              DiemTongKet = {4}, 
              DiemChu = '{5}'
          WHERE MaLHP = '{0}' AND MSSV = '{1}'",
                maLHP, mssv, gk, ck, tk, chu);

            return ExecuteNonQuery(sql) > 0;
        }

        // 6. Xem kết quả học tập của một sinh viên (Dùng cho SV xem điểm)
        public DataTable GetDiemBySinhVien(string mssv)
        {
            string sql = string.Format(@"
        SELECT m.MaMon, m.TenMon, m.SoTinChi, l.HocKy, l.NamHoc,
               k.DiemGiuaKy, k.DiemCuoiKy, 
               k.DiemTongKet, k.DiemChu
        FROM KetQua k
        JOIN LopHocPhan l ON k.MaLHP = l.MaLHP
        JOIN MonHoc m ON l.MaMon = m.MaMon
        WHERE k.MSSV = '{0}'", mssv);

            return GetDataTable(sql);
        }
    }
}