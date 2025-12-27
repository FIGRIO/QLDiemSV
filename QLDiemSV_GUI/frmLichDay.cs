using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLDiemSV_BUS;

namespace QLDiemSV_GUI
{
    public partial class frmLichDay : Form
    {
        // Controls UI
        private Panel pnlHeader, pnlTable, pnlFilter;
        private Label lblHeader, lblInfo, lblNamHoc, lblHocKy;
        private ComboBox cboNamHoc, cboHocKy;
        private DataGridView dgvLich;
        private Button btnLamMoi;

        // Data
        private string _maGV;
        private DataTable _dtGoc; // Lưu bảng dữ liệu gốc để lọc
        BUS_LopHocPhan busLHP = new BUS_LopHocPhan();

        public frmLichDay(string maGV)
        {
            this._maGV = maGV;
            InitializeComponent_Lich();
            LoadData();
        }

        private void InitializeComponent_Lich()
        {
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(242, 244, 248);
            this.FormBorderStyle = FormBorderStyle.None;

            // =========================================================================
            // 1. TABLE (ADD ĐẦU TIÊN ĐỂ NẰM DƯỚI CÙNG)
            // =========================================================================
            pnlTable = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            lblInfo = new Label
            {
                Text = "Danh sách lớp phụ trách:",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.DimGray
            };
            pnlTable.Controls.Add(lblInfo);

            dgvLich = new DataGridView
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
                ColumnHeadersHeight = 45
            };

            // Style Header
            dgvLich.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(12, 59, 124);
            dgvLich.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLich.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            pnlTable.Controls.Add(dgvLich);
            pnlTable.Controls.SetChildIndex(dgvLich, 0);

            this.Controls.Add(pnlTable);

