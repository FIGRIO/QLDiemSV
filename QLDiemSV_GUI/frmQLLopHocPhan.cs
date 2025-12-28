using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLLopHocPhan : Form
    {
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvLopHP;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;
        private Button btnExcel, btnPdf;

        private TextBox txtMaLHP, txtNamHoc, txtPhong;
        private ComboBox cboMonHoc, cboGiangVien, cboHocKy, cboThu;
        private NumericUpDown numTLQT, numTLCK, numTietBD, numSoTiet;

        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_MonHoc busMH = new BUS_MonHoc();
        BUS_GiangVien busGV = new BUS_GiangVien();
        private const string PLACEHOLDER_TEXT = "Nhập Mã Lớp / Tên Môn";

        public frmQLLopHocPhan()
        {
            InitializeComponent_LHP();
            LoadComboBoxData();
            LoadData();
        }

        private void InitializeComponent_LHP()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvLopHP = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 40 };
            dgvLopHP.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvLopHP.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLopHP.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLopHP.CellClick += DgvLopHP_CellClick;
            pnlTable.Controls.Add(dgvLopHP);
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

            pnlInput = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // Hàng 1
            CreateLabel(pnlInput, "Mã LHP:", 20, 23); txtMaLHP = CreateTextBox(pnlInput, 95, 20, 150);
            CreateLabel(pnlInput, "Môn học:", 270, 23); cboMonHoc = CreateComboBox(pnlInput, 350, 20, 200);
            CreateLabel(pnlInput, "Giảng viên:", 580, 23); cboGiangVien = CreateComboBox(pnlInput, 660, 20, 180);

            // Hàng 2
            CreateLabel(pnlInput, "Học kỳ:", 20, 73); cboHocKy = CreateComboBox(pnlInput, 95, 70, 150); cboHocKy.Items.AddRange(new string[] { "1", "2", "3" }); cboHocKy.SelectedIndex = 0;
            CreateLabel(pnlInput, "Năm học:", 270, 73); txtNamHoc = CreateTextBox(pnlInput, 350, 70, 200); txtNamHoc.Text = "2024-2025";

            // Hàng 3
            CreateLabel(pnlInput, "% Quá trình:", 20, 123); numTLQT = new NumericUpDown { Location = new Point(115, 120), Size = new Size(60, 27), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100, Value = 30 }; pnlInput.Controls.Add(numTLQT);
            CreateLabel(pnlInput, "% Cuối kỳ:", 270, 123); numTLCK = new NumericUpDown { Location = new Point(350, 120), Size = new Size(60, 27), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100, Value = 70 }; pnlInput.Controls.Add(numTLCK);
            numTLQT.ValueChanged += (s, e) => { numTLCK.Value = 100 - numTLQT.Value; };

            // Hàng 4
            CreateLabel(pnlInput, "Thứ:", 20, 173); cboThu = CreateComboBox(pnlInput, 95, 170, 80); cboThu.Items.AddRange(new string[] { "2", "3", "4", "5", "6", "7", "CN" }); cboThu.SelectedIndex = 0;
            CreateLabel(pnlInput, "Tiết BĐ:", 190, 173); numTietBD = new NumericUpDown { Location = new Point(250, 170), Size = new Size(50, 27), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 15, Value = 1 }; pnlInput.Controls.Add(numTietBD);
            CreateLabel(pnlInput, "Số tiết:", 320, 173); numSoTiet = new NumericUpDown { Location = new Point(380, 170), Size = new Size(50, 27), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 5, Value = 3 }; pnlInput.Controls.Add(numSoTiet);
            CreateLabel(pnlInput, "Phòng:", 450, 173); txtPhong = CreateTextBox(pnlInput, 510, 170, 100);

            btnThem = CreateButton(pnlInput, "Thêm", 880, 20, Color.FromArgb(40, 167, 69)); btnThem.Click += BtnThem_Click;
            btnSua = CreateButton(pnlInput, "Sửa", 880, 70, Color.FromArgb(255, 193, 7)); btnSua.Click += BtnSua_Click;
            btnXoa = CreateButton(pnlInput, "Xóa", 980, 20, Color.FromArgb(220, 53, 69)); btnXoa.Click += BtnXoa_Click;
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 980, 70, Color.Gray); btnLamMoi.Click += (s, e) => ResetControl();

            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  QUẢN LÝ LỚP HỌC PHẦN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
            dgvLopHP.CellClick += DgvLopHP_CellClick;
        }

        private void CreateLabel(Panel p, string text, int x, int y) { p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) }); }
        private TextBox CreateTextBox(Panel p, int x, int y, int w) { var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) }; p.Controls.Add(t); return t; }
        private ComboBox CreateComboBox(Panel p, int x, int y, int w) { var c = new ComboBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList }; p.Controls.Add(c); return c; }
        private Button CreateButton(Panel p, string text, int x, int y, Color bg) { var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand }; b.FlatAppearance.BorderSize = 0; p.Controls.Add(b); return b; }

        private void XuatFile(string type)
        {
            if (dgvLopHP.Rows.Count == 0) return;
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvLopHP.Columns) if (col.Visible) dt.Columns.Add(col.HeaderText);
            foreach (DataGridViewRow row in dgvLopHP.Rows)
            {
                DataRow r = dt.NewRow();
                foreach (DataGridViewColumn col in dgvLopHP.Columns) if (col.Visible) r[col.HeaderText] = row.Cells[col.Name].Value;
                dt.Rows.Add(r);
            }
            string title = "DANH SÁCH LỚP HỌC PHẦN";
            if (type == "Excel") ExcelHelper.XuatRaExcel(dt, "DS_LopHP", title);
            else PdfHelper.XuatRaPdf(dt, title);
        }

        private void LoadComboBoxData()
        {
            cboMonHoc.DataSource = busMH.GetDS(); cboMonHoc.DisplayMember = "TenMH"; cboMonHoc.ValueMember = "MaMH";
            cboGiangVien.DataSource = busGV.GetDS(); cboGiangVien.DisplayMember = "HoTen"; cboGiangVien.ValueMember = "MaGV";
        }
        private void LoadData() => dgvLopHP.DataSource = busLHP.GetDS();
        private void ResetControl()
        {
            // Quan trọng: Phải mở khóa ô Mã Lớp để nhập mã mới
            txtMaLHP.Clear();
            txtMaLHP.Enabled = true; // <--- Dòng này quan trọng nhất
            txtMaLHP.Focus();

            // Reset các ô khác về mặc định
            cboMonHoc.SelectedIndex = (cboMonHoc.Items.Count > 0) ? 0 : -1;
            cboGiangVien.SelectedIndex = (cboGiangVien.Items.Count > 0) ? 0 : -1;
            cboHocKy.SelectedIndex = 0;
            txtNamHoc.Text = "2024-2025";
            numTLQT.Value = 30;
            numTLCK.Value = 70;

            // Reset lịch học
            cboThu.SelectedIndex = 0;
            numTietBD.Value = 1;
            numSoTiet.Value = 3;
            txtPhong.Clear();

            txtTimKiem.Text = PLACEHOLDER_TEXT;
            txtTimKiem.ForeColor = Color.Gray;

            LoadData(); // Tải lại bảng
        }
        private void DgvLopHP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvLopHP.Rows.Count)
            {
                try
                {
                    DataGridViewRow r = dgvLopHP.Rows[e.RowIndex];

                    // 1. Mã LHP & Các khóa ngoại
                    txtMaLHP.Text = r.Cells["MaLHP"].Value?.ToString();
                    txtMaLHP.Enabled = false; // Khóa lại không cho sửa mã

                    if (r.Cells["MaMon"].Value != null)
                        cboMonHoc.SelectedValue = r.Cells["MaMon"].Value.ToString();

                    if (r.Cells["MaGV"].Value != null)
                        cboGiangVien.SelectedValue = r.Cells["MaGV"].Value.ToString();

                    // 2. Học kỳ & Năm học
                    cboHocKy.Text = r.Cells["HocKy"].Value?.ToString();
                    txtNamHoc.Text = r.Cells["NamHoc"].Value?.ToString();

                    // 3. Tỷ lệ điểm (Lấy theo tên alias TyLeQuaTrinh đã đặt ở DAL)
                    if (r.Cells["TyLeQuaTrinh"].Value != DBNull.Value)
                        numTLQT.Value = Convert.ToDecimal(r.Cells["TyLeQuaTrinh"].Value);

                    if (r.Cells["TyLeCuoiKy"].Value != DBNull.Value)
                        numTLCK.Value = Convert.ToDecimal(r.Cells["TyLeCuoiKy"].Value);

                    // 4. Lịch học (Lấy theo tên alias)
                    if (r.Cells["Thu"].Value != DBNull.Value)
                    {
                        int thu = Convert.ToInt32(r.Cells["Thu"].Value);
                        cboThu.Text = (thu == 8) ? "CN" : thu.ToString();
                    }

                    if (r.Cells["TietBatDau"].Value != DBNull.Value) // Alias TietBatDau
                        numTietBD.Value = Convert.ToDecimal(r.Cells["TietBatDau"].Value);

                    if (r.Cells["SoTiet"].Value != DBNull.Value)
                        numSoTiet.Value = Convert.ToDecimal(r.Cells["SoTiet"].Value);

                    if (r.Cells["PhongHoc"].Value != DBNull.Value) // Alias PhongHoc
                        txtPhong.Text = r.Cells["PhongHoc"].Value.ToString();
                }
                catch (Exception ex)
                {
                    // Reset nếu lỗi để tránh crash
                    // ResetControl();
                    // MessageBox.Show("Lỗi chọn dòng: " + ex.Message);
                }
            }
        }
        private void BtnTimKiem_Click(object sender, EventArgs e) { string kw = txtTimKiem.Text.Trim(); if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData(); else dgvLopHP.DataSource = busLHP.TimKiem(kw); }
        private void BtnThem_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nhập liệu trống
            if (!CheckInput()) return;

            string maMoi = txtMaLHP.Text.Trim();

            // 2. KIỂM TRA TRÙNG MÃ (QUAN TRỌNG)
            // Kiểm tra trong DataGridView xem mã này đã có chưa
            DataTable dt = (DataTable)dgvLopHP.DataSource;
            DataRow[] foundRows = dt.Select($"MaLHP = '{maMoi}'");

            if (foundRows.Length > 0)
            {
                MessageBox.Show($"Mã lớp '{maMoi}' đã tồn tại rồi! Vui lòng nhập mã khác hoặc nhấn nút 'Làm mới' để nhập lại.",
                                "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLHP.Focus();
                return; // Dừng lại, không thêm nữa
            }

            // 3. Nếu không trùng thì mới thực hiện Thêm
            int thu = cboThu.Text == "CN" ? 8 : int.Parse(cboThu.Text);

            DTO_LopHocPhan lhp = new DTO_LopHocPhan(
                maMoi,
                cboMonHoc.SelectedValue.ToString(),
                cboGiangVien.SelectedValue.ToString(),
                cboHocKy.Text,
                txtNamHoc.Text,
                (float)numTLQT.Value,
                (float)numTLCK.Value,
                thu,
                (int)numTietBD.Value,
                (int)numSoTiet.Value,
                txtPhong.Text
            );

            if (busLHP.Them(lhp))
            {
                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetControl(); // Xóa trắng ô nhập để nhập tiếp
            }
            else
            {
                MessageBox.Show("Thêm thất bại! Vui lòng kiểm tra lại thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Hàm kiểm tra nhập liệu bắt buộc
        private bool CheckInput()
        {
            // Kiểm tra Mã Lớp Học Phần
            if (string.IsNullOrWhiteSpace(txtMaLHP.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp học phần!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLHP.Focus();
                return false;
            }

            // Kiểm tra Môn Học (ComboBox)
            if (cboMonHoc.SelectedIndex < 0 || cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Môn học!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMonHoc.Focus();
                return false;
            }

            // Kiểm tra Giảng Viên (ComboBox)
            if (cboGiangVien.SelectedIndex < 0 || cboGiangVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Giảng viên!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboGiangVien.Focus();
                return false;
            }

            // Kiểm tra Năm Học
            if (string.IsNullOrWhiteSpace(txtNamHoc.Text))
            {
                MessageBox.Show("Vui lòng nhập Năm học!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamHoc.Focus();
                return false;
            }

            // Kiểm tra tỷ lệ điểm (Tổng phải là 100% hoặc 1.0 tùy quy ước, ở đây chỉ check nhập số)
            // NumericUpDown (numTLQT, numTLCK) mặc định luôn có giá trị số nên không cần check null/empty

            return true; // Tất cả ok
        }
        private void BtnSua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn lớp để sửa chưa (Mã lớp không được trống)
            if (string.IsNullOrWhiteSpace(txtMaLHP.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp học phần cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra các ô nhập liệu khác
            if (!CheckInput()) return;

            // 3. Hỏi xác nhận
            if (MessageBox.Show($"Bạn có chắc muốn cập nhật thông tin cho lớp {txtMaLHP.Text}?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // Chuyển đổi dữ liệu Thứ
                    int thu = cboThu.Text == "CN" ? 8 : int.Parse(cboThu.Text);

                    // Tạo đối tượng DTO với thông tin mới
                    DTO_LopHocPhan lhp = new DTO_LopHocPhan(
                        txtMaLHP.Text.Trim(), // Mã lớp (Khóa chính, dùng để tìm dòng cần sửa)
                        cboMonHoc.SelectedValue.ToString(),
                        cboGiangVien.SelectedValue.ToString(),
                        cboHocKy.Text,
                        txtNamHoc.Text,
                        (float)numTLQT.Value,
                        (float)numTLCK.Value,
                        thu,
                        (int)numTietBD.Value,
                        (int)numSoTiet.Value,
                        txtPhong.Text
                    );

                    // Gọi BUS để sửa
                    if (busLHP.Sua(lhp))
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo");
                        ResetControl(); // Tải lại bảng và xóa trắng ô
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }
        }
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đã chọn lớp chưa
            if (string.IsNullOrWhiteSpace(txtMaLHP.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp học phần cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hỏi xác nhận (Rất quan trọng khi xóa)
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa lớp {txtMaLHP.Text} không?\nDữ liệu điểm liên quan cũng có thể bị mất!",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    // Gọi BUS để xóa
                    if (busLHP.Xoa(txtMaLHP.Text.Trim()))
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        ResetControl(); // Tải lại bảng
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Có thể lớp này đang chứa bảng điểm của sinh viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa. Lỗi: " + ex.Message);
                }
            }
        }
    }
}