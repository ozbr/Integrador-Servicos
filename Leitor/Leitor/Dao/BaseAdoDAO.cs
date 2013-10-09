using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Dao
{
    public class BaseAdoDAO
    {
        public SqlConnection _conn = new SqlConnection(@"Data Source=localhost;Initial Catalog=DOT_LEITOR;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=210681;Connect Timeout=120");
        
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