            // =========================================================================
            // 2. FILTER (BỘ LỌC) - NẰM GIỮA
            // =========================================================================
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            pnlFilter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 0, 59, pnlFilter.Width, 59); };

            // -- Combo Năm học --
            lblNamHoc = new Label { Text = "Năm học:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlFilter.Controls.Add(lblNamHoc);

            cboNamHoc = new ComboBox { Location = new Point(100, 17), Width = 150, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboNamHoc.SelectedIndexChanged += FilterData; // Sự kiện lọc
            pnlFilter.Controls.Add(cboNamHoc);

            // -- Combo Học kỳ --
            lblHocKy = new Label { Text = "Học kỳ:", Location = new Point(280, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlFilter.Controls.Add(lblHocKy);

            cboHocKy = new ComboBox { Location = new Point(350, 17), Width = 100, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboHocKy.SelectedIndexChanged += FilterData; // Sự kiện lọc
            pnlFilter.Controls.Add(cboHocKy);

            // -- Nút Làm mới --
            btnLamMoi = new Button { Text = "🔄 Tải lại", Location = new Point(500, 15), Size = new Size(100, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.WhiteSmoke, ForeColor = Color.Black, Cursor = Cursors.Hand };
            btnLamMoi.FlatAppearance.BorderColor = Color.Silver;
            btnLamMoi.Click += (s, e) => LoadData();
            pnlFilter.Controls.Add(btnLamMoi);

            this.Controls.Add(pnlFilter);

            // =========================================================================
            // 3. HEADER (ADD CUỐI CÙNG ĐỂ NẰM TRÊN)
            // =========================================================================
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(242, 244, 248) };
            pnlHeader.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(Color.FromArgb(12, 59, 124), 2), 15, 40, 250, 40); };

            lblHeader = new Label
            {
                Text = "  ➤  LỊCH GIẢNG DẠY",
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

        private void LoadData()
        {
            try
            {
                // 1. Lấy dữ liệu gốc từ DB
                _dtGoc = busLHP.GetLichDay(_maGV);

                // 2. Gán vào Grid
                dgvLich.DataSource = _dtGoc;

                // 3. Cấu hình cột hiển thị (Nếu có dữ liệu)
                if (_dtGoc != null && _dtGoc.Columns.Count > 0)
                {
                    // Đặt tên cột tiếng Việt
                    SetColumnHeader("MaLHP", "Mã Lớp HP");
                    SetColumnHeader("MaMon", "Mã Môn");
                    SetColumnHeader("TenMon", "Tên Môn Học", 200);
                    SetColumnHeader("SoTinChi", "TC", 50);
                    SetColumnHeader("HocKy", "Học Kỳ", 70);
                    SetColumnHeader("NamHoc", "Năm Học");
                    SetColumnHeader("Thu", "Thứ", 50);
                    SetColumnHeader("TietBatDau", "Tiết BĐ", 70);
                    SetColumnHeader("SoTiet", "Số Tiết", 70);
                    SetColumnHeader("PhongHoc", "Phòng");
                    SetColumnHeader("SiSo", "Sĩ Số", 60);

                    // 4. NẠP DỮ LIỆU CHO COMBOBOX LỌC (Distinct)
                    LoadComboBoxFilter();
                }

                UpdateLabelCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch dạy: " + ex.Message);
            }
        }

        // Hàm hỗ trợ đặt tên cột an toàn
        private void SetColumnHeader(string colName, string headerText, int width = 0)
        {
            if (dgvLich.Columns.Contains(colName))
            {
                dgvLich.Columns[colName].HeaderText = headerText;
                if (width > 0) dgvLich.Columns[colName].Width = width;
                if (colName == "Thu" || colName == "SiSo" || colName == "SoTinChi")
                    dgvLich.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void LoadComboBoxFilter()
        {
            // Tạm ngắt sự kiện để không bị kích hoạt lọc khi đang add item
            cboNamHoc.SelectedIndexChanged -= FilterData;
            cboHocKy.SelectedIndexChanged -= FilterData;

            cboNamHoc.Items.Clear();
            cboHocKy.Items.Clear();

            // Thêm mục mặc định
            cboNamHoc.Items.Add("Tất cả");
            cboHocKy.Items.Add("Tất cả");

            if (_dtGoc != null)
            {
                // Lấy danh sách Năm học duy nhất
                DataTable dtNam = _dtGoc.DefaultView.ToTable(true, "NamHoc");
                foreach (DataRow r in dtNam.Rows) cboNamHoc.Items.Add(r["NamHoc"].ToString());

                // Lấy danh sách Học kỳ duy nhất
                DataTable dtHK = _dtGoc.DefaultView.ToTable(true, "HocKy");
                foreach (DataRow r in dtHK.Rows) cboHocKy.Items.Add(r["HocKy"].ToString());
            }

            cboNamHoc.SelectedIndex = 0; // Chọn "Tất cả"
            cboHocKy.SelectedIndex = 0;

            // Bật lại sự kiện
            cboNamHoc.SelectedIndexChanged += FilterData;
            cboHocKy.SelectedIndexChanged += FilterData;
        }

        private void FilterData(object sender, EventArgs e)
        {
            if (_dtGoc == null) return;

            string filter = "";

            // Lọc Năm học
            if (cboNamHoc.SelectedIndex > 0) // Index 0 là "Tất cả"
            {
                filter += string.Format("NamHoc = '{0}'", cboNamHoc.SelectedItem);
            }

            // Lọc Học kỳ
            if (cboHocKy.SelectedIndex > 0)
            {
                if (filter.Length > 0) filter += " AND "; // Thêm AND nếu đã có điều kiện trước
                filter += string.Format("HocKy = '{0}'", cboHocKy.SelectedItem);
            }

            // Áp dụng bộ lọc
            _dtGoc.DefaultView.RowFilter = filter;
            UpdateLabelCount();
        }

        private void UpdateLabelCount()
        {
            int count = dgvLich.Rows.GetRowCount(DataGridViewElementStates.Visible);
            lblInfo.Text = "Tổng số lớp hiển thị: " + count;
        }
    }
}