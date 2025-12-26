using System.Data;
using QLDiemSV_DTO; // Sử dụng DTO để lấy dữ liệu truyền vào

namespace QLDiemSV_DAL
{
    // Kế thừa từ DBConnect để sử dụng lại kết nối
    public class DAL_TaiKhoan : DBConnect
    {
        // Hàm kiểm tra tài khoản có tồn tại không
        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            // Viết câu truy vấn (Nên dùng Parameter để tránh SQL Injection, ở đây viết đơn giản để bạn dễ hiểu)
            string sql = string.Format("SELECT * FROM TaiKhoan WHERE TenDangNhap = '{0}' AND MatKhau = '{1}'", tenDangNhap, matKhau);

            DataTable dt = this.GetDataTable(sql); // Gọi hàm lấy dữ liệu từ lớp cha DBConnect

            // Nếu bảng trả về có ít nhất 1 dòng -> Đăng nhập đúng
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        // Hàm lấy quyền hạn của tài khoản (Admin, GiangVien, hay SinhVien)
        public string LayQuyenHan(string tenDangNhap)
        {
            string sql = string.Format("SELECT Quyen FROM TaiKhoan WHERE TenDangNhap = '{0}'", tenDangNhap);
            DataTable dt = this.GetDataTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Quyen"].ToString();
            }
            return "";
        }

        public bool DoiMatKhau(string tenDangNhap, string matKhauMoi)
        {
            string sql = string.Format("UPDATE TaiKhoan SET MatKhau = '{0}' WHERE TenDangNhap = '{1}'", matKhauMoi, tenDangNhap);
            return ExecuteNonQuery(sql) > 0;
        }
    }
}