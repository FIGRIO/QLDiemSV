using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace QLDiemSV_DTO
{
    // File: QLDiemSV_DTO/DTO_LopHocPhan.cs

    public class DTO_LopHocPhan
    {
        // 1. Các thuộc tính (Properties)
        public string MaLHP { get; set; }
        public string MaMon { get; set; }
        public string MaGV { get; set; }
        public string HocKy { get; set; }
        public string NamHoc { get; set; }
        public float TyLeQT { get; set; }
        public float TyLeCK { get; set; }

        // --- THUỘC TÍNH MỚI (LỊCH HỌC) ---
        public int Thu { get; set; }        // 2, 3, 4, 5, 6, 7, 8 (CN)
        public int TietBD { get; set; }     // Tiết bắt đầu (1-15)
        public int SoTiet { get; set; }     // Số tiết học
        public string Phong { get; set; }   // Phòng học (A1-101)

        // 2. Hàm khởi tạo (Constructor) - ĐÃ BỔ SUNG THAM SỐ
        public DTO_LopHocPhan(string ma, string mamon, string magv, string hk, string nam,
                              float tlqt, float tlck,
                              int thu, int tietbd, int sotiet, string phong) // <--- Đã thêm vào đây
        {
            this.MaLHP = ma;
            this.MaMon = mamon;
            this.MaGV = magv;
            this.HocKy = hk;
            this.NamHoc = nam;
            this.TyLeQT = tlqt;
            this.TyLeCK = tlck;

            // Gán giá trị mới
            this.Thu = thu;
            this.TietBD = tietbd;
            this.SoTiet = sotiet;
            this.Phong = phong;
        }
    }
}