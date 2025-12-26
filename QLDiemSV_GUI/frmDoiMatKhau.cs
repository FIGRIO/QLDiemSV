using System;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmDoiMatKhau : Form
    {
        // Control
        private Label lblHeader, lblUser;
        private TextBox txtPassCu, txtPassMoi, txtXacNhan;
        private Button btnLuu, btnHuy;
        private CheckBox chkHienMatKhau;

        private string _tenDangNhap;
        BUS_TaiKhoan busTK = new BUS_TaiKhoan();

        public frmDoiMatKhau(string tenDangNhap)
        {
            this._tenDangNhap = tenDangNhap;
            InitializeComponent_DMK();
        }

        private void InitializeComponent_DMK()
        {
            // 1. Cài đặt Form (Popup nhỏ)
            this.Size = new Size(400, 450);
            this.Text = "Đổi Mật Khẩu";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // 2. Header
            lblHeader = new Label { Text = "ĐỔI MẬT KHẨU", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = true, Location = new Point(110, 30) };
            this.Controls.Add(lblHeader);

            lblUser = new Label { Text = "Tài khoản: " + _tenDangNhap, Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = true, Location = new Point(130, 65) };
            this.Controls.Add(lblUser);

            // 3. Input
            CreateLabel("Mật khẩu cũ:", 40, 110);
            txtPassCu = CreateTextBox(40, 135);

            CreateLabel("Mật khẩu mới:", 40, 180);
            txtPassMoi = CreateTextBox(40, 205);

            CreateLabel("Nhập lại mật khẩu mới:", 40, 250);
            txtXacNhan = CreateTextBox(40, 275);

            // 4. Checkbox Hiện mật khẩu
            chkHienMatKhau = new CheckBox { Text = "Hiện mật khẩu", Location = new Point(40, 310), AutoSize = true, Font = new Font("Segoe UI", 9), Cursor = Cursors.Hand };
            chkHienMatKhau.CheckedChanged += (s, e) => {
                char c = chkHienMatKhau.Checked ? '\0' : '•';
                txtPassCu.PasswordChar = c;
                txtPassMoi.PasswordChar = c;
                txtXacNhan.PasswordChar = c;
            };
            this.Controls.Add(chkHienMatKhau);

            // 5. Buttons
            btnLuu = new Button { Text = "Lưu thay đổi", Location = new Point(40, 350), Size = new Size(140, 40), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;
            this.Controls.Add(btnLuu);

            btnHuy = new Button { Text = "Thoát", Location = new Point(200, 350), Size = new Size(140, 40), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => this.Close();
            this.Controls.Add(btnHuy);
        }

        // Helper tạo Label và TextBox nhanh
        private void CreateLabel(string text, int x, int y) { this.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) }); }
        private TextBox CreateTextBox(int x, int y) { var t = new TextBox { Location = new Point(x, y), Size = new Size(300, 27), Font = new Font("Segoe UI", 11), PasswordChar = '•', BorderStyle = BorderStyle.FixedSingle }; this.Controls.Add(t); return t; }

        // --- XỬ LÝ LOGIC ---
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nhập liệu
            if (string.IsNullOrWhiteSpace(txtPassCu.Text) || string.IsNullOrWhiteSpace(txtPassMoi.Text) || string.IsNullOrWhiteSpace(txtXacNhan.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassMoi.Text != txtXacNhan.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Gọi BUS thực hiện đổi mật khẩu
            if (busTK.DoiMatKhau(_tenDangNhap, txtPassCu.Text, txtPassMoi.Text))
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Mật khẩu cũ không chính xác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassCu.Focus();
                txtPassCu.SelectAll();
            }
        }
    }
}