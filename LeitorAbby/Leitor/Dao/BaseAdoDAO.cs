using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Leitor.Dao
{
    public class BaseAdoDAO
    {
        public SqlConnection _conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public BaseAdoDAO()
        {
        }
        
        public void Close()
        {
            if (_conn.State == ConnectionState.Open)
                _conn.Close();
        }

    }
}
