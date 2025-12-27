using System.Data;
using System.Data.SqlClient; // Thư viện quan trọng để dùng SqlParameter
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    // Kế thừa từ DBConnect để sử dụng lại kết nối
    public class DAL_TaiKhoan : DBConnect
    {
        // 1. Hàm kiểm tra đăng nhập (An toàn tuyệt đối)
        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            // Sử dụng tham số @User và @Pass thay vì cộng chuỗi '{0}'
            string sql = "SELECT * FROM TaiKhoan WHERE TenDangNhap = @User AND MatKhau = @Pass";

            // Tạo danh sách tham số để gửi xuống DBConnect
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@User", tenDangNhap),
                new SqlParameter("@Pass", matKhau)
            };

            // Gọi hàm GetDataTable phiên bản mới (có truyền parameter)
            DataTable dt = this.GetDataTable(sql, parameters);

            // Nếu có dữ liệu trả về => Đăng nhập đúng
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        // 2. Hàm lấy quyền hạn (Admin, GiangVien, SinhVien)
        public string LayQuyenHan(string tenDangNhap)
        {
            string sql = "SELECT Quyen FROM TaiKhoan WHERE TenDangNhap = @User";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@User", tenDangNhap)
            };

            DataTable dt = this.GetDataTable(sql, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Quyen"].ToString();
            }
            return "";
        }

        // 3. Hàm đổi mật khẩu
        public bool DoiMatKhau(string tenDangNhap, string matKhauMoi)
        {
            string sql = "UPDATE TaiKhoan SET MatKhau = @NewPass WHERE TenDangNhap = @User";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NewPass", matKhauMoi),
                new SqlParameter("@User", tenDangNhap)
            };

            // Gọi hàm ExecuteNonQuery phiên bản mới
            return this.ExecuteNonQuery(sql, parameters) > 0;
        }
    }
}