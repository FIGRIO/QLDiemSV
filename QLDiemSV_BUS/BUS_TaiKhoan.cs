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
    }
}