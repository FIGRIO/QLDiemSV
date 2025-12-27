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

            // 1. TABLE
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

            // 2. FOOTER (GPA)
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.White };
            pnlFooter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 0, 0, pnlFooter.Width, 0); };

            lblGPA = new Label
            {
                Text = "Điểm trung bình tích lũy (GPA): ...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlFooter.Controls.Add(lblGPA);
            this.Controls.Add(pnlFooter);

            // 3. HEADER
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };
            lblHeader = new Label
            {
                Text = "  ➤  KẾT QUẢ HỌC TẬP CÁ NHÂN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 59, 124),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblHeader);
            this.Controls.Add(pnlHeader);

            // Sắp xếp lớp
            pnlHeader.SendToBack();
            pnlFooter.SendToBack();
            pnlTable.BringToFront();
        }

        private void LoadData()
        {
            DataTable dt = busKQ.GetDiemBySinhVien(_mssv);
            dgvDiem.DataSource = dt;

            if (dt != null && dt.Columns.Count > 0)
            {
                dgvDiem.Columns["MaMon"].HeaderText = "Mã MH";
                dgvDiem.Columns["TenMon"].HeaderText = "Tên Môn Học";
                dgvDiem.Columns["SoTinChi"].HeaderText = "TC";
                dgvDiem.Columns["HocKy"].HeaderText = "HK";
                dgvDiem.Columns["NamHoc"].HeaderText = "Năm Học";

                // --- ĐÃ XÓA CỘT DIEMCHUYENCAN ---
                dgvDiem.Columns["DiemGiuaKy"].HeaderText = "Giữa Kỳ";
                dgvDiem.Columns["DiemCuoiKy"].HeaderText = "Cuối Kỳ";

                dgvDiem.Columns["DiemTongKet"].HeaderText = "Tổng Kết";
                dgvDiem.Columns["DiemChu"].HeaderText = "Điểm Chữ";

                dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.BackColor = Color.LightYellow;
                dgvDiem.Columns["DiemTongKet"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                TinhGPA(dt);
            }
        }

        private void TinhGPA(DataTable dt)
        {
            double tongDiem = 0;
            int tongTinChi = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["DiemTongKet"] != DBNull.Value)
                {
                    double diemTK = Convert.ToDouble(row["DiemTongKet"]);
                    int tinChi = Convert.ToInt32(row["SoTinChi"]);

                    // Quy đổi Hệ 4
                    double diemHe4 = 0;
                    if (diemTK >= 8.5) diemHe4 = 4.0;
                    else if (diemTK >= 7.0) diemHe4 = 3.0;
                    else if (diemTK >= 5.5) diemHe4 = 2.0;
                    else if (diemTK >= 4.0) diemHe4 = 1.0;
                    else diemHe4 = 0;

                    tongDiem += diemHe4 * tinChi;
                    tongTinChi += tinChi;
                }
            }

            if (tongTinChi > 0)
            {
                double gpa = tongDiem / tongTinChi;
                lblGPA.Text = "Điểm trung bình tích lũy (Hệ 4): " + Math.Round(gpa, 2);
            }
            else
            {
                lblGPA.Text = "Chưa có dữ liệu điểm để tính GPA.";
            }
        }
    }
}