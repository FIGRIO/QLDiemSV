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

        private TextBox txtMSSV, txtHoTen, txtEmail, txtSDT, txtDiaChi, txtMaLop;
        private DateTimePicker dtpNgaySinh;
        private RadioButton rdoNam, rdoNu;
        private Button btnThem, btnSua, btnXoa, btnLamMoi;

        BUS_SinhVien busSV = new BUS_SinhVien();

        public frmQLSinhVien()
        {
            InitializeComponent_Final();
            LoadData();
        }

        private void InitializeComponent_Final()
        {
            // 1. Cài đặt Form
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // A. PHẦN BẢNG DỮ LIỆU (ADD ĐẦU TIÊN -> NẰM DƯỚI CÙNG)
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

            // --- QUAN TRỌNG: CHẶN KHÔNG CHO SỬA TRỰC TIẾP TRÊN BẢNG ---
            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.AllowUserToAddRows = false; // Tắt dòng trống ở cuối bảng
            dgvSinhVien.RowHeadersVisible = false;  // Tắt cột mũi tên bên trái cho gọn

            // Style Header bảng
            dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSinhVien.ColumnHeadersHeight = 40;
            dgvSinhVien.EnableHeadersVisualStyles = false;

            dgvSinhVien.CellClick += DgvSinhVien_CellClick;
            pnlTable.Controls.Add(dgvSinhVien);
            this.Controls.Add(pnlTable);

            // =========================================================================
            // B. PHẦN NHẬP LIỆU (ADD THỨ HAI -> NẰM GIỮA)
            // =========================================================================
            pnlInput = new Panel();
            pnlInput.Dock = DockStyle.Top;
            pnlInput.Height = 240;
            pnlInput.BackColor = Color.White;
            pnlInput.Padding = new Padding(20);
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // --- GRID SYSTEM CĂN CHỈNH ---
            // HÀNG 1
            CreateLabel(pnlInput, "Mã số SV:", 30, 23);
            txtMSSV = CreateTextBox(pnlInput, 110, 20, 200);

            CreateLabel(pnlInput, "Họ và tên:", 350, 23);
            txtHoTen = CreateTextBox(pnlInput, 430, 20, 220);

            CreateLabel(pnlInput, "Ngày sinh:", 690, 23);
            dtpNgaySinh = new DateTimePicker { Location = new Point(770, 20), Format = DateTimePickerFormat.Short, Size = new Size(150, 27), Font = new Font("Segoe UI", 10) };
            pnlInput.Controls.Add(dtpNgaySinh);

            // HÀNG 2
            CreateLabel(pnlInput, "Giới tính:", 30, 73);
            rdoNam = new RadioButton { Text = "Nam", Location = new Point(110, 73), Checked = true, AutoSize = true, Font = new Font("Segoe UI", 10) };
            rdoNu = new RadioButton { Text = "Nữ", Location = new Point(180, 73), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlInput.Controls.Add(rdoNam); pnlInput.Controls.Add(rdoNu);

            CreateLabel(pnlInput, "Lớp SH:", 350, 73);
            txtMaLop = CreateTextBox(pnlInput, 430, 70, 220);

            CreateLabel(pnlInput, "SĐT:", 690, 73);
            txtSDT = CreateTextBox(pnlInput, 770, 70, 150);

            // HÀNG 3
            CreateLabel(pnlInput, "Email:", 30, 123);
            txtEmail = CreateTextBox(pnlInput, 110, 120, 200);

            CreateLabel(pnlInput, "Địa chỉ:", 350, 123);
            txtDiaChi = CreateTextBox(pnlInput, 430, 120, 490);

            // HÀNG 4: NÚT BẤM
            btnThem = CreateButton("Thêm", 250, 180, Color.FromArgb(40, 167, 69));
            btnSua = CreateButton("Sửa", 370, 180, Color.FromArgb(255, 193, 7));
            btnXoa = CreateButton("Xóa", 490, 180, Color.FromArgb(220, 53, 69));
            btnLamMoi = CreateButton("Làm mới", 610, 180, Color.Gray);

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetControl();

            // =========================================================================
            // C. PHẦN TIÊU ĐỀ (ADD CUỐI CÙNG -> NẰM TRÊN ĐỈNH)
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
            b.Size = new Size(100, 38);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = bg;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 10, FontStyle.Bold);
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
            dtpNgaySinh.Value = DateTime.Now; rdoNam.Checked = true;
            txtMSSV.Focus();
        }

        private void DgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow r = dgvSinhVien.Rows[e.RowIndex];
                txtMSSV.Text = r.Cells["MSSV"].Value.ToString();
                txtMSSV.Enabled = false; // Khóa MSSV khi chọn

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
                if (busSV.ThemSV(sv)) { MessageBox.Show("Thêm thành công!"); LoadData(); ResetControl(); }
                else MessageBox.Show("Thêm thất bại!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (txtMSSV.Enabled == true) { MessageBox.Show("Vui lòng chọn SV để sửa!"); return; }
            if (!CheckInput()) return;
            DTO_SinhVien sv = new DTO_SinhVien(txtMSSV.Text.Trim(), txtHoTen.Text.Trim(), dtpNgaySinh.Value, rdoNam.Checked ? "Nam" : "Nữ", txtEmail.Text.Trim(), txtSDT.Text.Trim(), txtDiaChi.Text.Trim(), txtMaLop.Text.Trim());
            if (busSV.SuaSV(sv)) { MessageBox.Show("Cập nhật thành công!"); LoadData(); ResetControl(); } else MessageBox.Show("Lỗi cập nhật!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (txtMSSV.Enabled == true) { MessageBox.Show("Vui lòng chọn SV để xóa!"); return; }
            if (MessageBox.Show("Xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (busSV.XoaSV(txtMSSV.Text)) { MessageBox.Show("Đã xóa!"); LoadData(); ResetControl(); } else MessageBox.Show("Lỗi xóa!");
            }
        }
    }
}