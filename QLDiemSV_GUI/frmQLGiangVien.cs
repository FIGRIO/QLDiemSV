using System;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLGiangVien : Form
    {
        // Control
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvGiangVien;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;

        // Input fields (Theo đúng SQL: MaGV, HoTen, Email, SDT, MaKhoa)
        private TextBox txtMaGV, txtHoTen, txtEmail, txtSDT, txtMaKhoa;

        BUS_GiangVien busGV = new BUS_GiangVien();
        private const string PLACEHOLDER_TEXT = "Nhập Mã GV / Tên GV";

        public frmQLGiangVien()
        {
            InitializeComponent_GV_Fixed();
            LoadData();
        }

        private void InitializeComponent_GV_Fixed()
        {
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvGiangVien = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvGiangVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvGiangVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGiangVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvGiangVien.CellClick += DgvGiangVien_CellClick;
            pnlTable.Controls.Add(dgvGiangVien);
            this.Controls.Add(pnlTable);

            // 2. SEARCH
            pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(20, 10, 20, 0) };
            pnlSearch.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 48, pnlSearch.Width - 20, 48); };

            lblTimKiem = new Label { Text = "Tìm kiếm:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(30, 15) };
            pnlSearch.Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox { Location = new Point(120, 12), Size = new Size(370, 27), Font = new Font("Segoe UI", 10), Text = PLACEHOLDER_TEXT, ForeColor = Color.Gray };
            txtTimKiem.Enter += (s, e) => { if (txtTimKiem.Text == PLACEHOLDER_TEXT) { txtTimKiem.Text = ""; txtTimKiem.ForeColor = Color.Black; } };
            txtTimKiem.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtTimKiem.Text)) { txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; } };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnTimKiem_Click(null, null); };
            pnlSearch.Controls.Add(txtTimKiem);

            btnTimKiem = CreateButton(pnlSearch, "Tìm", 500, 11, Color.FromArgb(12, 59, 124));
            btnTimKiem.Size = new Size(80, 29);
            btnTimKiem.Click += BtnTimKiem_Click;
            this.Controls.Add(pnlSearch);

            // 3. INPUT
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // Row 1
            CreateLabel(pnlInput, "Mã GV:", 20, 23); txtMaGV = CreateTextBox(pnlInput, 95, 20, 170);
            CreateLabel(pnlInput, "Họ tên:", 290, 23); txtHoTen = CreateTextBox(pnlInput, 370, 20, 180);
            CreateLabel(pnlInput, "Mã Khoa:", 570, 23); txtMaKhoa = CreateTextBox(pnlInput, 650, 20, 130);

            // Row 2
            CreateLabel(pnlInput, "Email:", 20, 73); txtEmail = CreateTextBox(pnlInput, 95, 70, 455);
            CreateLabel(pnlInput, "SĐT:", 570, 73); txtSDT = CreateTextBox(pnlInput, 650, 70, 130);

            // BUTTONS (Góc phải)
            btnThem = CreateButton(pnlInput, "Thêm", 810, 15, Color.FromArgb(40, 167, 69));
            btnSua = CreateButton(pnlInput, "Sửa", 905, 15, Color.FromArgb(255, 193, 7));
            btnXoa = CreateButton(pnlInput, "Xóa", 810, 65, Color.FromArgb(220, 53, 69));
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 905, 65, Color.Gray);

            btnThem.Click += BtnThem_Click; btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click; btnLamMoi.Click += (s, e) => ResetControl();

            // 4. HEADER
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  QUẢN LÝ GIẢNG VIÊN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        // --- HELPER ---
        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private Button CreateButton(Panel parent, string text, int x, int y, Color bg) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(85, 35), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; parent.Controls.Add(b); return b; }

        // --- LOGIC ---
        private void LoadData() => dgvGiangVien.DataSource = busGV.GetDS();
        private void ResetControl() { txtMaGV.Clear(); txtMaGV.Enabled = true; txtHoTen.Clear(); txtEmail.Clear(); txtSDT.Clear(); txtMaKhoa.Clear(); txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; LoadData(); txtMaGV.Focus(); }

        private void DgvGiangVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var r = dgvGiangVien.Rows[e.RowIndex];
                try
                {
                    // Mapping cột theo đúng tên trong SQL
                    txtMaGV.Text = r.Cells["MaGV"].Value.ToString(); txtMaGV.Enabled = false;
                    txtHoTen.Text = r.Cells["HoTen"].Value.ToString();
                    txtEmail.Text = r.Cells["Email"].Value.ToString();
                    txtSDT.Text = r.Cells["SDT"].Value.ToString();
                    txtMaKhoa.Text = r.Cells["MaKhoa"].Value.ToString();
                }
                catch { ResetControl(); }
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e) { string kw = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData(); else dgvGiangVien.DataSource = busGV.TimKiem(kw); }

        private bool CheckInput() { if (string.IsNullOrWhiteSpace(txtMaGV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text)) { MessageBox.Show("Thiếu thông tin!"); return false; } return true; }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            DTO_GiangVien gv = new DTO_GiangVien(txtMaGV.Text, txtHoTen.Text, txtEmail.Text, txtSDT.Text, txtMaKhoa.Text);
            if (busGV.Them(gv)) { MessageBox.Show("Thêm thành công!"); ResetControl(); } else MessageBox.Show("Lỗi thêm!");
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (txtMaGV.Enabled) return;
            DTO_GiangVien gv = new DTO_GiangVien(txtMaGV.Text, txtHoTen.Text, txtEmail.Text, txtSDT.Text, txtMaKhoa.Text);
            if (busGV.Sua(gv)) { MessageBox.Show("Sửa thành công!"); ResetControl(); } else MessageBox.Show("Lỗi sửa!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaGV.Enabled) return;
            if (MessageBox.Show("Xóa giảng viên này?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (busGV.Xoa(txtMaGV.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi xóa!");
        }
    }
}