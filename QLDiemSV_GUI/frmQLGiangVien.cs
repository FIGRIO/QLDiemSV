using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLGiangVien : Form
    {
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvGiangVien;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;
        private Button btnExcel, btnPdf; // Nút xuất

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
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvGiangVien = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvGiangVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvGiangVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGiangVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvGiangVien.CellClick += DgvGiangVien_CellClick;
            pnlTable.Controls.Add(dgvGiangVien);
            this.Controls.Add(pnlTable);

            // SEARCH PANEL
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

            // XUẤT FILE
            btnExcel = CreateButton(pnlSearch, "Excel", 530, 11, Color.FromArgb(40, 167, 69));
            btnExcel.Click += (s, e) => XuatFile("Excel");
            btnPdf = CreateButton(pnlSearch, "PDF", 620, 11, Color.FromArgb(220, 53, 69));
            btnPdf.Click += (s, e) => XuatFile("PDF");

            this.Controls.Add(pnlSearch);

            // INPUT
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);
            CreateLabel(pnlInput, "Mã GV:", 20, 23); txtMaGV = CreateTextBox(pnlInput, 95, 20, 170);
            CreateLabel(pnlInput, "Họ tên:", 290, 23); txtHoTen = CreateTextBox(pnlInput, 370, 20, 180);
            CreateLabel(pnlInput, "Mã Khoa:", 570, 23); txtMaKhoa = CreateTextBox(pnlInput, 650, 20, 130);
            CreateLabel(pnlInput, "Email:", 20, 73); txtEmail = CreateTextBox(pnlInput, 95, 70, 455);
            CreateLabel(pnlInput, "SĐT:", 570, 73); txtSDT = CreateTextBox(pnlInput, 650, 70, 130);

            btnThem = CreateButton(pnlInput, "Thêm", 810, 15, Color.FromArgb(40, 167, 69)); btnThem.Click += BtnThem_Click;
            btnSua = CreateButton(pnlInput, "Sửa", 905, 15, Color.FromArgb(255, 193, 7)); btnSua.Click += BtnSua_Click;
            btnXoa = CreateButton(pnlInput, "Xóa", 810, 65, Color.FromArgb(220, 53, 69)); btnXoa.Click += BtnXoa_Click;
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 905, 65, Color.Gray); btnLamMoi.Click += (s, e) => ResetControl();

            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  QUẢN LÝ GIẢNG VIÊN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private Button CreateButton(Panel p, string text, int x, int y, Color bg) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; p.Controls.Add(b); return b; }

        private void XuatFile(string type)
        {
            if (dgvGiangVien.Rows.Count == 0) return;
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvGiangVien.Columns) if (col.Visible) dt.Columns.Add(col.HeaderText);
            foreach (DataGridViewRow row in dgvGiangVien.Rows)
            {
                DataRow r = dt.NewRow();
                foreach (DataGridViewColumn col in dgvGiangVien.Columns) if (col.Visible) r[col.HeaderText] = row.Cells[col.Name].Value;
                dt.Rows.Add(r);
            }
            string title = "DANH SÁCH GIẢNG VIÊN";
            if (type == "Excel") ExcelHelper.XuatRaExcel(dt, "DS_GiangVien", title);
            else PdfHelper.XuatRaPdf(dt, title);
        }

        private void LoadData() => dgvGiangVien.DataSource = busGV.GetDS();
        private void ResetControl() { txtMaGV.Clear(); txtMaGV.Enabled = true; txtHoTen.Clear(); txtEmail.Clear(); txtSDT.Clear(); txtMaKhoa.Clear(); txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; LoadData(); txtMaGV.Focus(); }
        private void DgvGiangVien_CellClick(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) { var r = dgvGiangVien.Rows[e.RowIndex]; txtMaGV.Text = r.Cells["MaGV"].Value.ToString(); txtMaGV.Enabled = false; txtHoTen.Text = r.Cells["HoTen"].Value.ToString(); txtEmail.Text = r.Cells["Email"].Value.ToString(); txtSDT.Text = r.Cells["SDT"].Value.ToString(); txtMaKhoa.Text = r.Cells["MaKhoa"].Value.ToString(); } }
        private void BtnTimKiem_Click(object sender, EventArgs e) { string kw = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData(); else dgvGiangVien.DataSource = busGV.TimKiem(kw); }
        private void BtnThem_Click(object sender, EventArgs e) { if (string.IsNullOrWhiteSpace(txtMaGV.Text)) return; DTO_GiangVien gv = new DTO_GiangVien(txtMaGV.Text, txtHoTen.Text, txtEmail.Text, txtSDT.Text, txtMaKhoa.Text); if (busGV.Them(gv)) { MessageBox.Show("Thêm thành công!"); ResetControl(); } else MessageBox.Show("Lỗi!"); }
        private void BtnSua_Click(object sender, EventArgs e) { if (txtMaGV.Enabled) return; DTO_GiangVien gv = new DTO_GiangVien(txtMaGV.Text, txtHoTen.Text, txtEmail.Text, txtSDT.Text, txtMaKhoa.Text); if (busGV.Sua(gv)) { MessageBox.Show("Sửa thành công!"); ResetControl(); } else MessageBox.Show("Lỗi!"); }
        private void BtnXoa_Click(object sender, EventArgs e) { if (txtMaGV.Enabled) return; if (MessageBox.Show("Xóa GV này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes) if (busGV.Xoa(txtMaGV.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi!"); }
    }
}