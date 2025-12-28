using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmNhapDiem : Form
    {
        private Panel pnlHeader, pnlFilter, pnlControl, pnlTable;
        private Label lblHeader, lblChonLop, lblTyLe, lblNamHoc, lblHocKy;
        private ComboBox cboLopHP, cboNamHoc, cboHocKy;

        // --- 3 NÚT CHỨC NĂNG ---
        private Button btnLuu, btnExcel, btnPdf;
        private DataGridView dgvDiem;

        private string _maGV;
        private DataTable _dtLopGoc;
        private float _tyLeQT = 0.3f;
        private float _tyLeCK = 0.7f;

        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_KetQua busKQ = new BUS_KetQua();

        public frmNhapDiem(string maGV = "")
        {
            this._maGV = maGV;
            InitializeComponent_Diem();
            LoadDataLop();
        }

        private void InitializeComponent_Diem()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE
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
            dgvDiem.CellValidating += DgvDiem_CellValidating;
            dgvDiem.CellEndEdit += DgvDiem_CellEndEdit;
            pnlTable.Controls.Add(dgvDiem);
            this.Controls.Add(pnlTable);

            // 2. CONTROL (CHỨA 3 NÚT)
            pnlControl = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(20) };
            pnlControl.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlControl.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid); };

            lblChonLop = new Label { Text = "Chọn Lớp HP:", Location = new Point(30, 28), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124) };
            pnlControl.Controls.Add(lblChonLop);

            cboLopHP = new ComboBox { Location = new Point(150, 25), Size = new Size(250, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cboLopHP.SelectedIndexChanged += CboLopHP_SelectedIndexChanged;
            pnlControl.Controls.Add(cboLopHP);

            lblTyLe = new Label { Text = "(Tỷ lệ: -- / --)", Location = new Point(410, 28), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.DimGray };
            pnlControl.Controls.Add(lblTyLe);

            // --- NÚT 1: LƯU DB ---
            btnLuu = CreateButton(pnlControl, "💾 LƯU ĐIỂM", 900, 20, Color.FromArgb(0, 123, 255));
            btnLuu.Width = 150;
            btnLuu.Click += BtnLuu_Click;

            // --- NÚT 2: EXCEL ---
            btnExcel = CreateButton(pnlControl, "📗 EXCEL", 780, 20, Color.FromArgb(40, 167, 69));
            btnExcel.Width = 110;
            btnExcel.Click += (s, e) => XuatFile("Excel");

            // --- NÚT 3: PDF ---
            btnPdf = CreateButton(pnlControl, "📕 PDF", 660, 20, Color.FromArgb(220, 53, 69));
            btnPdf.Width = 110;
            btnPdf.Click += (s, e) => XuatFile("PDF");

            this.Controls.Add(pnlControl);

            // 3. FILTER
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
            pnlFilter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.Silver, 0, 59, pnlFilter.Width, 59); };
            lblNamHoc = new Label { Text = "Năm học:", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlFilter.Controls.Add(lblNamHoc);
            cboNamHoc = new ComboBox { Location = new Point(110, 17), Width = 150, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboNamHoc.SelectedIndexChanged += FilterDSLop;
            pnlFilter.Controls.Add(cboNamHoc);
            lblHocKy = new Label { Text = "Học kỳ:", Location = new Point(290, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlFilter.Controls.Add(lblHocKy);
            cboHocKy = new ComboBox { Location = new Point(350, 17), Width = 100, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboHocKy.SelectedIndexChanged += FilterDSLop;
            pnlFilter.Controls.Add(cboHocKy);
            this.Controls.Add(pnlFilter);

            // 4. HEADER
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

        // --- XUẤT FILE CHUNG ---
        private void XuatFile(string loaiFile)
        {
            if (dgvDiem.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu!", "Cảnh báo");
                return;
            }

            DataTable dtExport = new DataTable();
            foreach (DataGridViewColumn col in dgvDiem.Columns)
            {
                if (col.Visible) dtExport.Columns.Add(col.HeaderText);
            }

            foreach (DataGridViewRow row in dgvDiem.Rows)
            {
                DataRow dRow = dtExport.NewRow();
                foreach (DataGridViewColumn col in dgvDiem.Columns)
                {
                    if (col.Visible) dRow[col.HeaderText] = row.Cells[col.Name].Value;
                }
                dtExport.Rows.Add(dRow);
            }

            string title = "BẢNG ĐIỂM LỚP " + cboLopHP.Text;

            if (loaiFile == "Excel") ExcelHelper.XuatRaExcel(dtExport, "BangDiem", title);
            else PdfHelper.XuatRaPdf(dtExport, title);
        }

        // --- DATA & LOGIC ---
        private void LoadDataLop()
        {
            if (string.IsNullOrEmpty(_maGV) || _maGV.ToLower() == "admin") _dtLopGoc = busLHP.GetDS();
            else _dtLopGoc = busLHP.GetLopByGV(_maGV);
            LoadFilterCombos();
            FilterDSLop(null, null);
        }

        private void LoadFilterCombos()
        {
            cboNamHoc.SelectedIndexChanged -= FilterDSLop; cboHocKy.SelectedIndexChanged -= FilterDSLop;
            cboNamHoc.Items.Clear(); cboHocKy.Items.Clear();
            cboNamHoc.Items.Add("Tất cả"); cboHocKy.Items.Add("Tất cả");

            if (_dtLopGoc != null)
            {
                DataTable dtNam = _dtLopGoc.DefaultView.ToTable(true, "NamHoc");
                foreach (DataRow r in dtNam.Rows) cboNamHoc.Items.Add(r["NamHoc"].ToString());
                DataTable dtHK = _dtLopGoc.DefaultView.ToTable(true, "HocKy");
                foreach (DataRow r in dtHK.Rows) cboHocKy.Items.Add(r["HocKy"].ToString());
            }
            cboNamHoc.SelectedIndex = 0; cboHocKy.SelectedIndex = 0;
            cboNamHoc.SelectedIndexChanged += FilterDSLop; cboHocKy.SelectedIndexChanged += FilterDSLop;
        }

        private void FilterDSLop(object sender, EventArgs e)
        {
            if (_dtLopGoc == null) return;

            // --- BƯỚC 1: LƯU GIÁ TRỊ CŨ ---
            string oldSelectedValue = "";
            if (cboLopHP.SelectedValue != null)
            {
                oldSelectedValue = cboLopHP.SelectedValue.ToString();
            }

            string filter = "1=1";
            if (cboNamHoc.SelectedIndex > 0) filter += string.Format(" AND NamHoc = '{0}'", cboNamHoc.SelectedItem);
            if (cboHocKy.SelectedIndex > 0) filter += string.Format(" AND HocKy = '{0}'", cboHocKy.SelectedItem);

            DataView dv = new DataView(_dtLopGoc);
            dv.RowFilter = filter;

            // Ngắt sự kiện để tránh trigger lung tung khi gán DataSource
            cboLopHP.SelectedIndexChanged -= CboLopHP_SelectedIndexChanged;

            cboLopHP.DataSource = dv;
            cboLopHP.DisplayMember = "MaLHP";
            cboLopHP.ValueMember = "MaLHP";

            // --- BƯỚC 2: KHÔI PHỤC GIÁ TRỊ CŨ (NẾU CÓ) ---
            // Kiểm tra xem giá trị cũ có còn tồn tại trong danh sách mới không
            bool exists = false;
            foreach (DataRowView item in dv)
            {
                if (item["MaLHP"].ToString() == oldSelectedValue)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                cboLopHP.SelectedValue = oldSelectedValue;
            }
            else if (cboLopHP.Items.Count > 0)
            {
                cboLopHP.SelectedIndex = 0; // Nếu không còn thì chọn cái đầu
            }

            // Bật lại sự kiện
            cboLopHP.SelectedIndexChanged += CboLopHP_SelectedIndexChanged;

            // Gọi thủ công 1 lần để load điểm
            CboLopHP_SelectedIndexChanged(null, null);

            if (dv.Count == 0) { dgvDiem.DataSource = null; lblTyLe.Text = "(Không có lớp nào)"; }
        }

        private void CboLopHP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLopHP.SelectedIndex == -1 || cboLopHP.SelectedValue == null) return;

            DataRowView drv = (DataRowView)cboLopHP.SelectedItem;

            // --- SỬA ĐOẠN NÀY ĐỂ CẬP NHẬT BIẾN TOÀN CỤC ---
            if (drv.DataView.Table.Columns.Contains("TyLeQuaTrinh"))
            {
                // Cập nhật biến dùng để tính toán (quan trọng)
                _tyLeQT = Convert.ToSingle(drv["TyLeQuaTrinh"]);
                _tyLeCK = Convert.ToSingle(drv["TyLeCuoiKy"]);
            }
            else if (drv.DataView.Table.Columns.Contains("TyLeQT"))
            {
                _tyLeQT = Convert.ToSingle(drv["TyLeQT"]);
                _tyLeCK = Convert.ToSingle(drv["TyLeCK"]);
            }

            // Hiển thị lên Label
            lblTyLe.Text = string.Format("(Tỷ lệ QT/CK: {0}% - {1}%)", _tyLeQT * 100, _tyLeCK * 100);

            LoadBangDiem(cboLopHP.SelectedValue.ToString());
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

                    if (dgvDiem.Columns.Contains("DiemGK")) dgvDiem.Columns["DiemGK"].HeaderText = "Giữa Kỳ";
                    if (dgvDiem.Columns.Contains("DiemCK")) dgvDiem.Columns["DiemCK"].HeaderText = "Cuối Kỳ";

                    if (dgvDiem.Columns.Contains("DiemTK"))
                    {
                        dgvDiem.Columns["DiemTK"].ReadOnly = true;
                        dgvDiem.Columns["DiemTK"].HeaderText = "Tổng Kết";
                        dgvDiem.Columns["DiemTK"].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvDiem.Columns["DiemTK"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                        // --- THÊM DÒNG NÀY: Định dạng hiển thị 1 số lẻ (ví dụ: 9.1) ---
                        dgvDiem.Columns["DiemTK"].DefaultCellStyle.Format = "N1";
                    }

                    if (dgvDiem.Columns.Contains("DiemChu")) { dgvDiem.Columns["DiemChu"].ReadOnly = true; dgvDiem.Columns["DiemChu"].HeaderText = "Điểm Chữ"; }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải điểm: " + ex.Message); }
        }

        private void DgvDiem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            string colName = dgvDiem.Columns[dgvDiem.CurrentCell.ColumnIndex].Name;
            if (colName == "DiemGK" || colName == "DiemCK")
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null) tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
            }
        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true; }

        private void DgvDiem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string colName = dgvDiem.Columns[e.ColumnIndex].Name;
            if (colName == "DiemGK" || colName == "DiemCK") TinhDiemTongKet(e.RowIndex);
        }

        private void TinhDiemTongKet(int rowIndex)
        {
            try
            {
                DataGridViewRow r = dgvDiem.Rows[rowIndex];

                // 1. Chuyển đổi sang decimal để tính chính xác
                decimal gk = r.Cells["DiemGK"].Value == DBNull.Value ? 0 : Convert.ToDecimal(r.Cells["DiemGK"].Value);
                decimal ck = r.Cells["DiemCK"].Value == DBNull.Value ? 0 : Convert.ToDecimal(r.Cells["DiemCK"].Value);

                // Ép kiểu tỷ lệ từ float (biến toàn cục) sang decimal
                decimal tlQT = (decimal)_tyLeQT;
                decimal tlCK = (decimal)_tyLeCK;

                // 2. Tính toán
                decimal diemTongKet = (gk * tlQT) + (ck * tlCK);

                // 3. Làm tròn chuẩn (0.5 làm tròn lên)
                diemTongKet = Math.Round(diemTongKet, 1, MidpointRounding.AwayFromZero);

                // Gán giá trị (Dạng số, DataGridView sẽ tự format hiển thị nhờ code ở hàm LoadBangDiem)
                r.Cells["DiemTK"].Value = diemTongKet;

                // 4. Xếp loại (Thêm 'm' sau số để so sánh decimal)
                string diemChu = "";
                if (diemTongKet >= 9.0m) diemChu = "A+";
                else if (diemTongKet >= 8.5m) diemChu = "A";
                else if (diemTongKet >= 8.0m) diemChu = "B+";
                else if (diemTongKet >= 7.0m) diemChu = "B";
                else if (diemTongKet >= 6.5m) diemChu = "C+";
                else if (diemTongKet >= 5.5m) diemChu = "C";
                else if (diemTongKet >= 5.0m) diemChu = "D+";
                else if (diemTongKet >= 4.0m) diemChu = "D";
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
                float gk = r.Cells["DiemGK"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemGK"].Value);
                float ck = r.Cells["DiemCK"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemCK"].Value);
                float tk = r.Cells["DiemTK"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemTK"].Value);
                string chu = r.Cells["DiemChu"].Value == DBNull.Value ? "" : r.Cells["DiemChu"].Value.ToString();
                if (busKQ.CapNhatDiem(maLHP, mssv, gk, ck, tk, chu)) count++;
            }
            MessageBox.Show("Đã lưu thành công " + count + " sinh viên!", "Thông báo");
        }

        private void DgvDiem_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string colName = dgvDiem.Columns[e.ColumnIndex].Name;
            if (colName == "DiemGK" || colName == "DiemCK")
            {
                string value = e.FormattedValue.ToString().Trim();
                if (string.IsNullOrEmpty(value)) return;

                float num;
                if (!float.TryParse(value, out num))
                {
                    e.Cancel = true;
                    dgvDiem.Rows[e.RowIndex].ErrorText = "Phải nhập số!";
                    MessageBox.Show("Dữ liệu nhập phải là số!", "Lỗi");
                }
                else if (num < 0 || num > 10)
                {
                    e.Cancel = true;
                    dgvDiem.Rows[e.RowIndex].ErrorText = "Điểm 0-10 thôi!";
                    MessageBox.Show("Điểm chỉ được từ 0 đến 10!", "Lỗi");
                }
            }
        }

        private void DgvDiem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvDiem.Rows[e.RowIndex].ErrorText = string.Empty;
        }

    }
}