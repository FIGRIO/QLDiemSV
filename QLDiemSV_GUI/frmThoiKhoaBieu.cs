using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmThoiKhoaBieu : Form
    {
        // UI Controls
        private Panel pnlHeader, pnlFilter, pnlContent;
        private Label lblHeader, lblNamHoc, lblHocKy;
        private ComboBox cboNamHoc, cboHocKy;
        private Button btnXem;
        private TableLayoutPanel tblLich; // Lưới hiển thị thời khóa biểu

        // Data
        private string _mssv;
        private BUS_LopHocPhan busLHP = new BUS_LopHocPhan();

        public frmThoiKhoaBieu(string mssv)
        {
            this._mssv = mssv;
            InitializeComponent_TKB();
            LoadComboboxData(); // Tự động lấy năm học có thật trong CSDL
        }

        private void InitializeComponent_TKB()
        {
            this.ClientSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // 1. FILTER PANEL (Chọn Năm học/Học kỳ)
            // =========================================================================
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            pnlFilter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.Silver, 0, 59, pnlFilter.Width, 59); };

            lblNamHoc = new Label { Text = "Năm học:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlFilter.Controls.Add(lblNamHoc);

            cboNamHoc = new ComboBox { Location = new Point(100, 17), Width = 150, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlFilter.Controls.Add(cboNamHoc);

            lblHocKy = new Label { Text = "Học kỳ:", Location = new Point(280, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlFilter.Controls.Add(lblHocKy);

            cboHocKy = new ComboBox { Location = new Point(350, 17), Width = 100, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlFilter.Controls.Add(cboHocKy);

            btnXem = new Button { Text = "Xem Lịch", Location = new Point(480, 15), Size = new Size(100, 30), BackColor = Color.FromArgb(12, 59, 124), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnXem.FlatAppearance.BorderSize = 0;
            btnXem.Click += (s, e) => VeLichHoc();
            pnlFilter.Controls.Add(btnXem);

            this.Controls.Add(pnlFilter);

            // =========================================================================
            // 2. CONTENT PANEL (Chứa TableLayoutPanel)
            // =========================================================================
            pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };

            // Khởi tạo TableLayoutPanel
            tblLich = new TableLayoutPanel();
            tblLich.Dock = DockStyle.Top; // Để Top để nó tự giãn chiều cao khi có nhiều dòng
            tblLich.AutoSize = true;
            tblLich.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblLich.BackColor = Color.White;

            // Cấu hình cột (8 cột: 1 cột Tiết + 7 cột Thứ)
            tblLich.ColumnCount = 8;
            tblLich.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F)); // Cột Tiết cố định 80px
            for (int i = 0; i < 7; i++) tblLich.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28F)); // Các cột Thứ chia đều phần còn lại

            // Cấu hình hàng (13 hàng: 1 Header + 12 Tiết)
            tblLich.RowCount = 13;
            // Chiều cao các dòng
            for (int i = 0; i < 13; i++) tblLich.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Mỗi ô cao 50px

            pnlContent.Controls.Add(tblLich);
            this.Controls.Add(pnlContent);

            // =========================================================================
            // 3. HEADER (Tiêu đề trang)
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  THỜI KHÓA BIỂU CÁ NHÂN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);

            // =========================================================================
            // QUAN TRỌNG: SẮP XẾP LỚP (Z-ORDER) ĐỂ KHÔNG BỊ ĐÈ
            // =========================================================================
            pnlHeader.SendToBack(); // Đẩy Header ra sau cùng (để chiếm chỗ trên cùng)
            pnlFilter.SendToBack(); // Đẩy Filter ra sau Header
            pnlContent.BringToFront(); // Đẩy Nội dung lên trên cùng để Fill vào phần trống còn lại
        }

        // --- HÀM 1: Lấy dữ liệu Năm học/Học kỳ thực tế từ DB ---
        private void LoadComboboxData()
        {
            // 1. Lấy toàn bộ danh sách lớp
            DataTable dt = busLHP.GetDS();

            cboNamHoc.Items.Clear();
            cboHocKy.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                // 2. Lấy Năm học duy nhất (Distinct)
                DataTable dtNam = dt.DefaultView.ToTable(true, "NamHoc");
                foreach (DataRow r in dtNam.Rows)
                {
                    cboNamHoc.Items.Add(r["NamHoc"].ToString());
                }

                // 3. Lấy Học kỳ duy nhất
                DataTable dtHK = dt.DefaultView.ToTable(true, "HocKy");
                foreach (DataRow r in dtHK.Rows)
                {
                    cboHocKy.Items.Add(r["HocKy"].ToString());
                }
            }

            // 4. Chọn giá trị mặc định nếu có
            if (cboNamHoc.Items.Count > 0) cboNamHoc.SelectedIndex = 0;
            if (cboHocKy.Items.Count > 0) cboHocKy.SelectedIndex = 0;
        }

        // --- HÀM 2: VẼ LỊCH HỌC (LOGIC CHÍNH) ---
        private void VeLichHoc()
        {
            // 1. Reset lưới về trạng thái trắng
            tblLich.Controls.Clear();

            // 2. Vẽ Header (Hàng 0: Thứ 2 -> CN)
            string[] thuArray = { "", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
            for (int i = 0; i < thuArray.Length; i++)
            {
                Label lbl = new Label { Text = thuArray[i], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(230, 230, 230) };
                tblLich.Controls.Add(lbl, i, 0);
            }

            // 3. Vẽ Cột Tiết (Cột 0: Tiết 1 -> 12)
            for (int i = 1; i <= 12; i++)
            {
                Label lbl = new Label { Text = "Tiết " + i, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray };
                tblLich.Controls.Add(lbl, 0, i);
            }

            // 4. Lấy dữ liệu từ CSDL
            string nam = cboNamHoc.Text;
            string hk = cboHocKy.Text;
            DataTable dt = busLHP.GetLichHocSinhVien(_mssv, nam, hk);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    // Bỏ qua nếu dữ liệu thiếu
                    if (r["Thu"] == DBNull.Value || r["TietBatDau"] == DBNull.Value) continue;

                    string tenMon = r["TenMon"].ToString();
                    string phong = r["PhongHoc"].ToString();
                    string gv = r["TenGV"].ToString();

                    // --- SỬA LỖI: Dùng tên biến 'thuHoc' để không trùng với 'thuArray' ---
                    int thuHoc = Convert.ToInt32(r["Thu"]); // 2,3,4...8
                    int tietBD = Convert.ToInt32(r["TietBatDau"]);
                    int soTiet = Convert.ToInt32(r["SoTiet"]);

                    // --- TÍNH TOÁN VỊ TRÍ TRONG LƯỚI ---
                    // Cột: Thứ 2 -> Index 1, ... Thứ 7 -> Index 6, CN(8) -> Index 7
                    int colIndex = (thuHoc == 8) ? 7 : (thuHoc - 1);
                    int rowIndex = tietBD;

                    // Tạo Control hiển thị (Button cho đẹp)
                    Button btnMon = new Button();
                    btnMon.Text = string.Format("{0}\n({1})\n{2}", tenMon, phong, gv);
                    btnMon.Dock = DockStyle.Fill;
                    btnMon.FlatStyle = FlatStyle.Flat;
                    btnMon.FlatAppearance.BorderSize = 0;
                    btnMon.BackColor = Color.FromArgb(190, 227, 255); // Màu xanh nhạt dễ nhìn
                    btnMon.Font = new Font("Segoe UI", 9);
                    btnMon.TextAlign = ContentAlignment.MiddleCenter;

                    // Sự kiện Click xem chi tiết
                    btnMon.Click += (s, args) => MessageBox.Show($"Môn: {tenMon}\nGiảng viên: {gv}\nPhòng: {phong}\nThời gian: Thứ {thuHoc}, Tiết {tietBD}-{tietBD + soTiet - 1}", "Thông tin lớp học");

                    // Add vào TableLayout tại vị trí (Cột, Hàng)
                    tblLich.Controls.Add(btnMon, colIndex, rowIndex);

                    // QUAN TRỌNG: Kéo giãn ô theo số tiết (RowSpan)
                    if (soTiet > 1)
                    {
                        tblLich.SetRowSpan(btnMon, soTiet);
                    }
                }
            }
            else
            {
                // Nếu không có dữ liệu thì thôi, lưới vẫn hiện khung trống
                // Có thể MessageBox báo "Không tìm thấy lịch học" nếu muốn
            }
        }
    }
}