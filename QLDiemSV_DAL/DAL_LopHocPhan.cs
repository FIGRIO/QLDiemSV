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
            string sql = string.Format("INSERT INTO LopHocPhan (MaLHP, MaMon, MaGV, HocKy, NamHoc, TyLeQuaTrinh, TyLeCuoiKy) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6})",
                                       lhp.MaLHP, lhp.MaMon, lhp.MaGV, lhp.HocKy, lhp.NamHoc, lhp.TyLeQT, lhp.TyLeCK);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Sua(DTO_LopHocPhan lhp)
        {
            string sql = string.Format("UPDATE LopHocPhan SET MaMon = '{0}', MaGV = '{1}', HocKy = '{2}', NamHoc = '{3}', TyLeQuaTrinh = {4}, TyLeCuoiKy = {5} WHERE MaLHP = '{6}'",
                                       lhp.MaMon, lhp.MaGV, lhp.HocKy, lhp.NamHoc, lhp.TyLeQT, lhp.TyLeCK, lhp.MaLHP);
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
    }
}