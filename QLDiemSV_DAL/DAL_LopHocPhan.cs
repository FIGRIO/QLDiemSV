using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_LopHocPhan : DBConnect
    {
        public DataTable GetDS()
        {
            // Kết bảng để lấy Tên Môn và Tên GV
            string sql = @"
                SELECT l.MaLHP, l.MaMon, m.TenMon, l.MaGV, g.HoTen, l.HocKy, l.NamHoc, l.TyLeQuaTrinh, l.TyLeCuoiKy
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMon
                JOIN GiangVien g ON l.MaGV = g.MaGV";
            return GetDataTable(sql);
        }

        public bool Them(DTO_LopHocPhan lhp)
        {
            // Chú ý thứ tự các biến {0}, {1}... phải khớp
            string sql = string.Format(@"
        INSERT INTO LopHocPhan (MaLHP, MaMon, MaGV, HocKy, NamHoc, TyLeQuaTrinh, TyLeCuoiKy, Thu, TietBatDau, SoTiet, PhongHoc) 
        VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, {9}, N'{10}')",
                lhp.MaLHP, lhp.MaMon, lhp.MaGV, lhp.HocKy, lhp.NamHoc, lhp.TyLeQT, lhp.TyLeCK,
                lhp.Thu, lhp.TietBD, lhp.SoTiet, lhp.Phong); // <--- Truyền thêm tham số mới

            return ExecuteNonQuery(sql) > 0;
        }

        public bool Sua(DTO_LopHocPhan lhp)
        {
            string sql = string.Format(@"
        UPDATE LopHocPhan 
        SET MaMon='{0}', MaGV='{1}', HocKy='{2}', NamHoc='{3}', 
            TyLeQuaTrinh={4}, TyLeCuoiKy={5},
            Thu={6}, TietBatDau={7}, SoTiet={8}, PhongHoc=N'{9}' -- <--- Update thêm cột mới
        WHERE MaLHP='{10}'",
                lhp.MaMon, lhp.MaGV, lhp.HocKy, lhp.NamHoc, lhp.TyLeQT, lhp.TyLeCK,
                lhp.Thu, lhp.TietBD, lhp.SoTiet, lhp.Phong,
                lhp.MaLHP);

            return ExecuteNonQuery(sql) > 0;
        }

        public bool Xoa(string maLHP)
        {
            string sql = string.Format("DELETE FROM LopHocPhan WHERE MaLHP = '{0}'", maLHP);
            return ExecuteNonQuery(sql) > 0;
        }

        public DataTable TimKiem(string kw)
        {
            // Tìm theo Mã LHP hoặc Tên Môn
            string sql = string.Format(@"
                SELECT l.MaLHP, l.MaMon, m.TenMon, l.MaGV, g.HoTen, l.HocKy, l.NamHoc, l.TyLeQuaTrinh, l.TyLeCuoiKy
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMon
                JOIN GiangVien g ON l.MaGV = g.MaGV
                WHERE l.MaLHP LIKE '%{0}%' OR m.TenMon LIKE N'%{0}%'", kw);
            return GetDataTable(sql);
        }

        public DataTable GetLopByGV(string maGV)
        {
            // Chỉ lấy các lớp mà MaGV trùng với người đăng nhập
            string sql = string.Format(@"
                SELECT l.MaLHP, l.MaMon, m.TenMon, l.HocKy, l.NamHoc, 
                       l.TyLeQuaTrinh, l.TyLeCuoiKy
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMon
                WHERE l.MaGV = '{0}'", maGV);

            return GetDataTable(sql);
        }

        public DataTable GetLichDay(string maGV)
        {
            // BỔ SUNG: l.MaMon và m.SoTinChi vào câu Select
            string sql = string.Format(@"
        SELECT l.MaLHP, 
               l.MaMon,      -- <--- Đã thêm lại cột này
               m.TenMon, 
               m.SoTinChi,   -- <--- Đã thêm lại cột này
               l.HocKy, l.NamHoc, 
               l.Thu, l.TietBatDau, l.SoTiet, l.PhongHoc,
               (SELECT COUNT(*) FROM KetQua k WHERE k.MaLHP = l.MaLHP) as SiSo
        FROM LopHocPhan l
        JOIN MonHoc m ON l.MaMon = m.MaMon
        WHERE l.MaGV = '{0}'
        ORDER BY l.NamHoc DESC, l.HocKy DESC, l.Thu ASC", maGV);

            return GetDataTable(sql);
        }

        public DataTable GetLichHocSinhVien(string mssv, string namHoc, string hocKy)
        {
            // Kết bảng KetQua (để lấy lớp SV học) với LopHocPhan (để lấy lịch)
            string sql = string.Format(@"
        SELECT l.MaLHP, m.TenMon, l.Thu, l.TietBatDau, l.SoTiet, l.PhongHoc, g.HoTen as TenGV
        FROM KetQua k
        JOIN LopHocPhan l ON k.MaLHP = l.MaLHP
        JOIN MonHoc m ON l.MaMon = m.MaMon
        JOIN GiangVien g ON l.MaGV = g.MaGV
        WHERE k.MSSV = '{0}' 
          AND l.NamHoc = '{1}' 
          AND l.HocKy = '{2}'", mssv, namHoc, hocKy);

            return GetDataTable(sql);
        }
    }
}