using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_GiangVien : DBConnect
    {
        public DataTable GetDS()
        {
            return GetDataTable("SELECT * FROM GiangVien");
        }

        public bool Them(DTO_GiangVien gv)
        {
            // Bước 1: Tạo tài khoản trước (Pass mặc định 123)
            string sqlTK = string.Format("INSERT INTO TaiKhoan VALUES ('{0}', '123', 'GiangVien')", gv.MaGV);

            // Bước 2: Thêm giảng viên (Đúng tên cột trong SQL của bạn)
            string sqlGV = string.Format("INSERT INTO GiangVien (MaGV, HoTen, Email, SDT, MaKhoa) VALUES ('{0}', N'{1}', '{2}', '{3}', '{4}')",
                                         gv.MaGV, gv.HoTen, gv.Email, gv.SDT, gv.MaKhoa);

            if (ExecuteNonQuery(sqlTK) > 0) return ExecuteNonQuery(sqlGV) > 0;
            return false;
        }

        public bool Sua(DTO_GiangVien gv)
        {
            string sql = string.Format("UPDATE GiangVien SET HoTen = N'{0}', Email = '{1}', SDT = '{2}', MaKhoa = '{3}' WHERE MaGV = '{4}'",
                                       gv.HoTen, gv.Email, gv.SDT, gv.MaKhoa, gv.MaGV);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Xoa(string maGV)
        {
            string sqlGV = string.Format("DELETE FROM GiangVien WHERE MaGV = '{0}'", maGV);
            string sqlTK = string.Format("DELETE FROM TaiKhoan WHERE TenDangNhap = '{0}'", maGV);

            if (ExecuteNonQuery(sqlGV) > 0) return ExecuteNonQuery(sqlTK) > 0;
            return false;
        }

        public DataTable TimKiem(string kw)
        {
            string sql = string.Format("SELECT * FROM GiangVien WHERE MaGV LIKE '%{0}%' OR HoTen LIKE N'%{0}%'", kw);
            return GetDataTable(sql);
        }
    }
}