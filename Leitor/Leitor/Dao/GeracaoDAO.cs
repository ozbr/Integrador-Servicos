using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Dao
{
    public class GeracaoDAO : BaseAdoDAO
    {
        public int Atualizar(int idGeracao,NF nota, Remetente r, String corpo, String local)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarXML]";

                    cmd.Parameters.AddWithValue("@GER_ID", idGeracao);
                    cmd.Parameters.AddWithValue("@REM_ID", r.Id);
                    cmd.Parameters.AddWithValue("@GER_STATUS", 0);
                    cmd.Parameters.AddWithValue("@GER_serie", nota.infNFe.ide.serie);
                    cmd.Parameters.AddWithValue("@GER_nNF", nota.infNFe.ide.nNF);
                    cmd.Parameters.AddWithValue("@GER_dEmi", nota.infNFe.ide.dEmi);
                    cmd.Parameters.AddWithValue("@GER_NumeroRpsSubstituido", nota.infNFe.ide.NumeroRps);
                    cmd.Parameters.AddWithValue("@GER_EMI_Cnpj",  nota.infNFe.emit.CNPJ);
                    cmd.Parameters.AddWithValue("@GER_EMI_xNome", nota.infNFe.emit.xNome);
                    cmd.Parameters.AddWithValue("@GER_EMI_xFant", nota.infNFe.emit.xFant);
                    cmd.Parameters.AddWithValue("@GER_DES_Cnpj", nota.infNFe.dest.CNPJ);
                    cmd.Parameters.AddWithValue("@GER_DES_xNome", nota.infNFe.dest.xNome);
                    cmd.Parameters.AddWithValue("@GER_DES_xFant", "");
                    cmd.Parameters.AddWithValue("@GER_XMLGERADO", corpo);
                    cmd.Parameters.AddWithValue("@GER_XMLLOCAL", local);

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

        public int Inserir(NF nota, Remetente r, String corpo, String local)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirXML]";

                    cmd.Parameters.AddWithValue("@REM_ID", r.Id);
                    cmd.Parameters.AddWithValue("@GER_STATUS", 0);
                    cmd.Parameters.AddWithValue("@GER_serie", nota.infNFe.ide.serie);
                    cmd.Parameters.AddWithValue("@GER_nNF", nota.infNFe.ide.nNF);
                    cmd.Parameters.AddWithValue("@GER_dEmi", nota.infNFe.ide.dEmi);
                    cmd.Parameters.AddWithValue("@GER_NumeroRpsSubstituido", nota.infNFe.ide.NumeroRps);
                    cmd.Parameters.AddWithValue("@GER_EMI_Cnpj", nota.infNFe.emit.CNPJ);
                    cmd.Parameters.AddWithValue("@GER_EMI_xNome", nota.infNFe.emit.xNome);
                    cmd.Parameters.AddWithValue("@GER_EMI_xFant", nota.infNFe.emit.xFant);
                    cmd.Parameters.AddWithValue("@GER_DES_Cnpj", nota.infNFe.dest.CNPJ);
                    cmd.Parameters.AddWithValue("@GER_DES_xNome", nota.infNFe.dest.xNome);
                    cmd.Parameters.AddWithValue("@GER_DES_xFant", "");
                    cmd.Parameters.AddWithValue("@GER_XMLGERADO", corpo);
                    cmd.Parameters.AddWithValue("@GER_XMLLOCAL", local);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = (int)cmd.ExecuteScalar();

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
