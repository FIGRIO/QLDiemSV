using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLSinhVien : Form
    {
        // --- KHAI BÁO CONTROL ---
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvSinhVien;
        private TextBox txtTimKiem;

        // NÚT CHỨC NĂNG
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;
        private Button btnExcel, btnPdf; // <--- Thêm nút xuất file

        private TextBox txtMSSV, txtHoTen, txtEmail, txtSDT, txtDiaChi, txtMaLop;
        private DateTimePicker dtpNgaySinh;
        private RadioButton rdoNam, rdoNu;

        BUS_SinhVien busSV = new BUS_SinhVien();
        private const string PLACEHOLDER_TEXT = "Nhập MSSV / Tên";

        public frmQLSinhVien()
        {
            InitializeComponent_Final_UI();
            LoadData();
        }

        private void InitializeComponent_Final_UI()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvSinhVien = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSinhVien.CellClick += DgvSinhVien_CellClick;
            pnlTable.Controls.Add(dgvSinhVien);
            this.Controls.Add(pnlTable);

            // 2. SEARCH BAR (CÓ NÚT EXPORT)
            pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(20, 10, 20, 0) };
            pnlSearch.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 48, pnlSearch.Width - 20, 48); };

            lblTimKiem = new Label { Text = "Tìm kiếm:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(30, 15) };
            pnlSearch.Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox { Location = new Point(120, 12), Size = new Size(300, 27), Font = new Font("Segoe UI", 10), Text = PLACEHOLDER_TEXT, ForeColor = Color.Gray };
            txtTimKiem.Enter += (s, e) => { if (txtTimKiem.Text == PLACEHOLDER_TEXT) { txtTimKiem.Text = ""; txtTimKiem.ForeColor = Color.Black; } };
            txtTimKiem.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtTimKiem.Text)) { txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; } };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnTimKiem_Click(null, null); };
            pnlSearch.Controls.Add(txtTimKiem);

            btnTimKiem = CreateButton(pnlSearch, "Tìm", 430, 11, Color.FromArgb(12, 59, 124));
            btnTimKiem.Click += BtnTimKiem_Click;

            // --- THÊM 2 NÚT XUẤT FILE Ở ĐÂY ---
            btnExcel = CreateButton(pnlSearch, "Excel", 530, 11, Color.FromArgb(40, 167, 69));
            btnExcel.Click += (s, e) => XuatFile("Excel");

            btnPdf = CreateButton(pnlSearch, "PDF", 620, 11, Color.FromArgb(220, 53, 69));
            btnPdf.Click += (s, e) => XuatFile("PDF");

            this.Controls.Add(pnlSearch);

            // 3. INPUT PANEL
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // Hàng 1
            CreateLabel(pnlInput, "Mã số SV:", 20, 23); txtMSSV = CreateTextBox(pnlInput, 95, 20, 170);
            CreateLabel(pnlInput, "Họ và tên:", 290, 23); txtHoTen = CreateTextBox(pnlInput, 370, 20, 180);
            CreateLabel(pnlInput, "Ngày sinh:", 570, 23); dtpNgaySinh = new DateTimePicker { Location = new Point(650, 20), Format = DateTimePickerFormat.Short, Size = new Size(130, 27), Font = new Font("Segoe UI", 10) }; pnlInput.Controls.Add(dtpNgaySinh);

            // Hàng 2
            CreateLabel(pnlInput, "Giới tính:", 20, 73);
            rdoNam = new RadioButton { Text = "Nam", Location = new Point(95, 73), Checked = true, AutoSize = true, Font = new Font("Segoe UI", 10) };
            rdoNu = new RadioButton { Text = "Nữ", Location = new Point(160, 73), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlInput.Controls.Add(rdoNam); pnlInput.Controls.Add(rdoNu);
            CreateLabel(pnlInput, "Lớp SH:", 290, 73); txtMaLop = CreateTextBox(pnlInput, 370, 70, 180);
            CreateLabel(pnlInput, "SĐT:", 570, 73); txtSDT = CreateTextBox(pnlInput, 650, 70, 130);

            // Hàng 3
            CreateLabel(pnlInput, "Email:", 20, 123); txtEmail = CreateTextBox(pnlInput, 95, 120, 170);
            CreateLabel(pnlInput, "Địa chỉ:", 290, 123); txtDiaChi = CreateTextBox(pnlInput, 370, 120, 410);

            // Nút CRUD
            btnThem = CreateButton(pnlInput, "Thêm", 810, 15, Color.FromArgb(40, 167, 69)); btnThem.Click += BtnThem_Click;
            btnSua = CreateButton(pnlInput, "Sửa", 905, 15, Color.FromArgb(255, 193, 7)); btnSua.Click += BtnSua_Click;
            btnXoa = CreateButton(pnlInput, "Xóa", 810, 65, Color.FromArgb(220, 53, 69)); btnXoa.Click += BtnXoa_Click;
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 905, 65, Color.Gray); btnLamMoi.Click += (s, e) => ResetControl();

            // 4. HEADER
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  QUẢN LÝ SINH VIÊN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        // --- HELPER & LOGIC ---
        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }

        private Button CreateButton(Panel p, string text, int x, int y, Color bg)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            p.Controls.Add(b);
            return b;
        }

        // --- HÀM XUẤT FILE CHUNG ---
        private void XuatFile(string type)
        {
            if (dgvSinhVien.Rows.Count == 0) return;

            // Tạo DataTable với tên cột Tiếng Việt từ HeaderText
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvSinhVien.Columns)
                if (col.Visible) dt.Columns.Add(col.HeaderText);

            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                DataRow r = dt.NewRow();
                foreach (DataGridViewColumn col in dgvSinhVien.Columns)
                    if (col.Visible) r[col.HeaderText] = row.Cells[col.Name].Value;
                dt.Rows.Add(r);
            }

            string title = "DANH SÁCH SINH VIÊN";
            if (type == "Excel") ExcelHelper.XuatRaExcel(dt, "DS_SinhVien", title);
            else PdfHelper.XuatRaPdf(dt, title);
        }

        private void LoadData() => dgvSinhVien.DataSource = busSV.GetDanhSachSV();
        private void ResetControl() { txtMSSV.Clear(); txtMSSV.Enabled = true; txtHoTen.Clear(); txtEmail.Clear(); txtSDT.Clear(); txtDiaChi.Clear(); txtMaLop.Clear(); txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; dtpNgaySinh.Value = DateTime.Now; rdoNam.Checked = true; LoadData(); txtMSSV.Focus(); }
        private void BtnTimKiem_Click(object sender, EventArgs e) { string k = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(k) || k == PLACEHOLDER_TEXT) LoadData(); else dgvSinhVien.DataSource = busSV.TimKiemSV(k); }
        private void DgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) { var r = dgvSinhVien.Rows[e.RowIndex]; txtMSSV.Text = r.Cells["MSSV"].Value.ToString(); txtMSSV.Enabled = false; txtHoTen.Text = r.Cells["HoTen"].Value.ToString(); if (r.Cells["NgaySinh"].Value != DBNull.Value) dtpNgaySinh.Value = Convert.ToDateTime(r.Cells["NgaySinh"].Value); rdoNam.Checked = r.Cells["GioiTinh"].Value.ToString() == "Nam"; rdoNu.Checked = !rdoNam.Checked; txtEmail.Text = r.Cells["Email"].Value.ToString(); txtSDT.Text = r.Cells["SDT"].Value.ToString(); txtDiaChi.Text = r.Cells["DiaChi"].Value.ToString(); txtMaLop.Text = r.Cells["MaLop"].Value.ToString(); } }
        private bool CheckInput() { if (string.IsNullOrWhiteSpace(txtMSSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text)) { MessageBox.Show("Thiếu thông tin!"); return false; } return true; }
        private void BtnThem_Click(object sender, EventArgs e) { if (!CheckInput()) return; DTO_SinhVien sv = new DTO_SinhVien(txtMSSV.Text, txtHoTen.Text, dtpNgaySinh.Value, rdoNam.Checked ? "Nam" : "Nữ", txtEmail.Text, txtSDT.Text, txtDiaChi.Text, txtMaLop.Text); if (busSV.ThemSV(sv)) { MessageBox.Show("Thêm thành công!"); ResetControl(); } else MessageBox.Show("Lỗi thêm!"); }
        private void BtnSua_Click(object sender, EventArgs e) { if (txtMSSV.Enabled) return; DTO_SinhVien sv = new DTO_SinhVien(txtMSSV.Text, txtHoTen.Text, dtpNgaySinh.Value, rdoNam.Checked ? "Nam" : "Nữ", txtEmail.Text, txtSDT.Text, txtDiaChi.Text, txtMaLop.Text); if (busSV.SuaSV(sv)) { MessageBox.Show("Sửa thành công!"); ResetControl(); } else MessageBox.Show("Lỗi sửa!"); }
        private void BtnXoa_Click(object sender, EventArgs e) { if (txtMSSV.Enabled) return; if (MessageBox.Show("Xóa SV này?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes) if (busSV.XoaSV(txtMSSV.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi xóa!"); }
    }
}