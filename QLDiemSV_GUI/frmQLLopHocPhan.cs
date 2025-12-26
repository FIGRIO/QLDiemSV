using System;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLLopHocPhan : Form
    {
        // Control
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvLopHP;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;

        // Input
        private TextBox txtMaLHP, txtNamHoc;
        private ComboBox cboMonHoc, cboGiangVien, cboHocKy;
        private NumericUpDown numTLQT, numTLCK; // Thêm 2 ô nhập tỷ lệ

        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_MonHoc busMH = new BUS_MonHoc();
        BUS_GiangVien busGV = new BUS_GiangVien();

        private const string PLACEHOLDER_TEXT = "Nhập Mã Lớp HP / Tên Môn";

        public frmQLLopHocPhan()
        {
            InitializeComponent_LHP();
            LoadComboBoxData();
            LoadData();
        }

        private void InitializeComponent_LHP()
        {
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvLopHP = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvLopHP.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvLopHP.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLopHP.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLopHP.CellClick += DgvLopHP_CellClick;
            pnlTable.Controls.Add(dgvLopHP);
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

            // 3. INPUT (Tăng chiều cao để chứa thêm Tỷ lệ)
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // Row 1
            CreateLabel(pnlInput, "Mã LHP:", 20, 23); txtMaLHP = CreateTextBox(pnlInput, 95, 20, 150);
            CreateLabel(pnlInput, "Môn học:", 270, 23); cboMonHoc = CreateComboBox(pnlInput, 350, 20, 200);
            CreateLabel(pnlInput, "Giảng viên:", 580, 23); cboGiangVien = CreateComboBox(pnlInput, 660, 20, 180);

            // Row 2
            CreateLabel(pnlInput, "Học kỳ:", 20, 73);
            cboHocKy = CreateComboBox(pnlInput, 95, 70, 150);
            cboHocKy.Items.AddRange(new string[] { "1", "2", "3" });
            cboHocKy.SelectedIndex = 0;

            CreateLabel(pnlInput, "Năm học:", 270, 73); txtNamHoc = CreateTextBox(pnlInput, 350, 70, 200); txtNamHoc.Text = "2024-2025";

            // Row 3: Tỷ lệ điểm
            CreateLabel(pnlInput, "% Quá trình:", 20, 123);
            numTLQT = new NumericUpDown { Location = new Point(115, 120), Size = new Size(60, 27), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100, Value = 30 };
            pnlInput.Controls.Add(numTLQT);

            CreateLabel(pnlInput, "% Cuối kỳ:", 270, 123);
            numTLCK = new NumericUpDown { Location = new Point(350, 120), Size = new Size(60, 27), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100, Value = 70 };
            pnlInput.Controls.Add(numTLCK);

            // Tự động tính cuối kỳ khi nhập quá trình (Tổng = 100)
            numTLQT.ValueChanged += (s, e) => { numTLCK.Value = 100 - numTLQT.Value; };

            // BUTTONS
            btnThem = CreateButton(pnlInput, "Thêm", 880, 15, Color.FromArgb(40, 167, 69));
            btnSua = CreateButton(pnlInput, "Sửa", 880, 65, Color.FromArgb(255, 193, 7));
            btnXoa = CreateButton(pnlInput, "Xóa", 980, 15, Color.FromArgb(220, 53, 69));
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 980, 65, Color.Gray);

            btnThem.Size = new Size(80, 35); btnSua.Size = new Size(80, 35);
            btnXoa.Size = new Size(80, 35); btnLamMoi.Size = new Size(80, 35);

            btnThem.Click += BtnThem_Click; btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click; btnLamMoi.Click += (s, e) => ResetControl();

            // 4. HEADER
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  LỚP HỌC PHẦN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        // --- HELPER ---
        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private ComboBox CreateComboBox(Panel p, int x, int y, int w) { var c = new ComboBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList }; p.Controls.Add(c); return c; }
        private Button CreateButton(Panel parent, string text, int x, int y, Color bg) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(80, 29), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; parent.Controls.Add(b); return b; }

        // --- LOGIC ---
        private void LoadComboBoxData()
        {
            cboMonHoc.DataSource = busMH.GetDS();
            cboMonHoc.DisplayMember = "TenMon"; cboMonHoc.ValueMember = "MaMon";
            cboGiangVien.DataSource = busGV.GetDS();
            cboGiangVien.DisplayMember = "HoTen"; cboGiangVien.ValueMember = "MaGV";
        }

        private void LoadData() => dgvLopHP.DataSource = busLHP.GetDS();

        private void ResetControl()
        {
            txtMaLHP.Clear(); txtMaLHP.Enabled = true;
            cboMonHoc.SelectedIndex = 0; cboGiangVien.SelectedIndex = 0; cboHocKy.SelectedIndex = 0;
            txtNamHoc.Text = "2024-2025"; numTLQT.Value = 30; numTLCK.Value = 70;
            txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray;
            LoadData();
        }

        private void DgvLopHP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var r = dgvLopHP.Rows[e.RowIndex];
                try
                {
                    txtMaLHP.Text = r.Cells["MaLHP"].Value.ToString(); txtMaLHP.Enabled = false;
                    cboMonHoc.SelectedValue = r.Cells["MaMon"].Value.ToString();
                    cboGiangVien.SelectedValue = r.Cells["MaGV"].Value.ToString();
                    cboHocKy.Text = r.Cells["HocKy"].Value.ToString();
                    txtNamHoc.Text = r.Cells["NamHoc"].Value.ToString();
                    numTLQT.Value = Convert.ToDecimal(r.Cells["TyLeQuaTrinh"].Value);
                    numTLCK.Value = Convert.ToDecimal(r.Cells["TyLeCuoiKy"].Value);
                }
                catch { ResetControl(); }
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e) { string kw = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData(); else dgvLopHP.DataSource = busLHP.TimKiem(kw); }

        private bool CheckInput() { if (string.IsNullOrWhiteSpace(txtMaLHP.Text)) { MessageBox.Show("Nhập mã lớp HP!"); return false; } return true; }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            string maMon = cboMonHoc.SelectedValue.ToString();
            string maGV = cboGiangVien.SelectedValue.ToString();

            DTO_LopHocPhan lhp = new DTO_LopHocPhan(txtMaLHP.Text, maMon, maGV, cboHocKy.Text, txtNamHoc.Text, (float)numTLQT.Value, (float)numTLCK.Value);
            if (busLHP.Them(lhp)) { MessageBox.Show("Thêm thành công!"); ResetControl(); } else MessageBox.Show("Lỗi thêm!");
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (txtMaLHP.Enabled) return;
            DTO_LopHocPhan lhp = new DTO_LopHocPhan(txtMaLHP.Text, cboMonHoc.SelectedValue.ToString(), cboGiangVien.SelectedValue.ToString(), cboHocKy.Text, txtNamHoc.Text, (float)numTLQT.Value, (float)numTLCK.Value);
            if (busLHP.Sua(lhp)) { MessageBox.Show("Sửa thành công!"); ResetControl(); } else MessageBox.Show("Lỗi sửa!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaLHP.Enabled) return;
            if (MessageBox.Show("Xóa lớp này?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (busLHP.Xoa(txtMaLHP.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi xóa!");
        }
    }
}