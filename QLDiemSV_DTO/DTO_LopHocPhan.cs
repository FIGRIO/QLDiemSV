namespace QLDiemSV_DTO
{
    public class DTO_LopHocPhan
    {
        public string MaLHP { get; set; }
        public string MaMon { get; set; }
        public string MaGV { get; set; }
        public string HocKy { get; set; }
        public string NamHoc { get; set; }
        public float TyLeQT { get; set; } // Tỷ lệ quá trình (VD: 0.3 hoặc 30)
        public float TyLeCK { get; set; } // Tỷ lệ cuối kỳ (VD: 0.7 hoặc 70)

        public DTO_LopHocPhan(string ma, string mamon, string magv, string hk, string nam, float tlqt, float tlck)
        {
            this.MaLHP = ma;
            this.MaMon = mamon;
            this.MaGV = magv;
            this.HocKy = hk;
            this.NamHoc = nam;
            this.TyLeQT = tlqt;
            this.TyLeCK = tlck;
        }
    }
}