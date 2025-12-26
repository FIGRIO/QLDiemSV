using System;
using System.Drawing;
using System.Drawing.Drawing2D; // Thư viện vẽ Gradient
using System.Runtime.InteropServices; // Thư viện kéo thả form
using System.Windows.Forms;
using QLDiemSV_BUS; // Gọi tầng nghiệp vụ

namespace QLDiemSV_GUI
{
    public partial class frmDangNhap : Form
    {
        // --- KHAI BÁO CÁC CONTROL ---
        private Panel pnlLeft;
        private PictureBox pbLogo;
        private Label lblSlogan;
        private Label lblTieuDe;

        private Panel pnlUserLine;
        private Panel pnlPassLine;
        private TextBox txtUser;
        private TextBox txtPass;
        private Label lblUserIcon;
        private Label lblPassIcon;

        private Button btnLogin;
        private Button btnExit;
        private LinkLabel lnkQuenMatKhau;

        // Gọi tầng BUS
        BUS_TaiKhoan busTaiKhoan = new BUS_TaiKhoan();

        public frmDangNhap()
        {
            InitializeComponent_Gradient();
        }

        // --- PHẦN 1: THIẾT KẾ GIAO DIỆN ---
        private void InitializeComponent_Gradient()
        {
            this.SuspendLayout();

            // 1. Cài đặt Form chính
            this.ClientSize = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.None; // Tắt viền Windows
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // 2. Panel Trái (Chứa Gradient & Logo)
            pnlLeft = new Panel();
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Width = 350;
            pnlLeft.Paint += new PaintEventHandler(pnlLeft_Paint); // Gắn sự kiện vẽ màu
            pnlLeft.MouseDown += new MouseEventHandler(pnlTop_MouseDown); // Gắn sự kiện kéo thả

            // --- LOGO TỪ RESOURCES (Đã sửa theo yêu cầu) ---
            pbLogo = new PictureBox();
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.Size = new Size(180, 180);
            pbLogo.Location = new Point(85, 100);
            pbLogo.BackColor = Color.Transparent;

            try
            {
                // LƯU Ý: Đảm bảo tên ảnh trong Resources là 'hcmute_logo'
                // Nếu báo lỗi đỏ ở đây, hãy kiểm tra lại tên file ảnh bạn đã Add vào Resources
                pbLogo.Image = Properties.Resources.hcmute_logo;
            }
            catch
            {
                // Nếu chưa có ảnh thì để trống hoặc tô màu tạm để không lỗi
                pbLogo.BackColor = Color.WhiteSmoke;
            }
            pnlLeft.Controls.Add(pbLogo);

            // Slogan
            lblSlogan = new Label();
            lblSlogan.Text = "KHOA CÔNG NGHỆ THÔNG TIN\nĐẠI HỌC SƯ PHẠM KỸ THUẬT";
            lblSlogan.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSlogan.ForeColor = Color.White;
            lblSlogan.TextAlign = ContentAlignment.MiddleCenter;
            lblSlogan.AutoSize = true;
            lblSlogan.BackColor = Color.Transparent;
            lblSlogan.Location = new Point(45, 300);
            pnlLeft.Controls.Add(lblSlogan);

            // 3. Phần Nhập liệu (Bên phải)

            // Tiêu đề
            lblTieuDe = new Label();
            lblTieuDe.Text = "WELCOME BACK";
            lblTieuDe.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(64, 64, 64);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(460, 60);

            // -- CỤM NHẬP USER (Đã chỉnh tọa độ không bị che chữ) --
            lblUserIcon = new Label();
            lblUserIcon.Text = "👤";
            lblUserIcon.Font = new Font("Segoe UI", 16);
            lblUserIcon.ForeColor = Color.FromArgb(0, 122, 204);
            lblUserIcon.Location = new Point(400, 160); // Icon giữ nguyên
            lblUserIcon.AutoSize = true;

            txtUser = new TextBox();
            txtUser.BorderStyle = BorderStyle.None;
            txtUser.Font = new Font("Segoe UI", 13);
            txtUser.ForeColor = Color.DimGray;
            txtUser.Text = "Tên đăng nhập";
            // ĐÃ SỬA: Tăng X lên 460 (Cách icon 60px) cho thoáng
            txtUser.Location = new Point(460, 165);
            txtUser.Width = 280; // Giảm độ rộng lại chút cho vừa khung
            // Sự kiện Placeholder & Đổi màu gạch chân
            txtUser.Enter += (s, e) => {
                if (txtUser.Text == "Tên đăng nhập") { txtUser.Text = ""; txtUser.ForeColor = Color.Black; }
                pnlUserLine.BackColor = Color.FromArgb(0, 122, 204); // Xanh sáng
            };
            txtUser.Leave += (s, e) => {
                if (txtUser.Text == "") { txtUser.Text = "Tên đăng nhập"; txtUser.ForeColor = Color.DimGray; }
                pnlUserLine.BackColor = Color.Silver; // Xám
            };

            pnlUserLine = new Panel();
            pnlUserLine.BackColor = Color.Silver;
            pnlUserLine.Size = new Size(340, 2);
            pnlUserLine.Location = new Point(400, 195);

            // -- CỤM NHẬP PASS (Đã chỉnh tọa độ) --
            lblPassIcon = new Label();
            lblPassIcon.Text = "🔒";
            lblPassIcon.Font = new Font("Segoe UI", 16);
            lblPassIcon.ForeColor = Color.FromArgb(0, 122, 204);
            lblPassIcon.Location = new Point(400, 240);
            lblPassIcon.AutoSize = true;

            txtPass = new TextBox();
            txtPass.BorderStyle = BorderStyle.None;
            txtPass.Font = new Font("Segoe UI", 13);
            txtPass.ForeColor = Color.DimGray;
            txtPass.Text = "Mật khẩu";
            // ĐÃ SỬA: Tăng X lên 460
            txtPass.Location = new Point(460, 245);
            txtPass.Width = 280;
            txtPass.Enter += (s, e) => {
                if (txtPass.Text == "Mật khẩu") { txtPass.Text = ""; txtPass.PasswordChar = '•'; txtPass.ForeColor = Color.Black; }
                pnlPassLine.BackColor = Color.FromArgb(0, 122, 204);
            };
            txtPass.Leave += (s, e) => {
                if (txtPass.Text == "") { txtPass.Text = "Mật khẩu"; txtPass.PasswordChar = '\0'; txtPass.ForeColor = Color.DimGray; }
                pnlPassLine.BackColor = Color.Silver;
            };

            pnlPassLine = new Panel();
            pnlPassLine.BackColor = Color.Silver;
            pnlPassLine.Size = new Size(340, 2);
            pnlPassLine.Location = new Point(400, 275);

            // -- NÚT ĐĂNG NHẬP (Vẽ Gradient) --
            btnLogin = new Button();
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Size = new Size(340, 50);
            btnLogin.Location = new Point(400, 330);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Paint += new PaintEventHandler(btnLogin_Paint); // Vẽ màu nút
            btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // -- LINK QUÊN MẬT KHẨU --
            lnkQuenMatKhau = new LinkLabel();
            lnkQuenMatKhau.Text = "Quên mật khẩu?";
            lnkQuenMatKhau.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lnkQuenMatKhau.LinkColor = Color.DimGray;
            lnkQuenMatKhau.ActiveLinkColor = Color.FromArgb(0, 122, 204);
            lnkQuenMatKhau.AutoSize = true;
            lnkQuenMatKhau.Location = new Point(620, 390);
            lnkQuenMatKhau.Cursor = Cursors.Hand;
            lnkQuenMatKhau.Click += (s, e) => MessageBox.Show("Vui lòng liên hệ Giáo vụ Khoa để được cấp lại mật khẩu!", "Hỗ trợ");

            // -- NÚT THOÁT (Góc trên phải) --
            btnExit = new Button();
            btnExit.Text = "X";
            btnExit.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnExit.ForeColor = Color.Gray;
            btnExit.BackColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Size = new Size(40, 40);
            btnExit.Location = new Point(760, 0);
            btnExit.Cursor = Cursors.Hand;
            btnExit.Click += (s, e) => Application.Exit();
            btnExit.MouseEnter += (s, e) => { btnExit.ForeColor = Color.Red; btnExit.BackColor = Color.FromArgb(255, 230, 230); };
            btnExit.MouseLeave += (s, e) => { btnExit.ForeColor = Color.Gray; btnExit.BackColor = Color.White; };

            // Thêm tất cả vào Form
            this.Controls.Add(pnlLeft);
            this.Controls.Add(lblTieuDe);
            this.Controls.Add(lblUserIcon);
            this.Controls.Add(txtUser);
            this.Controls.Add(pnlUserLine);
            this.Controls.Add(lblPassIcon);
            this.Controls.Add(txtPass);
            this.Controls.Add(pnlPassLine);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lnkQuenMatKhau);
            this.Controls.Add(btnExit);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // --- PHẦN 2: CÁC SỰ KIỆN VẼ MÀU (GRADIENT) ---

