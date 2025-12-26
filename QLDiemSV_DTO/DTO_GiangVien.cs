namespace QLDiemSV_DTO
{
    public class DTO_GiangVien
    {
        public string MaGV { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string MaKhoa { get; set; }

        public DTO_GiangVien(string ma, string ten, string email, string sdt, string makhoa)
        {
            this.MaGV = ma;
            this.HoTen = ten;
            this.Email = email;
            this.SDT = sdt;
            this.MaKhoa = makhoa;
        }
    }
}