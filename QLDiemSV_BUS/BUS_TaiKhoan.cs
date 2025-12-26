using QLDiemSV_DAL; // Cần tham chiếu đến DAL

namespace QLDiemSV_BUS
{
    public class BUS_TaiKhoan
    {
        // Khai báo đối tượng DAL để dùng
        DAL_TaiKhoan dalTaiKhoan = new DAL_TaiKhoan();

        public bool CheckLogin(string user, string pass)
        {
            // Có thể thêm kiểm tra nghiệp vụ ở đây (ví dụ không được để trống)
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return false;
            }
            // Gọi xuống DAL để kiểm tra trong database
            return dalTaiKhoan.KiemTraDangNhap(user, pass);
        }

        public string GetQuyen(string user)
        {
            return dalTaiKhoan.LayQuyenHan(user);
        }

        public bool DoiMatKhau(string user, string oldPass, string newPass)
        {
            // 1. Kiểm tra mật khẩu cũ có đúng không
            if (!dalTaiKhoan.KiemTraDangNhap(user, oldPass))
            {
                return false; // Mật khẩu cũ sai
            }

            // 2. Nếu đúng thì cập nhật mật khẩu mới
            return dalTaiKhoan.DoiMatKhau(user, newPass);
        }
    }
}