        // 1. Vẽ nền Gradient cho Panel Trái
        private void pnlLeft_Paint(object sender, PaintEventArgs e)
        {
            // Màu Xanh đậm HCMUTE -> Xanh sáng
            Color colorStart = Color.FromArgb(0, 51, 153);
            Color colorEnd = Color.FromArgb(0, 153, 204);

            LinearGradientBrush brush = new LinearGradientBrush(
                pnlLeft.ClientRectangle,
                colorStart,
                colorEnd,
                45F); // Góc nghiêng 45 độ

            e.Graphics.FillRectangle(brush, pnlLeft.ClientRectangle);
        }

        // 2. Vẽ nền Gradient cho Nút Đăng nhập
        private void btnLogin_Paint(object sender, PaintEventArgs e)
        {
            Color colorStart = Color.FromArgb(0, 153, 204);
            Color colorEnd = Color.FromArgb(0, 51, 153);

            LinearGradientBrush brush = new LinearGradientBrush(
                btnLogin.ClientRectangle,
                colorStart,
                colorEnd,
                0F); // Nằm ngang

            e.Graphics.FillRectangle(brush, btnLogin.ClientRectangle);

            // Vẽ chữ lên trên nền màu
            TextRenderer.DrawText(e.Graphics, btnLogin.Text, btnLogin.Font,
                btnLogin.ClientRectangle, btnLogin.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // --- PHẦN 3: XỬ LÝ LOGIC ---

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = (txtUser.Text == "Tên đăng nhập") ? "" : txtUser.Text.Trim();
            string pass = (txtPass.Text == "Mật khẩu") ? "" : txtPass.Text.Trim();

            if (user == "" || pass == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gọi BUS kiểm tra
                if (busTaiKhoan.CheckLogin(user, pass))
                {
                    string quyen = busTaiKhoan.GetQuyen(user);

                    MessageBox.Show("Đăng nhập thành công!\nXin chào: " + quyen, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    frmMain f = new frmMain(user, quyen);
                    f.ShowDialog();
                    this.Show(); // Hiện lại form đăng nhập khi thoát form chính

                    // Reset lại ô nhập
                    txtPass.Text = "Mật khẩu";
                    txtPass.PasswordChar = '\0';
                    txtPass.ForeColor = Color.DimGray;
                    pnlPassLine.BackColor = Color.Silver;
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
            }
        }

        // --- PHẦN 4: KÉO THẢ FORM KHÔNG VIỀN ---
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void pnlTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmDangNhap
            // 
            this.ClientSize = new System.Drawing.Size(531, 253);
            this.Name = "frmDangNhap";
            this.ResumeLayout(false);

        }
    }
}