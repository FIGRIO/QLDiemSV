using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_KetQua : DBConnect
    {
        // 1. Lấy danh sách sinh viên CỦA MỘT LỚP CỤ THỂ
        public DataTable GetDS_SinhVienTrongLop(string maLHP)
        {
            // Join với bảng SinhVien để lấy Họ Tên hiển thị cho dễ nhìn
            string sql = string.Format(@"
                SELECT k.MSSV, s.HoTen, s.LopSinhHoat, k.MaLHP 
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
            // Lấy thông tin sinh viên + Các cột điểm
            string sql = string.Format(@"
                SELECT k.MSSV, s.HoTen, 
                       k.DiemChuyenCan, k.DiemGiuaKy, k.DiemCuoiKy, 
                       k.DiemTongKet, k.DiemChu, k.MaLHP
                FROM KetQua k
                JOIN SinhVien s ON k.MSSV = s.MSSV
                WHERE k.MaLHP = '{0}'", maLHP);
            return GetDataTable(sql);
        }

        // 5. Cập nhật điểm số (Lưu điểm)
        public bool CapNhatDiem(string maLHP, string mssv, float cc, float gk, float ck, float tk, string chu)
        {
            // Lưu ý: SQL lưu số thực dùng dấu chấm, cần format cẩn thận
            string sql = string.Format(@"
                UPDATE KetQua 
                SET DiemChuyenCan = {2}, DiemGiuaKy = {3}, DiemCuoiKy = {4}, 
                    DiemTongKet = {5}, DiemChu = '{6}'
                WHERE MaLHP = '{0}' AND MSSV = '{1}'",
                maLHP, mssv, cc, gk, ck, tk, chu);
            return ExecuteNonQuery(sql) > 0;
        }
    }
}