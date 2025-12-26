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

        // Container
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

        private Form currentChildForm;
        private string _tenDangNhap;
        private string _quyenHan;

        public frmMain(string tenDangNhap, string quyenHan)
        {
            this._tenDangNhap = tenDangNhap;
            this._quyenHan = quyenHan;
            InitializeComponent_Correct_Layout();
        }

        private void InitializeComponent_Correct_Layout()
        {
            // 1. Cài đặt Form
            this.Size = new Size(1300, 750);
            this.Text = "HỆ THỐNG QUẢN LÝ ĐÀO TẠO";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Font = new Font("Segoe UI", 10);

            // =================================================================================
            // BƯỚC 1: TẠO CÁC PANEL
            // =================================================================================

            // A. SIDEBAR (CỘT TRÁI)
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 270;
            pnlSidebar.BackColor = Color.White;
            pnlSidebar.Paint += (s, e) => { e.Graphics.DrawLine(Pens.Silver, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height); };

            // B. HEADER (THANH XANH)
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = Color.FromArgb(12, 59, 124);

            // C. CONTENT
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(242, 244, 248);

            // =================================================================================
            // BƯỚC 2: ADD VÀO FORM & SẮP XẾP LỚP (QUAN TRỌNG NHẤT)
            // =================================================================================

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);

            // --- CHÌA KHÓA SỬA LỖI Ở ĐÂY ---
            // SendToBack() => Đẩy Sidebar xuống đáy lớp Z-Order.
            // Trong WinForms, lớp dưới cùng sẽ được Dock TRƯỚC TIÊN.
            // => Sidebar sẽ chiếm trọn chiều cao bên trái.
            pnlSidebar.SendToBack();

            // BringToFront() => Đưa Content lên trên cùng.
            // => Header sẽ nằm ở lớp giữa, chiếm phần Top của vùng còn lại (bên phải Sidebar).
            pnlContent.BringToFront();


            // =================================================================================
            // BƯỚC 3: CẤU TRÚC SIDEBAR
            // =================================================================================

            // 3.1 MENU (Add trước)
            flowMenuContainer = new FlowLayoutPanel();
            flowMenuContainer.Dock = DockStyle.Fill;
            flowMenuContainer.FlowDirection = FlowDirection.TopDown;
            flowMenuContainer.WrapContents = false;
            flowMenuContainer.AutoScroll = true;
            // Fix thanh cuộn
            flowMenuContainer.HorizontalScroll.Maximum = 0;
            flowMenuContainer.AutoScroll = false;
            flowMenuContainer.VerticalScroll.Visible = true;
            flowMenuContainer.AutoScroll = true;
            pnlSidebar.Controls.Add(flowMenuContainer);

            // 3.2 USER (Add nhì)
            pnlUserContainer = new Panel();
            pnlUserContainer.Dock = DockStyle.Top;
            pnlUserContainer.Height = 100;
            pnlUserContainer.BackColor = Color.FromArgb(250, 252, 255);
            pnlUserContainer.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 99, 250, 99); };
            pnlSidebar.Controls.Add(pnlUserContainer);

            // 3.3 LOGO (Add cuối -> Lên đỉnh)
            pnlLogoContainer = new Panel();
            pnlLogoContainer.Dock = DockStyle.Top;
            pnlLogoContainer.Height = 160;
            pnlLogoContainer.BackColor = Color.White;
            pnlSidebar.Controls.Add(pnlLogoContainer);


            // =================================================================================
            // BƯỚC 4: ĐIỀN NỘI DUNG
            // =================================================================================

            // --- LOGO ---
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

            // --- USER ---
            pbAvatar = new PictureBox();
            pbAvatar.Size = new Size(60, 60);
            pbAvatar.Location = new Point(20, 20);
            pbAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
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

            // --- MENU ---
            TaoMenu_FlowLayout();

            // =================================================================================
            // BƯỚC 5: HEADER TEXT (CĂN TRÁI + DỊCH PHẢI 30px)
            // =================================================================================
            lblTieuDeHeader = new Label();
            lblTieuDeHeader.Text = "TRƯỜNG ĐẠI HỌC SƯ PHẠM KỸ THUẬT TP.HCM";
            lblTieuDeHeader.ForeColor = Color.White;
            lblTieuDeHeader.Font = new Font("Segoe UI", 16, FontStyle.Bold);

            lblTieuDeHeader.AutoSize = false;
            lblTieuDeHeader.Dock = DockStyle.Fill;
            lblTieuDeHeader.TextAlign = ContentAlignment.MiddleLeft; // Căn lề trái
            lblTieuDeHeader.Padding = new Padding(30, 0, 0, 0);      // Dịch sang phải 30px

            pnlHeader.Controls.Add(lblTieuDeHeader);
        }

        private void TaoMenu_FlowLayout()
        {
            flowMenuContainer.Controls.Clear();

            AddLabelCategory("DANH MỤC QUẢN LÝ");
            if (_quyenHan == "Admin")
            {
                AddButton("Sinh viên", OpenQLSinhVien);
                AddButton("Giảng viên", null);
                AddButton("Môn học", null);
                AddButton("Lớp học phần", null);
            }
            else if (_quyenHan == "GiangVien")
            {
                AddButton("Nhập điểm", null);
                AddButton("Lịch giảng dạy", null);
            }
            else
            {
                AddButton("Kết quả học tập", null);
                AddButton("Đăng ký môn học", null);
                AddButton("Thời khóa biểu", null);
            }

            AddLabelCategory("HỆ THỐNG");
            AddButton("Đổi mật khẩu", null);
            AddButton("Đăng xuất", Logout);
        }

        private void AddButton(string text, EventHandler action)
        {
            Button btn = new Button();
            btn.Width = 250;
            btn.Height = 50;
            btn.Text = "      " + text;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.FromArgb(64, 64, 64);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(0);

            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(235, 245, 255);
                btn.ForeColor = Color.FromArgb(12, 59, 124);
                btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(12, 59, 124);
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
            lbl.Padding = new Padding(20, 20, 0, 5);
            lbl.AutoSize = true;
            lbl.MinimumSize = new Size(250, 40);
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
        }

        private void OpenQLSinhVien(object sender, EventArgs e)
        {
            OpenChildForm(new frmQLSinhVien(), "Quản lý Sinh viên");
        }

        private void Logout(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}