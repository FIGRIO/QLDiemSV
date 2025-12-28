using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_LopHocPhan : DBConnect
    {
        public DataTable GetDS()
        {
            string sql = @"
        SELECT 
            l.MaLHP, l.MaMon, m.TenMH as TenMon, l.MaGV, g.HoTen, 
            l.HocKy, l.NamHoc, 
            l.TyLeQT as TyLeQuaTrinh,    -- Phải có dòng này
            l.TyLeCK as TyLeCuoiKy,      -- Phải có dòng này
            l.Thu, 
            l.TietBD as TietBatDau,      
            l.SoTiet, 
            l.Phong as PhongHoc          
        FROM LopHocPhan l
        JOIN MonHoc m ON l.MaMon = m.MaMH
        JOIN GiangVien g ON l.MaGV = g.MaGV";

            return GetDataTable(sql);
        }

        public bool Them(DTO_LopHocPhan lhp)
        {
            // SỬA: TyLeQT, TyLeCK, TietBD, Phong
            string sql = string.Format(@"
                INSERT INTO LopHocPhan (MaLHP, MaMon, MaGV, HocKy, NamHoc, TyLeQT, TyLeCK, Thu, TietBD, SoTiet, Phong) 
                VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, {9}, N'{10}')",
                lhp.MaLHP, lhp.MaMon, lhp.MaGV, lhp.HocKy, lhp.NamHoc, lhp.TyLeQT, lhp.TyLeCK,
                lhp.Thu, lhp.TietBD, lhp.SoTiet, lhp.Phong);

            return ExecuteNonQuery(sql) > 0;
        }

        public bool Sua(DTO_LopHocPhan lhp)
        {
            // SỬA tên cột cho khớp DB
            string sql = string.Format(@"
                UPDATE LopHocPhan 
                SET MaMon='{0}', MaGV='{1}', HocKy='{2}', NamHoc='{3}', 
                    TyLeQT={4}, TyLeCK={5},
                    Thu={6}, TietBD={7}, SoTiet={8}, Phong=N'{9}'
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
            // Sửa TenMon -> TenMH và Join condition
            string sql = string.Format(@"
                SELECT l.MaLHP, l.MaMon, m.TenMH, l.MaGV, g.HoTen, l.HocKy, l.NamHoc, l.TyLeQT, l.TyLeCK
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMH
                JOIN GiangVien g ON l.MaGV = g.MaGV
                WHERE l.MaLHP LIKE '%{0}%' OR m.TenMH LIKE N'%{0}%'", kw);
            return GetDataTable(sql);
        }

        public DataTable GetLopByGV(string maGV)
        {
            // Sửa TenMon -> TenMH, TyLeQT, TyLeCK
            string sql = string.Format(@"
                SELECT l.MaLHP, l.MaMon, m.TenMH, l.HocKy, l.NamHoc, 
                       l.TyLeQT, l.TyLeCK
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMH
                WHERE l.MaGV = '{0}'", maGV);

            return GetDataTable(sql);
        }

        public DataTable GetLichDay(string maGV)
        {
            // Sửa TenMH, TietBD, Phong
            string sql = string.Format(@"
                SELECT l.MaLHP, 
                       l.MaMon, 
                       m.TenMH, 
                       m.SoTinChi, 
                       l.HocKy, l.NamHoc, 
                       l.Thu, l.TietBD, l.SoTiet, l.Phong,
                       (SELECT COUNT(*) FROM KetQua k WHERE k.MaLHP = l.MaLHP) as SiSo
                FROM LopHocPhan l
                JOIN MonHoc m ON l.MaMon = m.MaMH
                WHERE l.MaGV = '{0}'
                ORDER BY l.NamHoc DESC, l.HocKy DESC, l.Thu ASC", maGV);

            return GetDataTable(sql);
        }

        public DataTable GetLichHocSinhVien(string mssv, string namHoc, string hocKy)
        {
            // Sửa TenMH, TietBD, Phong
            string sql = string.Format(@"
                SELECT l.MaLHP, m.TenMH, l.Thu, l.TietBD, l.SoTiet, l.Phong, g.HoTen as TenGV
                FROM KetQua k
                JOIN LopHocPhan l ON k.MaLHP = l.MaLHP
                JOIN MonHoc m ON l.MaMon = m.MaMH
                JOIN GiangVien g ON l.MaGV = g.MaGV
                WHERE k.MSSV = '{0}' 
                  AND l.NamHoc = '{1}' 
                  AND l.HocKy = '{2}'", mssv, namHoc, hocKy);

            return GetDataTable(sql);
        }
    }
}