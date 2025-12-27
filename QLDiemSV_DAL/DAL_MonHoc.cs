using System.Data;
using QLDiemSV_DTO;

namespace QLDiemSV_DAL
{
    public class DAL_MonHoc : DBConnect
    {
        public DataTable GetDS()
        {
            // Trong DB bảng MonHoc dùng MaMH và TenMH
            return GetDataTable("SELECT * FROM MonHoc");
        }

        public bool Them(DTO_MonHoc mh)
        {
            // Sửa MaMon -> MaMH, TenMon -> TenMH
            string sql = string.Format("INSERT INTO MonHoc(MaMH, TenMH, SoTinChi) VALUES ('{0}', N'{1}', {2})",
                                       mh.MaMH, mh.TenMH, mh.SoTinChi);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Sua(DTO_MonHoc mh)
        {
            // Sửa TenMon -> TenMH, MaMon -> MaMH
            string sql = string.Format("UPDATE MonHoc SET TenMH = N'{0}', SoTinChi = {1} WHERE MaMH = '{2}'",
                                       mh.TenMH, mh.SoTinChi, mh.MaMH);
            return ExecuteNonQuery(sql) > 0;
        }

        public bool Xoa(string maMH)
        {
            // Sửa MaMon -> MaMH
            string sql = string.Format("DELETE FROM MonHoc WHERE MaMH = '{0}'", maMH);
            return ExecuteNonQuery(sql) > 0;
        }

        public DataTable TimKiem(string keyword)
        {
            // Sửa MaMon -> MaMH, TenMon -> TenMH
            string sql = string.Format("SELECT * FROM MonHoc WHERE MaMH LIKE '%{0}%' OR TenMH LIKE N'%{0}%'", keyword);
            return GetDataTable(sql);
        }
    }
}