using System;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLSinhVien : Form
    {
        // --- KHAI BÁO CONTROL ---
        private Panel pnlHeader;
        private Label lblHeader;
        private Panel pnlInput;
        private Panel pnlTable;
        private DataGridView dgvSinhVien;

        // Control Tìm kiếm
        private Panel pnlSearch;
        private TextBox txtTimKiem;
        private Button btnTimKiem;
        private Label lblTimKiem;

        private TextBox txtMSSV, txtHoTen, txtEmail, txtSDT, txtDiaChi, txtMaLop;
        private DateTimePicker dtpNgaySinh;
        private RadioButton rdoNam, rdoNu;
        private Button btnThem, btnSua, btnXoa, btnLamMoi;

        BUS_SinhVien busSV = new BUS_SinhVien();

        // Biến hằng cho Placeholder
        private const string PLACEHOLDER_TEXT = "Nhập MSSV / Tên";

        public frmQLSinhVien()
        {
            InitializeComponent_Final_UI();
            LoadData();
        }

        private void InitializeComponent_Final_UI()
        {
            // 1. Cài đặt Form
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // 1. BẢNG DỮ LIỆU (ADD ĐẦU TIÊN)
            // =========================================================================
            pnlTable = new Panel();
            pnlTable.Dock = DockStyle.Fill;
            pnlTable.Padding = new Padding(10);

            dgvSinhVien = new DataGridView();
            dgvSinhVien.Dock = DockStyle.Fill;
            dgvSinhVien.BackgroundColor = Color.White;
            dgvSinhVien.BorderStyle = BorderStyle.None;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.RowHeadersVisible = false;

            dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSinhVien.ColumnHeadersHeight = 40;
            dgvSinhVien.EnableHeadersVisualStyles = false;
            dgvSinhVien.CellClick += DgvSinhVien_CellClick;

            pnlTable.Controls.Add(dgvSinhVien);
            this.Controls.Add(pnlTable);

            // =========================================================================
            // 2. THANH TÌM KIẾM (CÓ PLACEHOLDER)
            // =========================================================================
            pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.White;
            pnlSearch.Padding = new Padding(20, 10, 20, 0);

            lblTimKiem = new Label();
            lblTimKiem.Text = "Tìm kiếm:"; // Ngắn gọn lại
            lblTimKiem.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTimKiem.AutoSize = true;
            lblTimKiem.Location = new Point(30, 15);
            pnlSearch.Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox();
            txtTimKiem.Location = new Point(120, 12); // Dịch sang trái chút vì Label ngắn đi
            txtTimKiem.Size = new Size(370, 27);      // Dài ra để chứa chữ
            txtTimKiem.Font = new Font("Segoe UI", 10);

            // --- XỬ LÝ PLACEHOLDER ---
            txtTimKiem.Text = PLACEHOLDER_TEXT;
            txtTimKiem.ForeColor = Color.Gray; // Màu chữ mờ
            txtTimKiem.Enter += (s, e) => {
                if (txtTimKiem.Text == PLACEHOLDER_TEXT)
                {
                    txtTimKiem.Text = "";
                    txtTimKiem.ForeColor = Color.Black; // Gõ thì màu đen
                }
            };
            txtTimKiem.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
                {
                    txtTimKiem.Text = PLACEHOLDER_TEXT;
                    txtTimKiem.ForeColor = Color.Gray;
                }
            };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnTimKiem_Click(null, null); };
            pnlSearch.Controls.Add(txtTimKiem);

            btnTimKiem = new Button();
            btnTimKiem.Text = "Tìm";
            btnTimKiem.Location = new Point(500, 11);
            btnTimKiem.Size = new Size(80, 29);
            btnTimKiem.BackColor = Color.FromArgb(12, 59, 124);
            btnTimKiem.ForeColor = Color.White;
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.FlatAppearance.BorderSize = 0;
            btnTimKiem.Cursor = Cursors.Hand;
            btnTimKiem.Click += BtnTimKiem_Click;
            pnlSearch.Controls.Add(btnTimKiem);

            pnlSearch.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 48, pnlSearch.Width - 20, 48); };
            this.Controls.Add(pnlSearch);

            // =========================================================================
            // 3. KHUNG NHẬP LIỆU (NÚT 2x2 GÓC PHẢI)
            // =========================================================================
            pnlInput = new Panel();
            pnlInput.Dock = DockStyle.Top;
            pnlInput.Height = 170;
            pnlInput.BackColor = Color.White;
            pnlInput.Padding = new Padding(20);
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // --- HÀNG 1 (Y=20) ---
            CreateLabel(pnlInput, "Mã số SV:", 20, 23);
            txtMSSV = CreateTextBox(pnlInput, 95, 20, 170);

            CreateLabel(pnlInput, "Họ và tên:", 290, 23);
            txtHoTen = CreateTextBox(pnlInput, 370, 20, 180);

            CreateLabel(pnlInput, "Ngày sinh:", 570, 23);
            dtpNgaySinh = new DateTimePicker { Location = new Point(650, 20), Format = DateTimePickerFormat.Short, Size = new Size(130, 27), Font = new Font("Segoe UI", 10) };
            pnlInput.Controls.Add(dtpNgaySinh);

            // --- HÀNG 2 (Y=70) ---
            CreateLabel(pnlInput, "Giới tính:", 20, 73);
            rdoNam = new RadioButton { Text = "Nam", Location = new Point(95, 73), Checked = true, AutoSize = true, Font = new Font("Segoe UI", 10) };
            rdoNu = new RadioButton { Text = "Nữ", Location = new Point(160, 73), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlInput.Controls.Add(rdoNam); pnlInput.Controls.Add(rdoNu);

            CreateLabel(pnlInput, "Lớp SH:", 290, 73);
            txtMaLop = CreateTextBox(pnlInput, 370, 70, 180);

            CreateLabel(pnlInput, "SĐT:", 570, 73);
            txtSDT = CreateTextBox(pnlInput, 650, 70, 130);

            // --- HÀNG 3 (Y=120) ---
            CreateLabel(pnlInput, "Email:", 20, 123);
            txtEmail = CreateTextBox(pnlInput, 95, 120, 170);

            CreateLabel(pnlInput, "Địa chỉ:", 290, 123);
            txtDiaChi = CreateTextBox(pnlInput, 370, 120, 410);

            // --- KHU VỰC NÚT BẤM (GÓC PHẢI - LƯỚI 2x2) ---
            btnThem = CreateButton("Thêm", 810, 15, Color.FromArgb(40, 167, 69));
            btnSua = CreateButton("Sửa", 905, 15, Color.FromArgb(255, 193, 7));
            btnXoa = CreateButton("Xóa", 810, 65, Color.FromArgb(220, 53, 69));
            btnLamMoi = CreateButton("Làm mới", 905, 65, Color.Gray);

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetControl();

            // =========================================================================
            // 4. HEADER (ADD CUỐI CÙNG)
            // =========================================================================
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;
            pnlHeader.BackColor = Color.FromArgb(242, 244, 248);

            lblHeader = new Label();
            lblHeader.Text = "  ➤  QUẢN LÝ SINH VIÊN";
            lblHeader.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(12, 59, 124);
            lblHeader.AutoSize = false;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            lblHeader.Padding = new Padding(10, 0, 0, 0);
            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };

            this.Controls.Add(pnlHeader);
        }

        // --- HÀM HỖ TRỢ UI ---
        private void CreateLabel(Panel p, string text, int x, int y)
        {
            Label l = new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Regular) };
            p.Controls.Add(l);
        }
        private TextBox CreateTextBox(Panel p, int x, int y, int w)
        {
            TextBox t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) };
            p.Controls.Add(t);
            return t;
        }
        private Button CreateButton(string text, int x, int y, Color bg)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(85, 35);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = bg;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            pnlInput.Controls.Add(b);
            return b;
        }

        // --- LOGIC NGHIỆP VỤ ---

        private void LoadData()
        {
            dgvSinhVien.DataSource = busSV.GetDanhSachSV();
        }

        private void ResetControl()
        {
            txtMSSV.Clear(); txtMSSV.Enabled = true;
            txtHoTen.Clear(); txtEmail.Clear(); txtSDT.Clear(); txtDiaChi.Clear(); txtMaLop.Clear();

            // Reset ô tìm kiếm về trạng thái Placeholder
            txtTimKiem.Text = PLACEHOLDER_TEXT;
            txtTimKiem.ForeColor = Color.Gray;

            dtpNgaySinh.Value = DateTime.Now; rdoNam.Checked = true;
            LoadData();
            txtMSSV.Focus();
        }

        // Xử lý Tìm kiếm (Cập nhật logic cho Placeholder)
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();

            // Nếu đang là chữ "Nhập MSSV / Tên" hoặc rỗng -> Load tất cả
            if (string.IsNullOrEmpty(keyword) || keyword == PLACEHOLDER_TEXT)
            {
                LoadData();
            }
            else
            {
                dgvSinhVien.DataSource = busSV.TimKiemSV(keyword);
            }
        }

        private void DgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow r = dgvSinhVien.Rows[e.RowIndex];
                txtMSSV.Text = r.Cells["MSSV"].Value.ToString();
                txtMSSV.Enabled = false;

                txtHoTen.Text = r.Cells["HoTen"].Value.ToString();
                if (r.Cells["NgaySinh"].Value != DBNull.Value) dtpNgaySinh.Value = Convert.ToDateTime(r.Cells["NgaySinh"].Value);

                string gt = r.Cells["GioiTinh"].Value.ToString();
                rdoNam.Checked = (gt == "Nam");
                rdoNu.Checked = !rdoNam.Checked;

                txtEmail.Text = r.Cells["Email"].Value.ToString();
                txtSDT.Text = r.Cells["SDT"].Value.ToString();
                txtDiaChi.Text = r.Cells["DiaChi"].Value.ToString();
                txtMaLop.Text = r.Cells["MaLop"].Value.ToString();
            }
        }

        private bool CheckInput()
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text)) { MessageBox.Show("Chưa nhập MSSV!"); txtMSSV.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text)) { MessageBox.Show("Chưa nhập Họ tên!"); txtHoTen.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtMaLop.Text)) { MessageBox.Show("Chưa nhập Lớp!"); txtMaLop.Focus(); return false; }
            return true;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            DTO_SinhVien sv = new DTO_SinhVien(txtMSSV.Text.Trim(), txtHoTen.Text.Trim(), dtpNgaySinh.Value, rdoNam.Checked ? "Nam" : "Nữ", txtEmail.Text.Trim(), txtSDT.Text.Trim(), txtDiaChi.Text.Trim(), txtMaLop.Text.Trim());
            try
            {
                if (busSV.ThemSV(sv)) { MessageBox.Show("Thêm thành công!"); ResetControl(); }
                else MessageBox.Show("Thêm thất bại!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (txtMSSV.Enabled == true) { MessageBox.Show("Vui lòng chọn SV để sửa!"); return; }
            if (!CheckInput()) return;
            DTO_SinhVien sv = new DTO_SinhVien(txtMSSV.Text.Trim(), txtHoTen.Text.Trim(), dtpNgaySinh.Value, rdoNam.Checked ? "Nam" : "Nữ", txtEmail.Text.Trim(), txtSDT.Text.Trim(), txtDiaChi.Text.Trim(), txtMaLop.Text.Trim());
            if (busSV.SuaSV(sv)) { MessageBox.Show("Cập nhật thành công!"); ResetControl(); } else MessageBox.Show("Lỗi cập nhật!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (txtMSSV.Enabled == true) { MessageBox.Show("Vui lòng chọn SV để xóa!"); return; }
            if (MessageBox.Show("Xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (busSV.XoaSV(txtMSSV.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi xóa!");
            }
        }
    }
}