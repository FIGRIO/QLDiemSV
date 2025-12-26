using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_MonHoc : DBConnect
    {
        public DataTable GetDS()
        {
            return GetDataTable("SELECT * FROM MonHoc");
        }

        public bool Them(DTO_MonHoc mh)
        {
            // Sửa tên cột nếu cần, hoặc dùng VALUES như này thì thứ tự phải đúng: MaMon, TenMon, SoTinChi
            string sql = string.Format("INSERT INTO MonHoc VALUES ('{0}', N'{1}', {2})", mh.MaMH, mh.TenMH, mh.SoTinChi);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Sua(DTO_MonHoc mh)
        {
            // SỬA Ở ĐÂY: TenMH -> TenMon, MaMH -> MaMon
            string sql = string.Format("UPDATE MonHoc SET TenMon = N'{0}', SoTinChi = {1} WHERE MaMon = '{2}'", mh.TenMH, mh.SoTinChi, mh.MaMH);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Xoa(string maMH)
        {
            // SỬA Ở ĐÂY: MaMH -> MaMon
            string sql = string.Format("DELETE FROM MonHoc WHERE MaMon = '{0}'", maMH);
            return ExecuteNonQuery(sql) > 0;
        }

        public DataTable TimKiem(string keyword)
        {
            // SỬA Ở ĐÂY: MaMH -> MaMon, TenMH -> TenMon
            string sql = string.Format("SELECT * FROM MonHoc WHERE MaMon LIKE '%{0}%' OR TenMon LIKE N'%{0}%'", keyword);
            return GetDataTable(sql);
        }
    }
}