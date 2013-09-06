using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Dao
{
    public class ArquivoDAO : BaseAdoDAO
    {
        public int InserirArquivo(int remetenteId, String nomeArquivo, String local, String status)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirArquivo]";

                    cmd.Parameters.AddWithValue("@REM_ID", remetenteId);
                    cmd.Parameters.AddWithValue("@ARQ_NOME", nomeArquivo);
                    cmd.Parameters.AddWithValue("@ARQ_LOCAL", local);
                    cmd.Parameters.AddWithValue("@ARQ_STATUS", status);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

        /*
         AtualizarStatusArquivo
         */

        public bool AtualizarStatusArquivo(int remetenteId, String local, String status)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "UPDATE [dbo].[ARQUIVO] SET [ARQ_STATUS] = '"+status+"', [ARQ_DATAEDICAO] = GETDATE() WHERE [REM_ID] = "+remetenteId+" AND [ARQ_LOCAL] = '"+local+"'";

                    cmd.Parameters.AddWithValue("@REM_ID", remetenteId);
                    cmd.Parameters.AddWithValue("@ARQ_LOCAL", local);
                    cmd.Parameters.AddWithValue("@ARQ_STATUS", status);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                _conn.Close();
            }

            return result > 0;
        }

        public bool VerificarArquivoGerado(String nome)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) FROM ARQUIVO WHERE ARQ_NOME ='" + nome + "'";

                    //cmd.Parameters.AddWithValue("@ARQ_NOME", nome);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = (int)cmd.ExecuteScalar();
                }
            }
            finally
            {
                _conn.Close();
            }

            return result > 0;
        }

        public Dictionary<String, int> SelecionarArquivosNaoLidos()
        {
            Dictionary<String, int> result = new Dictionary<String, int>();

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT ARQ_LOCAL, REM_ID FROM [DOT_LEITOR].[dbo].[ARQUIVO] WHERE ARQ_STATUS = ''";

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result.Add(dr["ARQ_LOCAL"] as string, (int)dr["REM_ID"]);
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

        public Dictionary<String, int> SelecionarArquivosNaoLidos(int remetenteId)
        {
            Dictionary<String, int> result = new Dictionary<String, int>();

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT * FROM [DOT_LEITOR].[dbo].[ARQUIVO] WHERE ARQ_STATUS = '' AND REM_ID=" + remetenteId;

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result.Add(dr["ARQ_LOCAL"] as string, (int)dr["REM_ID"]);
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

    }
}
