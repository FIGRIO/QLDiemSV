using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmNhapDiem : Form
    {
        // --- CONTROLS ---
        private Panel pnlHeader, pnlFilter, pnlControl, pnlTable; // Thêm pnlFilter
        private Label lblHeader, lblChonLop, lblTyLe, lblNamHoc, lblHocKy;
        private ComboBox cboLopHP, cboNamHoc, cboHocKy;
        private Button btnLuu;
        private DataGridView dgvDiem;

        // --- DATA ---
        private string _maGV;
        private DataTable _dtLopGoc; // Lưu danh sách lớp gốc để lọc

        // BUS
        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_KetQua busKQ = new BUS_KetQua();

        // Biến lưu tỷ lệ của lớp đang chọn
        private float _tyLeQT = 0.3f;
        private float _tyLeCK = 0.7f;

        public frmNhapDiem(string maGV = "")
        {
            this._maGV = maGV;
            InitializeComponent_Diem();
            LoadDataLop(); // Load dữ liệu ban đầu
        }

        private void InitializeComponent_Diem()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // 1. TABLE (ADD ĐẦU TIÊN - NẰM DƯỚI CÙNG)
            // =========================================================================
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvDiem = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                AllowUserToAddRows = false
            };
            dgvDiem.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvDiem.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDiem.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dgvDiem.CellValueChanged += DgvDiem_CellValueChanged;
            dgvDiem.EditingControlShowing += DgvDiem_EditingControlShowing;

            pnlTable.Controls.Add(dgvDiem);
            this.Controls.Add(pnlTable);

            // =========================================================================
            // 2. PANEL CONTROL (CHỌN LỚP ĐỂ NHẬP) - ADD THỨ HAI
            // =========================================================================
            pnlControl = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(20) };
            pnlControl.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlControl.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            lblChonLop = new Label { Text = "Chọn Lớp học phần:", Location = new Point(30, 28), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124) };
            pnlControl.Controls.Add(lblChonLop);

            cboLopHP = new ComboBox { Location = new Point(200, 25), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLopHP.SelectedIndexChanged += CboLopHP_SelectedIndexChanged;
            pnlControl.Controls.Add(cboLopHP);

            lblTyLe = new Label { Text = "(Tỷ lệ: -- / --)", Location = new Point(520, 28), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.DimGray };
            pnlControl.Controls.Add(lblTyLe);

            btnLuu = CreateButton(pnlControl, "💾 LƯU BẢNG ĐIỂM", 850, 20, Color.FromArgb(0, 123, 255));
            btnLuu.Width = 180;
            btnLuu.Click += BtnLuu_Click;

            this.Controls.Add(pnlControl);

            // =========================================================================
            // 3. PANEL FILTER (LỌC NĂM HỌC/HỌC KỲ) - ADD THỨ BA (MỚI)
            // =========================================================================
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
            // Vẽ đường kẻ dưới
            pnlFilter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.Silver, 0, 59, pnlFilter.Width, 59); };

            lblNamHoc = new Label { Text = "Năm học:", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlFilter.Controls.Add(lblNamHoc);

            cboNamHoc = new ComboBox { Location = new Point(110, 17), Width = 150, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboNamHoc.SelectedIndexChanged += FilterDSLop; // Sự kiện lọc
            pnlFilter.Controls.Add(cboNamHoc);

            lblHocKy = new Label { Text = "Học kỳ:", Location = new Point(290, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlFilter.Controls.Add(lblHocKy);

            cboHocKy = new ComboBox { Location = new Point(350, 17), Width = 100, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboHocKy.SelectedIndexChanged += FilterDSLop; // Sự kiện lọc
            pnlFilter.Controls.Add(cboHocKy);

            this.Controls.Add(pnlFilter);

            // =========================================================================
            // 4. HEADER (ADD CUỐI CÙNG)
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  NHẬP ĐIỂM THÀNH PHẦN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
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

        // --- DATA LOGIC ---

        private void LoadDataLop()
        {
            // 1. Lấy danh sách lớp gốc (Tất cả hoặc theo GV)
            if (string.IsNullOrEmpty(_maGV) || _maGV.ToLower() == "admin")
                _dtLopGoc = busLHP.GetDS();
            else
                _dtLopGoc = busLHP.GetLopByGV(_maGV);

            // 2. Nạp dữ liệu vào ComboBox Lọc (Năm/Học kỳ)
            LoadFilterCombos();

            // 3. Lọc lần đầu (Tự động chọn lớp đầu tiên)
            FilterDSLop(null, null);
        }

        private void LoadFilterCombos()
        {
            cboNamHoc.SelectedIndexChanged -= FilterDSLop;
            cboHocKy.SelectedIndexChanged -= FilterDSLop;

            cboNamHoc.Items.Clear(); cboHocKy.Items.Clear();
            cboNamHoc.Items.Add("Tất cả"); cboHocKy.Items.Add("Tất cả");

            if (_dtLopGoc != null)
            {
                // Lấy danh sách Năm học duy nhất
                DataTable dtNam = _dtLopGoc.DefaultView.ToTable(true, "NamHoc");
                foreach (DataRow r in dtNam.Rows) cboNamHoc.Items.Add(r["NamHoc"].ToString());

                // Lấy danh sách Học kỳ duy nhất
                DataTable dtHK = _dtLopGoc.DefaultView.ToTable(true, "HocKy");
                foreach (DataRow r in dtHK.Rows) cboHocKy.Items.Add(r["HocKy"].ToString());
            }

            cboNamHoc.SelectedIndex = 0;
            cboHocKy.SelectedIndex = 0;

            cboNamHoc.SelectedIndexChanged += FilterDSLop;
            cboHocKy.SelectedIndexChanged += FilterDSLop;
        }

        private void FilterDSLop(object sender, EventArgs e)
        {
            if (_dtLopGoc == null) return;

            string filter = "1=1"; // Mặc định lấy hết

            if (cboNamHoc.SelectedIndex > 0)
                filter += string.Format(" AND NamHoc = '{0}'", cboNamHoc.SelectedItem);

            if (cboHocKy.SelectedIndex > 0)
                filter += string.Format(" AND HocKy = '{0}'", cboHocKy.SelectedItem);

            // Áp dụng bộ lọc tạo ra DataView mới
            DataView dv = new DataView(_dtLopGoc);
            dv.RowFilter = filter;

            // Gán lại cho ComboBox chọn lớp
            cboLopHP.DataSource = dv;
            cboLopHP.DisplayMember = "MaLHP";
            cboLopHP.ValueMember = "MaLHP";

            // Nếu sau khi lọc không còn lớp nào
            if (dv.Count == 0)
            {
                dgvDiem.DataSource = null;
                lblTyLe.Text = "(Không có lớp nào)";
            }
        }

        private void CboLopHP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLopHP.SelectedValue == null) return;
            string maLHP = cboLopHP.SelectedValue.ToString();

            // SỬA LỖI BIẾN row: Khai báo trước khi dùng
            DataRowView row = (DataRowView)cboLopHP.SelectedItem;

            if (row["TyLeQuaTrinh"] != DBNull.Value && row["TyLeCuoiKy"] != DBNull.Value)
            {
                float rawQT = Convert.ToSingle(row["TyLeQuaTrinh"]);
                float rawCK = Convert.ToSingle(row["TyLeCuoiKy"]);

                _tyLeQT = (rawQT > 1) ? rawQT / 100 : rawQT;
                _tyLeCK = (rawCK > 1) ? rawCK / 100 : rawCK;
            }

            lblTyLe.Text = string.Format("(Tỷ lệ QT/CK: {0}% - {1}%)", _tyLeQT * 100, _tyLeCK * 100);

            // Load bảng điểm
            LoadBangDiem(maLHP);
        }

        private void LoadBangDiem(string maLHP)
        {
            try
            {
                DataTable dt = busKQ.GetBangDiem(maLHP);
                dgvDiem.DataSource = dt;

                if (dt != null && dt.Columns.Count > 0)
                {
                    if (dgvDiem.Columns.Contains("MSSV")) dgvDiem.Columns["MSSV"].ReadOnly = true;
                    if (dgvDiem.Columns.Contains("HoTen")) dgvDiem.Columns["HoTen"].ReadOnly = true;
                    if (dgvDiem.Columns.Contains("MaLHP")) dgvDiem.Columns["MaLHP"].Visible = false;

                    if (dgvDiem.Columns.Contains("DiemChuyenCan")) dgvDiem.Columns["DiemChuyenCan"].HeaderText = "Điểm CC";
                    if (dgvDiem.Columns.Contains("DiemGiuaKy")) dgvDiem.Columns["DiemGiuaKy"].HeaderText = "Giữa Kỳ";
                    if (dgvDiem.Columns.Contains("DiemCuoiKy")) dgvDiem.Columns["DiemCuoiKy"].HeaderText = "Cuối Kỳ";

                    if (dgvDiem.Columns.Contains("DiemTongKet"))
                    {
                        dgvDiem.Columns["DiemTongKet"].ReadOnly = true;
                        dgvDiem.Columns["DiemTongKet"].HeaderText = "Tổng Kết";
                        dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }

                    if (dgvDiem.Columns.Contains("DiemChu"))
                    {
                        dgvDiem.Columns["DiemChu"].ReadOnly = true;
                        dgvDiem.Columns["DiemChu"].HeaderText = "Điểm Chữ";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải điểm: " + ex.Message);
            }
        }

        // --- XỬ LÝ TÍNH ĐIỂM ---
        private void DgvDiem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            // Cột 2,3,4 là điểm (CC, GK, CK)
            if (dgvDiem.CurrentCell.ColumnIndex >= 2 && dgvDiem.CurrentCell.ColumnIndex <= 4)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null) tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
            }
        }
        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;
        }

        private void DgvDiem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string colName = dgvDiem.Columns[e.ColumnIndex].Name;
            if (colName == "DiemChuyenCan" || colName == "DiemGiuaKy" || colName == "DiemCuoiKy")
            {
                TinhDiemTongKet(e.RowIndex);
            }
        }

        private void TinhDiemTongKet(int rowIndex)
        {
            try
            {
                DataGridViewRow r = dgvDiem.Rows[rowIndex];
                float cc = r.Cells["DiemChuyenCan"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemChuyenCan"].Value);
                float gk = r.Cells["DiemGiuaKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemGiuaKy"].Value);
                float ck = r.Cells["DiemCuoiKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemCuoiKy"].Value);

                float diemQuaTrinh = (cc + gk) / 2;
                float diemTongKet = (diemQuaTrinh * _tyLeQT) + (ck * _tyLeCK);
                diemTongKet = (float)Math.Round(diemTongKet, 1);

                r.Cells["DiemTongKet"].Value = diemTongKet;

                string diemChu = "";
                if (diemTongKet >= 9.0) diemChu = "A+";
                else if (diemTongKet >= 8.5) diemChu = "A";
                else if (diemTongKet >= 8.0) diemChu = "B+";
                else if (diemTongKet >= 7.0) diemChu = "B";
                else if (diemTongKet >= 6.5) diemChu = "C+";
                else if (diemTongKet >= 5.5) diemChu = "C";
                else if (diemTongKet >= 5.0) diemChu = "D+";
                else if (diemTongKet >= 4.0) diemChu = "D";
                else diemChu = "F";

                r.Cells["DiemChu"].Value = diemChu;
            }
            catch { }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            int count = 0;
            string maLHP = cboLopHP.SelectedValue.ToString();

            foreach (DataGridViewRow r in dgvDiem.Rows)
            {
                string mssv = r.Cells["MSSV"].Value.ToString();
                float cc = r.Cells["DiemChuyenCan"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemChuyenCan"].Value);
                float gk = r.Cells["DiemGiuaKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemGiuaKy"].Value);
                float ck = r.Cells["DiemCuoiKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemCuoiKy"].Value);
                float tk = r.Cells["DiemTongKet"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemTongKet"].Value);
                string chu = r.Cells["DiemChu"].Value == DBNull.Value ? "" : r.Cells["DiemChu"].Value.ToString();

                if (busKQ.CapNhatDiem(maLHP, mssv, cc, gk, ck, tk, chu)) count++;
            }
            MessageBox.Show("Đã lưu thành công " + count + " sinh viên!", "Thông báo");
        }
    }
}