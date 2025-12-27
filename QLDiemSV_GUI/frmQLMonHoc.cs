using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLMonHoc : Form
    {
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvMonHoc;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;
        private Button btnExcel, btnPdf;

        private TextBox txtMaMH, txtTenMH;
        private NumericUpDown numTinChi;
        BUS_MonHoc busMH = new BUS_MonHoc();
        private const string PLACEHOLDER_TEXT = "Nhập Mã môn / Tên môn";

        public frmQLMonHoc()
        {
            InitializeComponent_MonHoc_Fixed();
            LoadData();
        }

        private void InitializeComponent_MonHoc_Fixed()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvMonHoc = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvMonHoc.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvMonHoc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMonHoc.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvMonHoc.CellClick += DgvMonHoc_CellClick;
            pnlTable.Controls.Add(dgvMonHoc);
            this.Controls.Add(pnlTable);

            pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(20, 10, 20, 0) };
            pnlSearch.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 48, pnlSearch.Width - 20, 48); };
            lblTimKiem = new Label { Text = "Tìm kiếm:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(30, 15) };
            pnlSearch.Controls.Add(lblTimKiem);
            txtTimKiem = new TextBox { Location = new Point(120, 12), Size = new Size(300, 27), Font = new Font("Segoe UI", 10), Text = PLACEHOLDER_TEXT, ForeColor = Color.Gray };
            txtTimKiem.Enter += (s, e) => { if (txtTimKiem.Text == PLACEHOLDER_TEXT) { txtTimKiem.Text = ""; txtTimKiem.ForeColor = Color.Black; } };
            txtTimKiem.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtTimKiem.Text)) { txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; } };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnTimKiem_Click(null, null); };
            pnlSearch.Controls.Add(txtTimKiem);

            btnTimKiem = CreateButton(pnlSearch, "Tìm", 430, 11, Color.FromArgb(12, 59, 124)); btnTimKiem.Click += BtnTimKiem_Click;
            btnExcel = CreateButton(pnlSearch, "Excel", 530, 11, Color.FromArgb(40, 167, 69)); btnExcel.Click += (s, e) => XuatFile("Excel");
            btnPdf = CreateButton(pnlSearch, "PDF", 620, 11, Color.FromArgb(220, 53, 69)); btnPdf.Click += (s, e) => XuatFile("PDF");
            this.Controls.Add(pnlSearch);

            pnlInput = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);
            CreateLabel(pnlInput, "Mã môn:", 50, 23); txtMaMH = CreateTextBox(pnlInput, 130, 20, 150);
            CreateLabel(pnlInput, "Tên môn học:", 320, 23); txtTenMH = CreateTextBox(pnlInput, 420, 20, 300);
            CreateLabel(pnlInput, "Số tín chỉ:", 50, 73); numTinChi = new NumericUpDown { Location = new Point(130, 70), Size = new Size(80, 27), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 10, Value = 3 }; pnlInput.Controls.Add(numTinChi);

            btnThem = CreateButton(pnlInput, "Thêm", 810, 15, Color.FromArgb(40, 167, 69)); btnThem.Click += BtnThem_Click;
            btnSua = CreateButton(pnlInput, "Sửa", 905, 15, Color.FromArgb(255, 193, 7)); btnSua.Click += BtnSua_Click;
            btnXoa = CreateButton(pnlInput, "Xóa", 810, 65, Color.FromArgb(220, 53, 69)); btnXoa.Click += BtnXoa_Click;
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 905, 65, Color.Gray); btnLamMoi.Click += (s, e) => ResetControl();

            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  QUẢN LÝ MÔN HỌC", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private Button CreateButton(Panel p, string text, int x, int y, Color bg) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; p.Controls.Add(b); return b; }

        private void XuatFile(string type)
        {
            if (dgvMonHoc.Rows.Count == 0) return;
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvMonHoc.Columns) if (col.Visible) dt.Columns.Add(col.HeaderText);
            foreach (DataGridViewRow row in dgvMonHoc.Rows)
            {
                DataRow r = dt.NewRow();
                foreach (DataGridViewColumn col in dgvMonHoc.Columns) if (col.Visible) r[col.HeaderText] = row.Cells[col.Name].Value;
                dt.Rows.Add(r);
            }
            string title = "DANH SÁCH MÔN HỌC";
            if (type == "Excel") ExcelHelper.XuatRaExcel(dt, "DS_MonHoc", title);
            else PdfHelper.XuatRaPdf(dt, title);
        }

        private void LoadData() => dgvMonHoc.DataSource = busMH.GetDS();
        private void ResetControl() { txtMaMH.Clear(); txtMaMH.Enabled = true; txtTenMH.Clear(); numTinChi.Value = 3; txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; LoadData(); }
        private void DgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) { var r = dgvMonHoc.Rows[e.RowIndex]; txtMaMH.Text = r.Cells["MaMH"].Value.ToString(); txtMaMH.Enabled = false; txtTenMH.Text = r.Cells["TenMH"].Value.ToString(); numTinChi.Value = Convert.ToDecimal(r.Cells["SoTinChi"].Value); } }
        private void BtnTimKiem_Click(object sender, EventArgs e) { string kw = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData(); else dgvMonHoc.DataSource = busMH.TimKiem(kw); }
        private void BtnThem_Click(object sender, EventArgs e) { if (string.IsNullOrWhiteSpace(txtMaMH.Text)) return; try { if (busMH.Them(new DTO_MonHoc(txtMaMH.Text, txtTenMH.Text, (int)numTinChi.Value))) { MessageBox.Show("Thêm thành công!"); ResetControl(); } else MessageBox.Show("Lỗi!"); } catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); } }
        private void BtnSua_Click(object sender, EventArgs e) { if (txtMaMH.Enabled) return; if (busMH.Sua(new DTO_MonHoc(txtMaMH.Text, txtTenMH.Text, (int)numTinChi.Value))) { MessageBox.Show("Sửa thành công!"); ResetControl(); } else MessageBox.Show("Lỗi!"); }
        private void BtnXoa_Click(object sender, EventArgs e) { if (txtMaMH.Enabled) return; if (MessageBox.Show("Xóa môn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes) if (busMH.Xoa(txtMaMH.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi!"); }
    }
}