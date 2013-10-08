using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Dao
{
    public class LogDAO : BaseAdoDAO
    {

        public bool InserirLog(String classe, String mensagem, int remetenteId)
        {

            int result = -1;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirLog]";

                    cmd.Parameters.AddWithValue("@LOG_CLASSE", classe);
                    cmd.Parameters.AddWithValue("@LOG_MENSAGEM", mensagem);
                    cmd.Parameters.AddWithValue("@REM_ID", remetenteId);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _conn.Close();
            }

            return result>0;
        }
    }
}
