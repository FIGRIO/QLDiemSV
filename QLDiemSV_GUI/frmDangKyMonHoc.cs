using System;
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
        private DataGridView dgvDS_Lop;

        // Data
        private string _maGV; // Biến lưu mã GV đăng nhập (Nếu là Admin thì rỗng)

        // BUS
        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_SinhVien busSV = new BUS_SinhVien();
        BUS_KetQua busKQ = new BUS_KetQua();

        // Constructor nhận thêm tham số maGV
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

            // =========================================================================
            // 1. BẢNG DỮ LIỆU (ADD ĐẦU TIÊN)
            // =========================================================================
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            lblDS = new Label
            {
                Text = "DANH SÁCH SINH VIÊN TRONG LỚP",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.BottomLeft
            };
            pnlTable.Controls.Add(lblDS);

            dgvDS_Lop = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40
            };
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDS_Lop.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            pnlTable.Controls.Add(dgvDS_Lop);
            pnlTable.Controls.SetChildIndex(dgvDS_Lop, 0);

            this.Controls.Add(pnlTable);

            // =========================================================================
            // 2. PANEL CONTROL (ĐIỀU KHIỂN) - ADD THỨ HAI
            // =========================================================================
            pnlControl = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.White, Padding = new Padding(20) };
            pnlControl.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlControl.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            // -- Chọn Lớp HP --
            lblChonLop = new Label { Text = "Chọn Lớp học phần:", Location = new Point(30, 30), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124) };
            pnlControl.Controls.Add(lblChonLop);

            cboLopHP = new ComboBox { Location = new Point(200, 27), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLopHP.SelectedIndexChanged += CboLopHP_SelectedIndexChanged;
            pnlControl.Controls.Add(cboLopHP);

            // -- Chọn Sinh viên --
            lblChonSV = new Label { Text = "Chọn Sinh viên:", Location = new Point(30, 80), AutoSize = true, Font = new Font("Segoe UI", 11) };
            pnlControl.Controls.Add(lblChonSV);

            // Combobox Tìm kiếm
            cboSinhVien = new ComboBox
            {
                Location = new Point(200, 77),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            pnlControl.Controls.Add(cboSinhVien);

            // -- Nút bấm --
            btnDangKy = CreateButton(pnlControl, "Thêm Vào Lớp", 550, 75, Color.FromArgb(40, 167, 69));
            btnDangKy.Width = 150;
            btnDangKy.Click += BtnDangKy_Click;

            btnHuyDangKy = CreateButton(pnlControl, "Xóa Khỏi Lớp", 720, 75, Color.FromArgb(220, 53, 69));
            btnHuyDangKy.Width = 150;
            btnHuyDangKy.Click += BtnHuyDangKy_Click;

            this.Controls.Add(pnlControl);

            // =========================================================================
            // 3. HEADER (ADD CUỐI CÙNG)
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };

            // Đổi tiêu đề linh hoạt theo vai trò
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

        // --- LOGIC ---

        private void LoadComboBoxData()
        {
            // 1. Load Lớp HP (Logic lọc theo quyền)
            if (string.IsNullOrEmpty(_maGV) || _maGV.ToLower() == "admin")
            {
                // Admin: Load hết
                cboLopHP.DataSource = busLHP.GetDS();
            }
            else
            {
                // Giảng viên: Chỉ load lớp mình dạy
                cboLopHP.DataSource = busLHP.GetLopByGV(_maGV);
            }

            cboLopHP.DisplayMember = "MaLHP";
            cboLopHP.ValueMember = "MaLHP";

            // 2. Load Tất cả Sinh viên
            cboSinhVien.DataSource = busSV.GetDanhSachSV();
            cboSinhVien.Format += (s, e) => {
                if (e.ListItem == null) return;
                var row = ((System.Data.DataRowView)e.ListItem).Row;
                e.Value = row["MSSV"] + " - " + row["HoTen"];
            };
            cboSinhVien.ValueMember = "MSSV";
        }

        private void CboLopHP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLopHP.SelectedValue != null)
                LoadDanhSachLop(cboLopHP.SelectedValue.ToString());
        }

        private void LoadDanhSachLop(string maLHP)
        {
            dgvDS_Lop.DataSource = busKQ.GetDS_SV(maLHP);
        }

        private void BtnDangKy_Click(object sender, EventArgs e)
        {
            if (cboLopHP.SelectedValue == null) return;

            if (cboSinhVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLHP = cboLopHP.SelectedValue.ToString();
            string mssv = cboSinhVien.SelectedValue.ToString();

            if (busKQ.DangKy(maLHP, mssv))
            {
                MessageBox.Show("Đã thêm sinh viên vào lớp!");
                LoadDanhSachLop(maLHP);
            }
            else
            {
                MessageBox.Show("Sinh viên này đã có trong lớp rồi!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnHuyDangKy_Click(object sender, EventArgs e)
        {
            string mssvCanXoa = "";
            string maLHP = cboLopHP.SelectedValue.ToString();

            if (dgvDS_Lop.SelectedRows.Count > 0)
                mssvCanXoa = dgvDS_Lop.SelectedRows[0].Cells["MSSV"].Value.ToString();
            else if (cboSinhVien.SelectedValue != null)
                mssvCanXoa = cboSinhVien.SelectedValue.ToString();

            if (string.IsNullOrEmpty(mssvCanXoa))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            if (MessageBox.Show("Xóa sinh viên " + mssvCanXoa + " khỏi lớp này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (busKQ.HuyDangKy(maLHP, mssvCanXoa))
                {
                    MessageBox.Show("Đã xóa khỏi lớp!");
                    LoadDanhSachLop(maLHP);
                }
            }
        }
    }
}