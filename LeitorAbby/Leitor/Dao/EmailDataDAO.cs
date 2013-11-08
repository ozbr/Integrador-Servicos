using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor.Utilities;
using System.IO;

namespace Leitor.Dao
{
    public class EmailDataDAO : BaseAdoDAO
    {
        public bool SalvarEmailData(EmailData email, Leitor.Helper.FlowStatus initialStatus)
        {
            bool success = false;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[SalvarEmailData]";

                    cmd.Parameters.AddWithValue("@PRE_NOME", email.Prefeitura);
                    cmd.Parameters.AddWithValue("@EDA_DATA", email.Data);
                    cmd.Parameters.AddWithValue("@EDA_ASSUNTO", email.Assunto);
                    cmd.Parameters.AddWithValue("@EDA_LOCAL_LOTE", email.CaminhoLote);
                    cmd.Parameters.AddWithValue("@EDA_STATUS", (int)initialStatus);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    int eda_id = (int)cmd.ExecuteScalar();

                    StringBuilder sb = new StringBuilder();

                    sb.Append(@"    DECLARE @EDA_ID int = {0}
                                    DECLARE @ARQ_NOME nvarchar(MAX)
                                    DECLARE @ARQ_OCRCONTROLE varchar(255)
                                    DECLARE @ARQ_LOCAL nvarchar(MAX)");

                    for (int i = 0; i < email.Anexos.Count; i++)
                    {
                        string controleOCR = string.IsNullOrEmpty(email.Anexos[i].ControleOCR) ? "NULL" : "'" + email.Anexos[i].ControleOCR + "'";
                        sb.Append(@"
                                      SET @ARQ_NOME = '").Append(email.Anexos[i].NomeArquivo).Append(@"'
                                      SET @ARQ_LOCAL = '").Append(email.Anexos[i].CaminhoArquivo).Append(@"'
                                      SET @ARQ_OCRCONTROLE = ").Append(controleOCR).Append(@"
                                      INSERT INTO ARQUIVO (EDA_ID, ARQ_NOME, ARQ_LOCAL, ARQ_DATACRIACAO, ARQ_OCRCONTROLE)
                                      VALUES (@EDA_ID, @ARQ_NOME, @ARQ_LOCAL, GETDATE(), @ARQ_OCRCONTROLE)");
                    }

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = string.Format(sb.ToString(), eda_id);
                    cmd.Parameters.Clear();

                    cmd.ExecuteScalar();

                    cmd.CommandText = @"UPDATE EMAIL SET EMA_DATA_RECEBIMENTO = @EMA_DATA_RECEBIMENTO WHERE EMA_ID = @EMA_ID";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EMA_DATA_RECEBIMENTO", email.Data);
                    cmd.Parameters.AddWithValue("@EMA_ID", email.IdEnderecoEmail);

                    cmd.ExecuteNonQuery();

                    success = true;
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.InserirEmailData", e.Message, Log.LogType.Erro);
                success = false;
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    _conn.Close();
                }
            }

            return success;
        }

        public bool AtualizarLocalLote(EmailData email)
        {
            bool success = false;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "UPDATE EMAIL_DATA SET EDA_LOCAL_LOTE = @EDA_LOCAL_LOTE, EDA_STATUS = 4 WHERE EDA_ID = @EDA_ID";

                    cmd.Parameters.AddWithValue("@EDA_ID", email.Id);
                    cmd.Parameters.AddWithValue("@EDA_LOCAL_LOTE", email.CaminhoLote);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    success = true;
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.InserirEmailData", e.Message, Log.LogType.Erro);
                success = false;
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    _conn.Close();
                }
            }

