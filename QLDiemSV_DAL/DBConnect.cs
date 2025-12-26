using System;
using System.Data;
using System.Data.SqlClient;

namespace QLDiemSV_DAL
{
    public class DBConnect
    {
        // 1. Chuỗi kết nối (Connection String)
        protected string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyDiemSV;Integrated Security=True;TrustServerCertificate=True";

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

        // 4. Hàm lấy dữ liệu (SELECT) - Trả về bảng dữ liệu (DataTable)
        // File: QLDiemSV_DAL/DBConnect.cs

        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(dt);
            }
            catch (Exception ex) // Thêm biến ex để bắt lỗi
            {
                dt = null;
                // --- THÊM DÒNG NÀY ĐỂ XEM LỖI LÀ GÌ ---
                System.Windows.Forms.MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        // 5. Hàm thực thi lệnh (INSERT, UPDATE, DELETE) - Trả về số dòng bị ảnh hưởng
        public int ExecuteNonQuery(string sql)
        {
            int result = 0;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
            }
            catch
            {
                result = 0;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        // 6. Hàm kiểm tra giá trị đơn (ví dụ: đếm số dòng, lấy 1 tên)
        public object ExecuteScalar(string sql)
        {
            object result = null;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteScalar();
            }
            catch
            {
                result = null;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }
    }
}