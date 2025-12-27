using System;
using System.Data;
using System.Data.SqlClient;

namespace QLDiemSV_DAL
{
    public class DBConnect
    {
        // 1. Chuỗi kết nối (ĐÃ SỬA LỖI)
        // Bỏ đoạn "TrustServerCertificate=True" đi là sẽ hết lỗi Index 54
        protected string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyDiemSV;Integrated Security=True";

        protected SqlConnection conn = null;

        // 2. Hàm mở kết nối
        public void OpenConnection()
        {
            if (conn == null)
            {
                conn = new SqlConnection(strConnect);
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        // 3. Hàm đóng kết nối
        public void CloseConnection()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        // 4. Hàm lấy dữ liệu (SELECT) - CÓ PARAMETER (CHỐNG SQL INJECTION)
        // Đây là hàm nâng cấp quan trọng nhất
        public DataTable GetDataTable(string sql, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Nếu có tham số truyền vào thì add vào command
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                // Ném lỗi ra để GUI bắt và hiển thị (như trong ảnh bạn chụp)
                throw new Exception("Lỗi DB: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        // 5. Hàm thực thi lệnh (INSERT, UPDATE, DELETE) - CÓ PARAMETER
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            int result = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Nếu có tham số truyền vào thì add vào command
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi thực thi: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        // 6. Hàm lấy giá trị đơn (Scalar) - CÓ PARAMETER
        public object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            object result = null;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi Scalar: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }
    }
}