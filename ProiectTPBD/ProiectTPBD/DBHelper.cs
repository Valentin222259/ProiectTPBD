using System;
using Oracle.ManagedDataAccess.Client;

namespace ProiectTPBD
{
    public class DBHelper
    {
        private static string connectionString =
            "User Id=student;Password=student;Data Source=localhost:1521/XE;";

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (OracleConnection con = GetConnection())
                {
                    con.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Eroare conexiune Oracle: " + ex.Message,
                    "Eroare",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
    }
}