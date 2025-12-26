using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmNhapDiem : Form
    {
        // Controls
        private Panel pnlHeader, pnlControl, pnlTable;
        private Label lblHeader, lblChonLop, lblTyLe;
        private ComboBox cboLopHP;
        private Button btnLuu, btnIn;
        private DataGridView dgvDiem;
        private string _maGV;

        // BUS
        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();
        BUS_KetQua busKQ = new BUS_KetQua();

        // Biến lưu tỷ lệ của lớp đang chọn
        private float _tyLeQT = 0.3f; // Mặc định 30%
        private float _tyLeCK = 0.7f; // Mặc định 70%

        public frmNhapDiem(string maGV = "")
        {
            this._maGV = maGV; // Lưu lại để dùng
            InitializeComponent_Diem();
            LoadCboLop();
        }

        private void InitializeComponent_Diem()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // --- KHỞI TẠO CÁC PANEL TRƯỚC ---

            // 1. TABLE (BẢNG ĐIỂM) - Khởi tạo
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

            // Sự kiện
            dgvDiem.CellValueChanged += DgvDiem_CellValueChanged;
            dgvDiem.EditingControlShowing += DgvDiem_EditingControlShowing;

            pnlTable.Controls.Add(dgvDiem);


            // 2. PANEL CONTROL (KHUNG CHỌN LỚP) - Khởi tạo
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


            // 3. HEADER - Khởi tạo
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  NHẬP ĐIỂM", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);


            // --- QUAN TRỌNG: THỨ TỰ ADD VÀO FORM ---
            // Add theo thứ tự: TABLE (Fill) -> CONTROL (Top) -> HEADER (Top)
            // Để Header nằm trên cùng, Control nằm giữa, Table nằm dưới cùng

            this.Controls.Add(pnlTable);   // Add trước tiên (sẽ nằm dưới cùng)
            this.Controls.Add(pnlControl); // Add thứ hai (nằm đè lên Table nhưng Dock Top sẽ đẩy Table xuống)
            this.Controls.Add(pnlHeader);  // Add cuối cùng (nằm trên cùng)
        }

        private Button CreateButton(Panel parent, string text, int x, int y, Color bg)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(120, 35), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            parent.Controls.Add(b);
            return b;
        }

        // --- LOGIC ---

        private void LoadCboLop()
        {
            // Nếu _maGV rỗng -> Là Admin -> Load hết
            if (string.IsNullOrEmpty(_maGV) || _maGV.ToLower() == "admin")
            {
                cboLopHP.DataSource = busLHP.GetDS();
            }
            else
            {
                // Nếu có Mã GV -> Chỉ load lớp của GV đó
                cboLopHP.DataSource = busLHP.GetLopByGV(_maGV);
            }

            cboLopHP.DisplayMember = "MaLHP";
            cboLopHP.ValueMember = "MaLHP";
        }

        private void CboLopHP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLopHP.SelectedValue == null) return;
            string maLHP = cboLopHP.SelectedValue.ToString();

            // --- SỬA: KHAI BÁO BIẾN row TRƯỚC ---
            DataRowView row = (DataRowView)cboLopHP.SelectedItem;

            // --- SAU ĐÓ MỚI DÙNG row ĐỂ LẤY DỮ LIỆU ---
            // Kiểm tra null để tránh lỗi crash
            if (row["TyLeQuaTrinh"] != DBNull.Value && row["TyLeCuoiKy"] != DBNull.Value)
            {
                float rawQT = Convert.ToSingle(row["TyLeQuaTrinh"]);
                float rawCK = Convert.ToSingle(row["TyLeCuoiKy"]);

                // Logic: Nếu lưu 0.5 -> giữ nguyên. Nếu lưu 50 -> chia 100.
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

                // Gán dữ liệu
                dgvDiem.DataSource = dt;

                // Nếu không có dữ liệu thì dừng, không làm gì thêm
                if (dt == null) return;

                // Cấu hình cột (Chỉ chạy khi có dữ liệu)
                if (dt.Columns.Contains("MSSV")) dgvDiem.Columns["MSSV"].ReadOnly = true;
                if (dt.Columns.Contains("HoTen")) dgvDiem.Columns["HoTen"].ReadOnly = true;
                if (dt.Columns.Contains("MaLHP")) dgvDiem.Columns["MaLHP"].Visible = false;

                // Kiểm tra cột trước khi đổi tên để tránh lỗi
                if (dt.Columns.Contains("DiemChuyenCan")) dgvDiem.Columns["DiemChuyenCan"].HeaderText = "Điểm CC";
                if (dt.Columns.Contains("DiemGiuaKy")) dgvDiem.Columns["DiemGiuaKy"].HeaderText = "Giữa Kỳ";
                if (dt.Columns.Contains("DiemCuoiKy")) dgvDiem.Columns["DiemCuoiKy"].HeaderText = "Cuối Kỳ";

                if (dt.Columns.Contains("DiemTongKet"))
                {
                    dgvDiem.Columns["DiemTongKet"].ReadOnly = true;
                    dgvDiem.Columns["DiemTongKet"].HeaderText = "Tổng Kết";
                    dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.BackColor = Color.LightYellow;
                    dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }

                if (dt.Columns.Contains("DiemChu"))
                {
                    dgvDiem.Columns["DiemChu"].ReadOnly = true;
                    dgvDiem.Columns["DiemChu"].HeaderText = "Điểm Chữ";
                }
            }
            catch (Exception ex)
            {
                // ĐÂY LÀ NƠI HIỆN LỖI CHÍNH XÁC
                MessageBox.Show("Không thể tải bảng điểm:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- XỬ LÝ TÍNH TOÁN ---

        // 1. Chỉ cho nhập số vào ô điểm
        private void DgvDiem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if (dgvDiem.CurrentCell.ColumnIndex >= 2 && dgvDiem.CurrentCell.ColumnIndex <= 4) // Các cột điểm
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }
        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho nhập số, dấu chấm, và phím xóa
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        // 2. Tự động tính điểm khi nhập xong
        private void DgvDiem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Kiểm tra nếu đang sửa cột CC, GK hoặc CK thì tính lại Tổng
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

                // Lấy giá trị (Nếu null coi như 0)
                float cc = r.Cells["DiemChuyenCan"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemChuyenCan"].Value);
                float gk = r.Cells["DiemGiuaKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemGiuaKy"].Value);
                float ck = r.Cells["DiemCuoiKy"].Value == DBNull.Value ? 0 : Convert.ToSingle(r.Cells["DiemCuoiKy"].Value);

                // CÔNG THỨC TÍNH: 
                // Quá trình = (CC + GK) / 2  (Hoặc tùy quy chế trường bạn)
                // Tổng kết = (Quá trình * Tỷ lệ QT) + (Cuối kỳ * Tỷ lệ CK)

                float diemQuaTrinh = (cc + gk) / 2;
                float diemTongKet = (diemQuaTrinh * _tyLeQT) + (ck * _tyLeCK);

                // Làm tròn 1 chữ số thập phân
                diemTongKet = (float)Math.Round(diemTongKet, 1);

                // Gán vào lưới
                r.Cells["DiemTongKet"].Value = diemTongKet;

                // Quy đổi điểm chữ (Hệ 10)
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

        // --- LƯU XUỐNG CSDL ---
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            // Duyệt từng dòng trong bảng và update
            int count = 0;
            string maLHP = cboLopHP.SelectedValue.ToString();

            foreach (DataGridViewRow r in dgvDiem.Rows)
            {
                string mssv = r.Cells["MSSV"].Value.ToString();

                // Lấy điểm (xử lý null)
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