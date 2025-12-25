using System;
using System.Windows.Forms;

namespace QLDiemSV_GUI
{
    public partial class frmMain : Form
    {
        private string taiKhoan;
        private string quyenHan;

        // Constructor mặc định (bắt buộc phải có để Designer hoạt động)
        public frmMain()
        {
            InitializeComponent();
        }

        // Constructor nhận tham số từ Form Đăng nhập (Quan trọng)
        public frmMain(string taiKhoan, string quyenHan) : this()
        {
            this.taiKhoan = taiKhoan;
            this.quyenHan = quyenHan;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin người dùng lên tiêu đề Form
            this.Text = "Hệ thống Quản lý Điểm - Xin chào: " + taiKhoan + " (" + quyenHan + ")";

            // Xử lý phân quyền (Ẩn hiện menu) sẽ viết ở đây sau...
        }
    }
}