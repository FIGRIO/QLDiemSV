using System;
using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_SinhVien : DBConnect
    {
        // 1. Lấy danh sách sinh viên
        public DataTable GetSinhVien()
        {
            // Kết bảng SinhVien với LopSinhHoat để lấy tên lớp cho đẹp (nếu cần)
            string sql = "SELECT * FROM SinhVien";
            return this.GetDataTable(sql);
        }

        // 2. Thêm Sinh viên (Đồng thời tạo luôn Tài khoản)
        public bool ThemSinhVien(DTO_SinhVien sv)
        {
            // Bước 1: Tạo tài khoản trước (Mặc định pass 123)
            string sqlTaiKhoan = string.Format("INSERT INTO TaiKhoan(TenDangNhap, MatKhau, Quyen) VALUES ('{0}', '123', 'SinhVien')", sv.MSSV);

            // Bước 2: Thêm thông tin sinh viên
            string sqlSV = string.Format("INSERT INTO SinhVien(MSSV, HoTen, NgaySinh, GioiTinh, Email, SDT, DiaChi, MaLop) " +
                                         "VALUES ('{0}', N'{1}', '{2}', N'{3}', '{4}', '{5}', N'{6}', '{7}')",
                                         sv.MSSV, sv.HoTen, sv.NgaySinh.ToString("yyyy-MM-dd"), sv.GioiTinh, sv.Email, sv.SDT, sv.DiaChi, sv.MaLop);

            // Thực thi 2 lệnh (Nếu lệnh 1 ok thì làm lệnh 2)
            if (ExecuteNonQuery(sqlTaiKhoan) > 0)
            {
                return ExecuteNonQuery(sqlSV) > 0;
            }
            return false;
        }

        // 3. Sửa Sinh viên
        public bool SuaSinhVien(DTO_SinhVien sv)
        {
            string sql = string.Format("UPDATE SinhVien SET HoTen = N'{0}', NgaySinh = '{1}', GioiTinh = N'{2}', Email = '{3}', SDT = '{4}', DiaChi = N'{5}', MaLop = '{6}' WHERE MSSV = '{7}'",
                                       sv.HoTen, sv.NgaySinh.ToString("yyyy-MM-dd"), sv.GioiTinh, sv.Email, sv.SDT, sv.DiaChi, sv.MaLop, sv.MSSV);
            return ExecuteNonQuery(sql) > 0;
        }

        // 4. Xóa Sinh viên (Xóa SV trước -> Xóa Tài khoản sau)
        public bool XoaSinhVien(string mssv)
        {
            string sqlSV = string.Format("DELETE FROM SinhVien WHERE MSSV = '{0}'", mssv);
            string sqlTK = string.Format("DELETE FROM TaiKhoan WHERE TenDangNhap = '{0}'", mssv);

            // Xóa ở bảng SinhVien trước (vì có khóa ngoại)
            if (ExecuteNonQuery(sqlSV) > 0)
            {
                // Xóa tiếp ở bảng TaiKhoan
                return ExecuteNonQuery(sqlTK) > 0;
            }
            return false;
        }
    }
}