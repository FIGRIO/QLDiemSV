using System;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;
using QLDiemSV_DTO;

namespace QLDiemSV_GUI
{
    public partial class frmQLMonHoc : Form
    {
        // --- KHAI BÁO CONTROL ---
        private Panel pnlHeader, pnlInput, pnlTable, pnlSearch;
        private Label lblHeader, lblTimKiem;
        private DataGridView dgvMonHoc;
        private TextBox txtTimKiem;
        private Button btnTimKiem, btnThem, btnSua, btnXoa, btnLamMoi;

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
            // 1. Cài đặt Form
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // 2. BẢNG DỮ LIỆU (ADD ĐẦU TIÊN -> NẰM DƯỚI CÙNG)
            // =========================================================================
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            dgvMonHoc = new DataGridView
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
            dgvMonHoc.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvMonHoc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMonHoc.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvMonHoc.CellClick += DgvMonHoc_CellClick;

            pnlTable.Controls.Add(dgvMonHoc);
            this.Controls.Add(pnlTable);

            // =========================================================================
            // 3. THANH TÌM KIẾM (ADD THỨ 2)
            // =========================================================================
            pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(20, 10, 20, 0) };
            pnlSearch.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 20, 48, pnlSearch.Width - 20, 48); };

            lblTimKiem = new Label { Text = "Tìm kiếm:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(30, 15) };
            pnlSearch.Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox { Location = new Point(120, 12), Size = new Size(370, 27), Font = new Font("Segoe UI", 10), Text = PLACEHOLDER_TEXT, ForeColor = Color.Gray };
            // Sự kiện Placeholder
            txtTimKiem.Enter += (s, e) => { if (txtTimKiem.Text == PLACEHOLDER_TEXT) { txtTimKiem.Text = ""; txtTimKiem.ForeColor = Color.Black; } };
            txtTimKiem.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtTimKiem.Text)) { txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray; } };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnTimKiem_Click(null, null); };
            pnlSearch.Controls.Add(txtTimKiem);

            // --- SỬA LỖI Ở ĐÂY: Truyền pnlSearch vào hàm CreateButton ---
            btnTimKiem = CreateButton(pnlSearch, "Tìm", 500, 11, Color.FromArgb(12, 59, 124));
            btnTimKiem.Size = new Size(80, 29); // Chỉnh lại size riêng cho nút tìm
            btnTimKiem.Click += BtnTimKiem_Click;

            this.Controls.Add(pnlSearch);

            // =========================================================================
            // 4. KHUNG NHẬP LIỆU (ADD THỨ 3)
            // =========================================================================
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.White, Padding = new Padding(20) };
            pnlInput.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlInput.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };
            this.Controls.Add(pnlInput);

            // --- INPUT FIELDS ---
            CreateLabel(pnlInput, "Mã môn:", 50, 23);
            txtMaMH = CreateTextBox(pnlInput, 130, 20, 150);

            CreateLabel(pnlInput, "Tên môn học:", 320, 23);
            txtTenMH = CreateTextBox(pnlInput, 420, 20, 300);

            CreateLabel(pnlInput, "Số tín chỉ:", 50, 73);
            numTinChi = new NumericUpDown { Location = new Point(130, 70), Size = new Size(80, 27), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 10, Value = 3 };
            pnlInput.Controls.Add(numTinChi);

            // --- BUTTONS (Góc phải) ---
            // Truyền pnlInput vào hàm CreateButton
            btnThem = CreateButton(pnlInput, "Thêm", 810, 15, Color.FromArgb(40, 167, 69));
            btnSua = CreateButton(pnlInput, "Sửa", 905, 15, Color.FromArgb(255, 193, 7));
            btnXoa = CreateButton(pnlInput, "Xóa", 810, 65, Color.FromArgb(220, 53, 69));
            btnLamMoi = CreateButton(pnlInput, "Làm mới", 905, 65, Color.Gray);

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetControl();

            // =========================================================================
            // 5. HEADER (ADD CUỐI CÙNG)
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };

            lblHeader = new Label
            {
                Text = "  ➤  QUẢN LÝ MÔN HỌC",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 59, 124),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);
        }

        // --- CÁC HÀM HỖ TRỢ ---
        private void CreateLabel(Panel p, string text, int x, int y)
        {
            p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        }

        private TextBox CreateTextBox(Panel p, int x, int y, int w)
        {
            var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 27), Font = new Font("Segoe UI", 10) };
            p.Controls.Add(t);
            return t;
        }

        // --- SỬA HÀM NÀY: Thêm tham số 'parent' ---
        private Button CreateButton(Panel parent, string text, int x, int y, Color bg)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(85, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;

            // Thêm nút vào đúng Panel được truyền vào (tránh lỗi null)
            parent.Controls.Add(b);
            return b;
        }

        // --- LOGIC ---
        private void LoadData() => dgvMonHoc.DataSource = busMH.GetDS();

        private void ResetControl()
        {
            txtMaMH.Clear(); txtMaMH.Enabled = true;
            txtTenMH.Clear(); numTinChi.Value = 3;
            txtTimKiem.Text = PLACEHOLDER_TEXT; txtTimKiem.ForeColor = Color.Gray;
            LoadData();
        }

        private void DgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow r = dgvMonHoc.Rows[e.RowIndex];

                // SỬA TÊN CỘT Ở ĐÂY CHO KHỚP VỚI SQL
                txtMaMH.Text = r.Cells["MaMon"].Value.ToString();  // Cũ: "MaMH" -> Mới: "MaMon"
                txtMaMH.Enabled = false;

                txtTenMH.Text = r.Cells["TenMon"].Value.ToString(); // Cũ: "TenMH" -> Mới: "TenMon"

                // Cột SoTinChi tên giống nhau rồi nên giữ nguyên
                numTinChi.Value = Convert.ToDecimal(r.Cells["SoTinChi"].Value);
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            string kw = txtTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(kw) || kw == PLACEHOLDER_TEXT) LoadData();
            else dgvMonHoc.DataSource = busMH.TimKiem(kw);
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (txtMaMH.Text == "" || txtTenMH.Text == "") { MessageBox.Show("Vui lòng nhập đủ thông tin!"); return; }
            try
            {
                if (busMH.Them(new DTO_MonHoc(txtMaMH.Text, txtTenMH.Text, (int)numTinChi.Value)))
                { MessageBox.Show("Thêm thành công!"); ResetControl(); }
                else MessageBox.Show("Thêm thất bại (Trùng mã)!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (txtMaMH.Enabled) { MessageBox.Show("Chọn môn cần sửa!"); return; }
            if (busMH.Sua(new DTO_MonHoc(txtMaMH.Text, txtTenMH.Text, (int)numTinChi.Value)))
            { MessageBox.Show("Sửa thành công!"); ResetControl(); }
            else MessageBox.Show("Lỗi sửa!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaMH.Enabled) { MessageBox.Show("Chọn môn cần xóa!"); return; }
            if (MessageBox.Show("Xóa môn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (busMH.Xoa(txtMaMH.Text)) { MessageBox.Show("Đã xóa!"); ResetControl(); } else MessageBox.Show("Lỗi xóa (Có thể môn này đã có điểm)!");
            }
        }
    }
}