            return success;
        }

        public List<object[]> SelectEmailData()
        {
            List<object[]> infoData = new List<object[]>();

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"
	                        SELECT EDA.EDA_ID, EDA.EDA_ASSUNTO, EDA.EDA_LOCAL_LOTE, ARQ.ARQ_LOCAL, ARQ.ARQ_NOME, PRE.PRE_ID, PRE.PRE_NOME, PRE.PRE_RGXLINK, PRE.PRE_RGXLINK_SECUNDARIO, PRE.PRE_RGXLINK_FORMAT, ARQ.ARQ_LOCAL_ORIGINAL_OCR
	                        INTO #PROCESSAMENTO
							FROM EMAIL_DATA EDA
	                        INNER JOIN PREFEITURA PRE ON PRE.PRE_ID = EDA.PRE_ID
	                        INNER JOIN ARQUIVO ARQ ON ARQ.EDA_ID = EDA.EDA_ID
	                        WHERE EDA.EDA_ID IN (SELECT DISTINCT TOP 30 EDA_ID FROM EMAIL_DATA EDA WHERE EDA.PRE_ID IS NOT NULL AND EDA.EDA_STATUS = 1)
	                        ORDER BY EDA_ID;
							 
							UPDATE EMAIL_DATA 
                            SET EDA_STATUS = 2 
                            WHERE EDA_ID IN (SELECT EDA_ID FROM #PROCESSAMENTO);

							SELECT * FROM #PROCESSAMENTO ORDER BY EDA_ID";

                    cmd.Connection.Open();

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        EmailData email = null;
                        int EDA_ID = 0;

                        while (dataReader.Read())
                        {
                            if (email == null || EDA_ID != (int)dataReader["EDA_ID"])
                            {
                                EDA_ID = (int)dataReader["EDA_ID"];

                                email = new EmailData();
                                email.Assunto = dataReader["EDA_ASSUNTO"] == DBNull.Value ? null : (string)dataReader["EDA_ASSUNTO"];
                                email.Prefeitura = dataReader["PRE_NOME"] == DBNull.Value ? null : (string)dataReader["PRE_NOME"];
                                email.Id = (int)dataReader["EDA_ID"];
                                email.CaminhoLote = (string)dataReader["EDA_LOCAL_LOTE"];
                                email.Anexos.Add(new Anexo() { NomeArquivo = (string)dataReader["ARQ_NOME"], CaminhoArquivo = (string)dataReader["ARQ_LOCAL"], CaminhoOriginalOCR = (string)dataReader["ARQ_LOCAL_ORIGINAL_OCR"] });

                                Prefeitura prefeitura = new Prefeitura();

                                prefeitura.Id = (int)dataReader["PRE_ID"];
                                prefeitura.Nome = (string)dataReader["PRE_NOME"];
                                prefeitura.RgxLinkFormat = dataReader["PRE_RGXLINK_FORMAT"] == DBNull.Value ? null : (string)dataReader["PRE_RGXLINK_FORMAT"];
                                prefeitura.RgxLink = dataReader["PRE_RGXLINK"] == DBNull.Value ? null : (string)dataReader["PRE_RGXLINK"];
                                prefeitura.RgxLinkSecundario = dataReader["PRE_RGXLINK_SECUNDARIO"] == DBNull.Value ? null : (string)dataReader["PRE_RGXLINK_SECUNDARIO"];

                                infoData.Add(new object[] { email, prefeitura });
                            }
                            else
                            {
                                email.Anexos.Add(new Anexo() { NomeArquivo = (string)dataReader["ARQ_NOME"], CaminhoArquivo = (string)dataReader["ARQ_LOCAL"] });
                            }

                        }
                        dataReader.Close();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                    _conn.Close();
            }

            return infoData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="prefeitura"></param>
        /// <param name="status"></param>
        /// <returns>O STATUS DO EMAILDATA EM QUESTÃO, 0 CASO PRECISE SER LIDO</returns>
        public int InserirEmailData(EmailData email, String prefeitura, int status)
        {
            int result = -1;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirEmailData]";

                    //cmd.Parameters.AddWithValue("@PRE_ID", nome);//OPCIONAL
                    cmd.Parameters.AddWithValue("@PRE_NOME", prefeitura);
                    cmd.Parameters.AddWithValue("@EDA_DATA", email.Data);
                    cmd.Parameters.AddWithValue("@EDA_ASSUNTO", email.Assunto);
                    cmd.Parameters.AddWithValue("@EDA_STATUS", status);//OPCIONAL DEFAULT 0

                    string attachments = string.Empty;

                    for (int i = 0; i < email.Anexos.Count; i++)
                    {
                        attachments += email.Anexos[i].NomeArquivo + ";";
                    }

                    cmd.Parameters.AddWithValue("@EDA_ANEXO", attachments);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.InserirEmailData", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

        public void AtualizarEnvio(EmailData email, String local, String resposta, int status)
        { 

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarEnvioEmailData]";

                    cmd.Parameters.AddWithValue("@EDA_ID", email.Id);
                    cmd.Parameters.AddWithValue("@EDA_LOCAL_LOTE", local);
                    cmd.Parameters.AddWithValue("@EDA_RESULTADO_ENVIO", resposta);
                    cmd.Parameters.AddWithValue("@EDA_STATUS", status);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.AtualizarEnvio", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
        }

        public void AtualizarEmailData(EmailData email, int status)
        {
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarEmailData]";

                    cmd.Parameters.AddWithValue("@EDA_ID", email.Id);
                    cmd.Parameters.AddWithValue("@EDA_STATUS", status);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.AtualizarEmailData", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns>0 = Enviar; -1 = Já foi enviado; 1 = Aguardar</returns>
        public int VerificaEnvioAndSinaliza(EmailData email)
        {
            int result = 0;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[VerificaEnvio]";

                    cmd.Parameters.AddWithValue("@EDA_ID", email.Id);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = (int) cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDataDAO.VerificaEnvio", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }
    }
}
