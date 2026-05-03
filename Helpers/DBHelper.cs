using System.Data;
using System.Data.SqlClient;

namespace DisasterMIS_Frontend.Helpers
{
    public class DBHelper
    {
        // Hardcoded connection string – change if your SQL Server is different
        private static string connectionString = @"Server=DESKTOP-FUGQNF4\SQLEXPRESS;Database=DisasterMIS;Trusted_Connection=True;TrustServerCertificate=True;";

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    DataTable dt = new DataTable();
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        // Helper for ResourceController that needs connection string directly
        public static string GetConnectionString()
        {
            return connectionString;
        }
    }
}