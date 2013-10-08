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
        public SqlConnection _conn = new SqlConnection(@"Data Source=neutron-sql.dotinsight.corp;Initial Catalog=DOT_LEITOR;Integrated Security=False;Persist Security Info=False;User ID=dot11;Password=@senna11@;Connect Timeout=120");
        
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
