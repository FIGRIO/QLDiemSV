namespace QLDiemSV_DTO
{
    public class DTO_KetQua
    {
        public string MaLHP { get; set; }
        public string MSSV { get; set; }
        // Các điểm số có thể null nên dùng float? (nullable) hoặc float
        public float DiemGK { get; set; }
        public float DiemCK { get; set; }
        public float DiemTK { get; set; }
        public string DiemChu { get; set; }

        // Constructor dùng cho việc Đăng ký (Chưa có điểm)
        public DTO_KetQua(string malhp, string mssv)
        {
            this.MaLHP = malhp;
            this.MSSV = mssv;
        }
    }
}