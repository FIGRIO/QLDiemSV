using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmDangKyMonHoc : Form
    {
        // Controls
        private Panel pnlHeader, pnlControl, pnlTable;
        private Label lblHeader, lblChonLop, lblChonSV, lblDS;
        private ComboBox cboLopHP, cboSinhVien;
        private Button btnDangKy, btnHuyDangKy;
        private Button btnExcel, btnPdf; // Nút xuất file
        private DataGridView dgvDS_Lop;

        private string _maGV;
        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_SinhVien busSV = new BUS_SinhVien();
        BUS_KetQua busKQ = new BUS_KetQua();

        public frmDangKyMonHoc(string maGV = "")
        {
            this._maGV = maGV;
            InitializeComponent_DK();
            LoadComboBoxData();
        }

        private void InitializeComponent_DK()
        {
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            lblDS = new Label { Text = "DANH SÁCH SINH VIÊN TRONG LỚP", Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.DimGray, TextAlign = ContentAlignment.BottomLeft };
            pnlTable.Controls.Add(lblDS);

            dgvDS_Lop = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            pnlTable.Controls.Add(dgvDS_Lop);
            pnlTable.Controls.SetChildIndex(dgvDS_Lop, 0);
            this.Controls.Add(pnlTable);

            // 2. CONTROL PANEL
            pnlControl = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.White, Padding = new Padding(20) };
            pnlControl.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlControl.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            lblChonLop = new Label { Text = "Chọn Lớp học phần:", Location = new Point(30, 30), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124) };
            pnlControl.Controls.Add(lblChonLop);

            cboLopHP = new ComboBox { Location = new Point(200, 27), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLopHP.SelectedIndexChanged += CboLopHP_SelectedIndexChanged;
            pnlControl.Controls.Add(cboLopHP);

            lblChonSV = new Label { Text = "Chọn Sinh viên:", Location = new Point(30, 80), AutoSize = true, Font = new Font("Segoe UI", 11) };
            pnlControl.Controls.Add(lblChonSV);

            cboSinhVien = new ComboBox { Location = new Point(200, 77), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDown, AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems };
            pnlControl.Controls.Add(cboSinhVien);

            btnDangKy = CreateButton(pnlControl, "Thêm Vào Lớp", 550, 25, Color.FromArgb(40, 167, 69)); btnDangKy.Width = 150; btnDangKy.Click += BtnDangKy_Click;
            btnHuyDangKy = CreateButton(pnlControl, "Xóa Khỏi Lớp", 720, 25, Color.FromArgb(220, 53, 69)); btnHuyDangKy.Width = 150; btnHuyDangKy.Click += BtnHuyDangKy_Click;

            // --- THÊM 2 NÚT XUẤT DANH SÁCH ---
            btnExcel = CreateButton(pnlControl, "Xuất DS Excel", 550, 75, Color.FromArgb(0, 123, 255)); // Màu xanh dương
            btnExcel.Width = 150;
            btnExcel.Click += (s, e) => XuatFile("Excel");

            btnPdf = CreateButton(pnlControl, "Xuất DS PDF", 720, 75, Color.FromArgb(255, 87, 34)); // Màu cam đỏ
            btnPdf.Width = 150;
            btnPdf.Click += (s, e) => XuatFile("PDF");
            // --------------------------------

            this.Controls.Add(pnlControl);

            // 3. HEADER
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            string tieuDe = string.IsNullOrEmpty(_maGV) ? "ĐĂNG KÝ MÔN HỌC (ADMIN)" : "QUẢN LÝ SINH VIÊN LỚP DẠY";
            lblHeader = new Label { Text = "  ➤  " + tieuDe, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        private Button CreateButton(Panel parent, string text, int x, int y, Color bg)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(120, 35), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            parent.Controls.Add(b);
            return b;
        }

        // --- HÀM XUẤT FILE DANH SÁCH LỚP ---
        private void XuatFile(string type)
        {
            if (dgvDS_Lop.Rows.Count == 0)
            {
                MessageBox.Show("Lớp này chưa có sinh viên nào!", "Thông báo");
                return;
            }

            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvDS_Lop.Columns)
                if (col.Visible) dt.Columns.Add(col.HeaderText);

            foreach (DataGridViewRow row in dgvDS_Lop.Rows)
            {
                DataRow r = dt.NewRow();
                foreach (DataGridViewColumn col in dgvDS_Lop.Columns)
                    if (col.Visible) r[col.HeaderText] = row.Cells[col.Name].Value;
                dt.Rows.Add(r);
            }

            string title = "DANH SÁCH LỚP " + cboLopHP.Text;
            if (type == "Excel") ExcelHelper.XuatRaExcel(dt, "DS_Lop", title);
            else PdfHelper.XuatRaPdf(dt, title);
        }

        // --- LOGIC ---
        private void LoadComboBoxData()
        {
            if (string.IsNullOrEmpty(_maGV) || _maGV.ToLower() == "admin") cboLopHP.DataSource = busLHP.GetDS();
            else cboLopHP.DataSource = busLHP.GetLopByGV(_maGV);
            cboLopHP.DisplayMember = "MaLHP"; cboLopHP.ValueMember = "MaLHP";

            cboSinhVien.DataSource = busSV.GetDanhSachSV();
            cboSinhVien.Format += (s, e) => { if (e.ListItem == null) return; var row = ((System.Data.DataRowView)e.ListItem).Row; e.Value = row["MSSV"] + " - " + row["HoTen"]; };
            cboSinhVien.ValueMember = "MSSV";
        }

        private void CboLopHP_SelectedIndexChanged(object sender, EventArgs e) { if (cboLopHP.SelectedValue != null) LoadDanhSachLop(cboLopHP.SelectedValue.ToString()); }
        private void LoadDanhSachLop(string maLHP) { dgvDS_Lop.DataSource = busKQ.GetDS_SV(maLHP); }
        private void BtnDangKy_Click(object sender, EventArgs e) { if (cboLopHP.SelectedValue == null || cboSinhVien.SelectedValue == null) { MessageBox.Show("Vui lòng chọn thông tin!"); return; } string maLHP = cboLopHP.SelectedValue.ToString(); string mssv = cboSinhVien.SelectedValue.ToString(); if (busKQ.DangKy(maLHP, mssv)) { MessageBox.Show("Đã thêm sinh viên!"); LoadDanhSachLop(maLHP); } else MessageBox.Show("Sinh viên đã có trong lớp!", "Cảnh báo"); }
        private void BtnHuyDangKy_Click(object sender, EventArgs e) { string mssv = ""; if (dgvDS_Lop.SelectedRows.Count > 0) mssv = dgvDS_Lop.SelectedRows[0].Cells["MSSV"].Value.ToString(); else if (cboSinhVien.SelectedValue != null) mssv = cboSinhVien.SelectedValue.ToString(); if (string.IsNullOrEmpty(mssv)) { MessageBox.Show("Chọn SV cần xóa!"); return; } if (MessageBox.Show("Xóa SV khỏi lớp?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes) if (busKQ.HuyDangKy(cboLopHP.SelectedValue.ToString(), mssv)) { MessageBox.Show("Đã xóa!"); LoadDanhSachLop(cboLopHP.SelectedValue.ToString()); } }
    }
}