using System;

namespace QLDiemSV_DTO
{
    public class DTO_SinhVien
    {
        public string MSSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string MaLop { get; set; }

        public DTO_SinhVien(string mssv, string hoten, DateTime ngaysinh, string gioitinh, string email, string sdt, string diachi, string malop)
        {
            this.MSSV = mssv;
            this.HoTen = hoten;
            this.NgaySinh = ngaysinh;
            this.GioiTinh = gioitinh;
            this.Email = email;
            this.SDT = sdt;
            this.DiaChi = diachi;
            this.MaLop = malop;
        }
    }
}