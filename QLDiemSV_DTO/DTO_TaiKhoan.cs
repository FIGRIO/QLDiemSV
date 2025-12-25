using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDiemSV_DTO
{
    public class DTO_TaiKhoan
    {
        // Các thuộc tính tương ứng với cột trong bảng TaiKhoan (SQL)
        private string _TenDangNhap;
        private string _MatKhau;
        private string _Quyen;

        // Getter - Setter (Encapsulation)
        public string TenDangNhap
        {
            get { return _TenDangNhap; }
            set { _TenDangNhap = value; }
        }

        public string MatKhau
        {
            get { return _MatKhau; }
            set { _MatKhau = value; }
        }

        public string Quyen
        {
            get { return _Quyen; }
            set { _Quyen = value; }
        }

        // Constructor 1: Khởi tạo mặc định
        public DTO_TaiKhoan()
        {
        }

        // Constructor 2: Khởi tạo có tham số (để truyền dữ liệu nhanh)
        public DTO_TaiKhoan(string user, string pass, string role)
        {
            this.TenDangNhap = user;
            this.MatKhau = pass;
            this.Quyen = role;
        }
    }
}