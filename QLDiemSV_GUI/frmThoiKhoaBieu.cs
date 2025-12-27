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

        // --- 2 NÚT XUẤT FILE MỚI ---
        private Button btnExcel, btnPdf;

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
            // 1. FILTER PANEL (Thêm nút Excel/PDF vào đây)
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

            // Nút Xem
            btnXem = new Button { Text = "Xem Lịch", Location = new Point(480, 15), Size = new Size(100, 30), BackColor = Color.FromArgb(12, 59, 124), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnXem.FlatAppearance.BorderSize = 0;
            btnXem.Click += (s, e) => VeLichHoc();
            pnlFilter.Controls.Add(btnXem);

            // --- NÚT EXCEL ---
            btnExcel = new Button { Text = "Excel", Location = new Point(600, 15), Size = new Size(80, 30), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnExcel.FlatAppearance.BorderSize = 0;
            btnExcel.Click += (s, e) => XuatFile("Excel");
            pnlFilter.Controls.Add(btnExcel);

            // --- NÚT PDF ---
            btnPdf = new Button { Text = "PDF", Location = new Point(690, 15), Size = new Size(80, 30), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnPdf.FlatAppearance.BorderSize = 0;
            btnPdf.Click += (s, e) => XuatFile("PDF");
            pnlFilter.Controls.Add(btnPdf);

            this.Controls.Add(pnlFilter);

            // =========================================================================
            // 2. CONTENT PANEL
            // =========================================================================
            pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };

            tblLich = new TableLayoutPanel();
            tblLich.Dock = DockStyle.Top;
            tblLich.AutoSize = true;
            tblLich.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblLich.BackColor = Color.White;

            tblLich.ColumnCount = 8;
            tblLich.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            for (int i = 0; i < 7; i++) tblLich.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28F));

            tblLich.RowCount = 13;
            for (int i = 0; i < 13; i++) tblLich.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

            pnlContent.Controls.Add(tblLich);
            this.Controls.Add(pnlContent);

            // =========================================================================
            // 3. HEADER
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label { Text = "  ➤  THỜI KHÓA BIỂU CÁ NHÂN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(12, 59, 124), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);

            pnlHeader.SendToBack();
            pnlFilter.SendToBack();
            pnlContent.BringToFront();
        }

        private void LoadComboboxData()
        {
            DataTable dt = busLHP.GetDS();
            cboNamHoc.Items.Clear(); cboHocKy.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataTable dtNam = dt.DefaultView.ToTable(true, "NamHoc");
                foreach (DataRow r in dtNam.Rows) cboNamHoc.Items.Add(r["NamHoc"].ToString());

                DataTable dtHK = dt.DefaultView.ToTable(true, "HocKy");
                foreach (DataRow r in dtHK.Rows) cboHocKy.Items.Add(r["HocKy"].ToString());
            }

            if (cboNamHoc.Items.Count > 0) cboNamHoc.SelectedIndex = 0;
            if (cboHocKy.Items.Count > 0) cboHocKy.SelectedIndex = 0;
        }

        private void VeLichHoc()
        {
            tblLich.Controls.Clear();

            string[] thuArray = { "", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
            for (int i = 0; i < thuArray.Length; i++)
            {
                Label lbl = new Label { Text = thuArray[i], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(230, 230, 230) };
                tblLich.Controls.Add(lbl, i, 0);
            }

            for (int i = 1; i <= 12; i++)
            {
                Label lbl = new Label { Text = "Tiết " + i, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray };
                tblLich.Controls.Add(lbl, 0, i);
            }

            string nam = cboNamHoc.Text;
            string hk = cboHocKy.Text;
            DataTable dt = busLHP.GetLichHocSinhVien(_mssv, nam, hk);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r["Thu"] == DBNull.Value || r["TietBatDau"] == DBNull.Value) continue;

                    string tenMon = r["TenMon"].ToString();
                    string phong = r["PhongHoc"].ToString();
                    string gv = r["TenGV"].ToString();

                    int thuHoc = Convert.ToInt32(r["Thu"]);
                    int tietBD = Convert.ToInt32(r["TietBatDau"]);
                    int soTiet = Convert.ToInt32(r["SoTiet"]);

                    int colIndex = (thuHoc == 8) ? 7 : (thuHoc - 1);
                    int rowIndex = tietBD;

                    Button btnMon = new Button();
                    btnMon.Text = string.Format("{0}\n({1})\n{2}", tenMon, phong, gv);
                    btnMon.Dock = DockStyle.Fill;
                    btnMon.FlatStyle = FlatStyle.Flat;
                    btnMon.FlatAppearance.BorderSize = 0;
                    btnMon.BackColor = Color.FromArgb(190, 227, 255);
                    btnMon.Font = new Font("Segoe UI", 9);
                    btnMon.TextAlign = ContentAlignment.MiddleCenter;

                    btnMon.Click += (s, args) => MessageBox.Show($"Môn: {tenMon}\nGiảng viên: {gv}\nPhòng: {phong}\nThời gian: Thứ {thuHoc}, Tiết {tietBD}-{tietBD + soTiet - 1}", "Thông tin lớp học");

                    tblLich.Controls.Add(btnMon, colIndex, rowIndex);
                    if (soTiet > 1) tblLich.SetRowSpan(btnMon, soTiet);
                }
            }
        }

        // --- HÀM XUẤT FILE ĐẶC BIỆT CHO THỜI KHÓA BIỂU ---
        // Chuyển đổi dữ liệu danh sách thành bảng 2 chiều (Thứ x Tiết)
        private void XuatFile(string type)
        {
            // 1. Tạo DataTable cấu trúc bảng TKB 
            DataTable dtExport = new DataTable();
            dtExport.Columns.Add("Tiết");
            dtExport.Columns.Add("Thứ 2");
            dtExport.Columns.Add("Thứ 3");
            dtExport.Columns.Add("Thứ 4");
            dtExport.Columns.Add("Thứ 5");
            dtExport.Columns.Add("Thứ 6");
            dtExport.Columns.Add("Thứ 7");
            dtExport.Columns.Add("CN");

            // Tạo 12 dòng trống cho 12 tiết
            for (int i = 1; i <= 12; i++)
            {
                DataRow r = dtExport.NewRow();
                r["Tiết"] = "Tiết " + i;
                dtExport.Rows.Add(r);
            }

            // 2. Điền dữ liệu môn học vào bảng
            string nam = cboNamHoc.Text;
            string hk = cboHocKy.Text;
            DataTable dtRaw = busLHP.GetLichHocSinhVien(_mssv, nam, hk);

            if (dtRaw != null)
            {
                foreach (DataRow r in dtRaw.Rows)
                {
                    if (r["Thu"] == DBNull.Value || r["TietBatDau"] == DBNull.Value) continue;

                    int thu = Convert.ToInt32(r["Thu"]); // 2..8
                    int tietBD = Convert.ToInt32(r["TietBatDau"]);
                    int soTiet = Convert.ToInt32(r["SoTiet"]);

                    string noiDung = string.Format("{0}\n({1})", r["TenMon"], r["PhongHoc"]);

                    // Xác định tên cột (Thứ 2 -> cột 1, CN -> cột 7)
                    string colName = (thu == 8) ? "CN" : "Thứ " + thu;

                    // Điền vào các ô tương ứng (từ tiết bắt đầu -> hết số tiết)
                    // LƯU Ý: Đây là điền text vào Excel/PDF, không phải vẽ Button
                    for (int k = 0; k < soTiet; k++)
                    {
                        int rowIndex = tietBD - 1 + k; // Index dòng bắt đầu từ 0
                        if (rowIndex < 12)
                        {
                            dtExport.Rows[rowIndex][colName] = noiDung;
                        }
                    }
                }
            }

            // 3. Xuất file
            string title = "THỜI KHÓA BIỂU - " + cboHocKy.Text + " - " + cboNamHoc.Text;
            if (type == "Excel") ExcelHelper.XuatRaExcel(dtExport, "TKB", title);
            else PdfHelper.XuatRaPdf(dtExport, title);
        }
    }
}