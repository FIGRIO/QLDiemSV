using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmXemDiem : Form
    {
        // Controls
        private Panel pnlHeader, pnlTable, pnlFooter;
        private Label lblHeader, lblGPA;
        private DataGridView dgvDiem;

        // --- NÚT XUẤT FILE ---
        private Button btnExcel, btnPdf;

        private string _mssv;
        private BUS_KetQua busKQ = new BUS_KetQua();

        public frmXemDiem(string mssv)
        {
            this._mssv = mssv;
            InitializeComponent_XemDiem();
            LoadData();
        }

        private void InitializeComponent_XemDiem()
        {
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // 1. TABLE (Lưới điểm)
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvDiem = new DataGridView
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
            dgvDiem.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvDiem.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDiem.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            pnlTable.Controls.Add(dgvDiem);
            this.Controls.Add(pnlTable);

            // 2. FOOTER
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.White };
            pnlFooter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 0, 0, pnlFooter.Width, 0); };
            lblGPA = new Label { Text = "Điểm trung bình tích lũy (GPA): ...", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Red, AutoSize = true, Location = new Point(20, 20) };
            pnlFooter.Controls.Add(lblGPA);
            this.Controls.Add(pnlFooter);

            // 3. HEADER (SỬA LỖI HIỂN THỊ NÚT TẠI ĐÂY)
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };

            // SỬA: Dock = Left (Thay vì Fill)
            lblHeader = new Label
            {
                Text = "  ➤  KẾT QUẢ HỌC TẬP CÁ NHÂN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 59, 124),
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblHeader);

            // THÊM NÚT EXCEL
            btnExcel = new Button
            {
                Text = "Excel",
                Location = new Point(800, 10), // Vị trí cố định
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right, // Neo góc phải
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExcel.FlatAppearance.BorderSize = 0;
            btnExcel.Click += (s, e) => XuatFile("Excel");
            pnlHeader.Controls.Add(btnExcel);

            // THÊM NÚT PDF
            btnPdf = new Button
            {
                Text = "PDF",
                Location = new Point(890, 10),
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right, // Neo góc phải
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPdf.FlatAppearance.BorderSize = 0;
            btnPdf.Click += (s, e) => XuatFile("PDF");
            pnlHeader.Controls.Add(btnPdf);

            // QUAN TRỌNG: Đưa nút lên trên cùng
            btnExcel.BringToFront();
            btnPdf.BringToFront();

            this.Controls.Add(pnlHeader);

            // Sắp xếp thứ tự hiển thị panel
            pnlHeader.SendToBack();
            pnlFooter.SendToBack();
            pnlTable.BringToFront();
        }

        private void XuatFile(string loaiFile)
        {
            DataTable dt = (DataTable)dgvDiem.DataSource;
            if (dt == null || dt.Rows.Count == 0) { MessageBox.Show("Không có dữ liệu!"); return; }

            // Tạo bảng tạm để đổi tên cột
            DataTable dtPrint = dt.Copy();

            if (dtPrint.Columns.Contains("MaMH")) dtPrint.Columns["MaMH"].ColumnName = "Mã Môn";
            if (dtPrint.Columns.Contains("TenMH")) dtPrint.Columns["TenMH"].ColumnName = "Tên Môn Học";
            if (dtPrint.Columns.Contains("SoTinChi")) dtPrint.Columns["SoTinChi"].ColumnName = "TC";
            if (dtPrint.Columns.Contains("HocKy")) dtPrint.Columns["HocKy"].ColumnName = "HK";
            if (dtPrint.Columns.Contains("NamHoc")) dtPrint.Columns["NamHoc"].ColumnName = "Năm Học";
            if (dtPrint.Columns.Contains("DiemGK")) dtPrint.Columns["DiemGK"].ColumnName = "Giữa Kỳ";
            if (dtPrint.Columns.Contains("DiemCK")) dtPrint.Columns["DiemCK"].ColumnName = "Cuối Kỳ";
            if (dtPrint.Columns.Contains("DiemTK")) dtPrint.Columns["DiemTK"].ColumnName = "Tổng Kết";
            if (dtPrint.Columns.Contains("DiemChu")) dtPrint.Columns["DiemChu"].ColumnName = "Điểm Chữ";

            string title = "KẾT QUẢ HỌC TẬP - " + _mssv;

            if (loaiFile == "Excel") ExcelHelper.XuatRaExcel(dtPrint, "KQHT", title);
            else PdfHelper.XuatRaPdf(dtPrint, title);
        }

        private void LoadData()
        {
            DataTable dt = busKQ.GetDiemBySinhVien(_mssv);
            dgvDiem.DataSource = dt;
            if (dt != null && dt.Columns.Count > 0)
            {
                dgvDiem.Columns["MaMH"].HeaderText = "Mã MH";
                dgvDiem.Columns["TenMH"].HeaderText = "Tên Môn Học";
                dgvDiem.Columns["SoTinChi"].HeaderText = "TC";
                dgvDiem.Columns["HocKy"].HeaderText = "HK";
                dgvDiem.Columns["NamHoc"].HeaderText = "Năm Học";

                dgvDiem.Columns["DiemGK"].HeaderText = "Giữa Kỳ";
                dgvDiem.Columns["DiemCK"].HeaderText = "Cuối Kỳ";

                dgvDiem.Columns["DiemTK"].HeaderText = "Tổng Kết";
                dgvDiem.Columns["DiemChu"].HeaderText = "Điểm Chữ";

                dgvDiem.Columns["DiemTK"].DefaultCellStyle.BackColor = Color.LightYellow;
                dgvDiem.Columns["DiemTK"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                TinhGPA(dt);
            }
        }

        private void TinhGPA(DataTable dt)
        {
            double tongDiem = 0;
            int tongTinChi = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["DiemTK"] != DBNull.Value)
                {
                    double diemTK = Convert.ToDouble(row["DiemTK"]);
                    int tinChi = Convert.ToInt32(row["SoTinChi"]);
                    double diemHe4 = 0;
                    if (diemTK >= 8.5) diemHe4 = 4.0;
                    else if (diemTK >= 7.0) diemHe4 = 3.0;
                    else if (diemTK >= 5.5) diemHe4 = 2.0;
                    else if (diemTK >= 4.0) diemHe4 = 1.0;

                    tongDiem += diemHe4 * tinChi;
                    tongTinChi += tinChi;
                }
            }
            if (tongTinChi > 0)
            {
                double gpa = tongDiem / tongTinChi;
                lblGPA.Text = "Điểm trung bình tích lũy (Hệ 4): " + Math.Round(gpa, 2);
            }
            else lblGPA.Text = "Chưa có dữ liệu điểm để tính GPA.";
        }
    }
}