using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLDiemSV_GUI
{
    public partial class frmMain : Form
    {
        // --- KHAI BÁO COMPONENT ---
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;

        // Container bên trong Sidebar
        private Panel pnlLogoContainer;
        private Panel pnlUserContainer;
        private FlowLayoutPanel flowMenuContainer;

        // Control chi tiết
        private PictureBox pbLogoTruong;
        private Label lblTenTruongLogo;
        private PictureBox pbAvatar;
        private Label lblTenUser;
        private Label lblVaiTro;
        private Label lblTieuDeHeader;

        // Quản lý Form con
        private Form currentChildForm;
        private string _tenDangNhap;
        private string _quyenHan; // "Admin", "GiangVien", hoặc "SinhVien"

        // Constructor nhận thông tin đăng nhập
        public frmMain(string tenDangNhap, string quyenHan)
        {
            this._tenDangNhap = tenDangNhap;
            this._quyenHan = quyenHan;
            InitializeComponent_Layout();
        }

        private void InitializeComponent_Layout()
        {
            // 1. Cài đặt Form chính
            this.Size = new Size(1300, 750);
            this.Text = "HỆ THỐNG QUẢN LÝ ĐÀO TẠO";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Font = new Font("Segoe UI", 10);

            // =================================================================================
            // TẠO CÁC PANEL CHÍNH
            // =================================================================================

            // A. SIDEBAR (TRÁI)
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 270;
            pnlSidebar.BackColor = Color.White;
            // Vẽ đường kẻ dọc ngăn cách
            pnlSidebar.Paint += (s, e) => { e.Graphics.DrawLine(Pens.Silver, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height); };

            // B. HEADER (TRÊN)
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = Color.FromArgb(12, 59, 124); // Xanh đậm thương hiệu

            // C. CONTENT (GIỮA)
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(242, 244, 248); // Xám rất nhạt

            // --- QUAN TRỌNG: SẮP XẾP LỚP (Z-ORDER) ---
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);

            pnlSidebar.SendToBack(); // Dock Left chạy trước
            pnlContent.BringToFront(); // Fill phần còn lại

            // =================================================================================
            // CẤU TRÚC BÊN TRONG SIDEBAR
            // =================================================================================

            // 1. MENU (Dưới cùng, tự giãn)
            flowMenuContainer = new FlowLayoutPanel();
            flowMenuContainer.Dock = DockStyle.Fill;
            flowMenuContainer.FlowDirection = FlowDirection.TopDown;
            flowMenuContainer.WrapContents = false;
            flowMenuContainer.AutoScroll = true; // Cho phép cuộn nếu menu dài
            pnlSidebar.Controls.Add(flowMenuContainer);

            // 2. USER INFO (Giữa)
            pnlUserContainer = new Panel();
            pnlUserContainer.Dock = DockStyle.Top;
            pnlUserContainer.Height = 100;
            pnlUserContainer.BackColor = Color.FromArgb(250, 252, 255);
            pnlUserContainer.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 99, 250, 99); };
            pnlSidebar.Controls.Add(pnlUserContainer);

            // 3. LOGO (Trên cùng)
            pnlLogoContainer = new Panel();
            pnlLogoContainer.Dock = DockStyle.Top;
            pnlLogoContainer.Height = 160;
            pnlLogoContainer.BackColor = Color.White;
            pnlSidebar.Controls.Add(pnlLogoContainer);

            // =================================================================================
            // ĐIỀN NỘI DUNG CHI TIẾT
            // =================================================================================

            // --- LOGO AREA ---
            pbLogoTruong = new PictureBox();
            pbLogoTruong.Size = new Size(100, 100);
            pbLogoTruong.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogoTruong.Location = new Point(85, 20);
            try { pbLogoTruong.Image = Properties.Resources.hcmute_logo; } catch { pbLogoTruong.BackColor = Color.WhiteSmoke; }
            pnlLogoContainer.Controls.Add(pbLogoTruong);

            lblTenTruongLogo = new Label();
            lblTenTruongLogo.Text = "HCMUTE PORTAL";
            lblTenTruongLogo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTenTruongLogo.ForeColor = Color.FromArgb(12, 59, 124);
            lblTenTruongLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblTenTruongLogo.Size = new Size(270, 30);
            lblTenTruongLogo.Location = new Point(0, 125);
            pnlLogoContainer.Controls.Add(lblTenTruongLogo);

            // --- USER AREA ---
            pbAvatar = new PictureBox();
            pbAvatar.Size = new Size(60, 60);
            pbAvatar.Location = new Point(20, 20);
            pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            // Bo tròn Avatar
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, 59, 59);
            pbAvatar.Region = new Region(gp);
            try { pbAvatar.Load("https://cdn-icons-png.flaticon.com/512/3135/3135715.png"); } catch { pbAvatar.BackColor = Color.Gray; }
            pnlUserContainer.Controls.Add(pbAvatar);

            lblTenUser = new Label();
            lblTenUser.Text = _tenDangNhap;
            lblTenUser.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTenUser.AutoSize = true;
            lblTenUser.Location = new Point(90, 25);
            pnlUserContainer.Controls.Add(lblTenUser);

            lblVaiTro = new Label();
            lblVaiTro.Text = "• " + _quyenHan;
            lblVaiTro.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblVaiTro.ForeColor = Color.Green;
            lblVaiTro.AutoSize = true;
            lblVaiTro.Location = new Point(92, 50);
            pnlUserContainer.Controls.Add(lblVaiTro);

            // --- MENU AREA (Logic phân quyền nằm ở đây) ---
            TaoMenu_FlowLayout();

            // --- HEADER TEXT ---
            lblTieuDeHeader = new Label();
            lblTieuDeHeader.Text = "TRƯỜNG ĐẠI HỌC SƯ PHẠM KỸ THUẬT TP.HCM";
            lblTieuDeHeader.ForeColor = Color.White;
            lblTieuDeHeader.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTieuDeHeader.AutoSize = false;
            lblTieuDeHeader.Dock = DockStyle.Fill;
            lblTieuDeHeader.TextAlign = ContentAlignment.MiddleLeft;
            lblTieuDeHeader.Padding = new Padding(30, 0, 0, 0);
            pnlHeader.Controls.Add(lblTieuDeHeader);
        }

        // =================================================================================
        // HÀM TẠO MENU ĐỘNG (PHÂN QUYỀN)
        // =================================================================================
        private void TaoMenu_FlowLayout()
        {
            flowMenuContainer.Controls.Clear();

            // --- TRƯỜNG HỢP 1: ADMIN ---
            if (_quyenHan == "Admin")
            {
                AddLabelCategory("QUẢN TRỊ DỮ LIỆU");
                AddButton("Sinh viên", (s, e) => OpenChildForm(new frmQLSinhVien(), "Quản lý Sinh viên"));
                AddButton("Giảng viên", (s, e) => OpenChildForm(new frmQLGiangVien(), "Quản lý Giảng viên"));
                AddButton("Môn học", (s, e) => OpenChildForm(new frmQLMonHoc(), "Quản lý Môn học"));
                AddButton("Lớp học phần", (s, e) => OpenChildForm(new frmQLLopHocPhan(), "Quản lý Lớp Học Phần"));

                AddLabelCategory("NGHIỆP VỤ ĐÀO TẠO");
                // Admin cần vào đây để xếp lớp cho sinh viên
                AddButton("Đăng ký môn (Xếp lớp)", (s, e) => OpenChildForm(new frmDangKyMonHoc(), "Xếp lớp Sinh viên"));
                // Admin cần vào đây để sửa điểm nếu có sai sót (Hậu kiểm)
                AddButton("Quản lý Điểm", (s, e) => OpenChildForm(new frmNhapDiem(), "Quản lý Điểm (Admin)"));
            }
            // --- TRƯỜNG HỢP 2: GIẢNG VIÊN ---
            else if (_quyenHan == "GiangVien")
            {
                AddLabelCategory("GIẢNG DẠY");
                AddButton("Nhập điểm lớp dạy", (s, e) => OpenChildForm(new frmNhapDiem(_tenDangNhap), "Nhập điểm thành phần"));
                AddButton("Lịch giảng dạy", null); // Chưa làm
            }
            // --- TRƯỜNG HỢP 3: SINH VIÊN ---
            else
            {
                AddLabelCategory("HỌC TẬP");
                AddButton("Kết quả học tập", (s, e) => OpenChildForm(new frmXemDiem(_tenDangNhap), "Kết quả học tập"));
                AddButton("Đăng ký tín chỉ", null); // Chưa làm
                AddButton("Thời khóa biểu", null);  // Chưa làm
            }

            // --- PHẦN CHUNG CHO TẤT CẢ ---
            AddLabelCategory("HỆ THỐNG");
            AddButton("Đổi mật khẩu", (s, e) => new frmDoiMatKhau(_tenDangNhap).ShowDialog());
            AddButton("Đăng xuất", (s, e) => this.Close());
        }

        // =================================================================================
        // CÁC HÀM HỖ TRỢ (UI HELPER)
        // =================================================================================

        private void AddButton(string text, EventHandler action)
        {
            Button btn = new Button();
            btn.Width = 250;
            btn.Height = 50;
            btn.Text = "      " + text; // Khoảng trắng để thụt đầu dòng
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.FromArgb(64, 64, 64);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(0);

            // Hiệu ứng Hover xịn xò
            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(235, 245, 255);
                btn.ForeColor = Color.FromArgb(12, 59, 124);
                btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btn.FlatAppearance.BorderSize = 1; // Hiện viền nhẹ
                btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.White;
                btn.ForeColor = Color.FromArgb(64, 64, 64);
                btn.Font = new Font("Segoe UI", 10);
                btn.FlatAppearance.BorderSize = 0;
            };

            if (action != null) btn.Click += action;
            flowMenuContainer.Controls.Add(btn);
        }

        private void AddLabelCategory(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lbl.ForeColor = Color.Silver;
            lbl.Padding = new Padding(20, 20, 0, 5); // Căn lề
            lbl.AutoSize = true;
            lbl.MinimumSize = new Size(250, 40); // Chiếm dòng
            flowMenuContainer.Controls.Add(lbl);
        }

        private void OpenChildForm(Form childForm, string title)
        {
            if (currentChildForm != null) currentChildForm.Close();

            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;

            childForm.BringToFront();
            childForm.Show();

            // Cập nhật tiêu đề trên Header cho biết đang ở đâu
            lblTieuDeHeader.Text = "  ➤  " + title.ToUpper();
        }
    }
}