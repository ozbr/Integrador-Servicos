using System.Data;
using Leitor.Email;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Leitor.Utilities;
using System.Configuration;

namespace Leitor.Dao
{
    public class EmailDAO : BaseAdoDAO
    {
        private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public List<EmailInfo> GetCaixasPostais()
        {
            List<EmailInfo> result = new List<EmailInfo>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = @"
                                        SELECT [EMA_ID]
                                              ,[EMA_URL]
                                              ,[EMA_EMAIL]
                                              ,[EMA_SENHA]
                                              ,[EMA_DOMAIN]
                                              ,[EMA_PROVIDER]
                                              ,[EMA_DATA_RECEBIMENTO]
                                          FROM EMAIL";

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                EmailInfo e = new EmailInfo
                                    {
                                        Id = (int)dataReader["EMA_ID"],
                                        Url = (string)dataReader["EMA_URL"],
                                        EmailAddress = (string)dataReader["EMA_EMAIL"],
                                        Password = (string)dataReader["EMA_SENHA"],
                                        Domain = dataReader["EMA_DOMAIN"] == DBNull.Value ? null : (string)dataReader["EMA_DOMAIN"],
                                        Provider = (string)dataReader["EMA_PROVIDER"]
                                    };

                                if (dataReader["EMA_DATA_RECEBIMENTO"] != DBNull.Value)
                                    e.LastReceipt = Convert.ToDateTime(dataReader["EMA_DATA_RECEBIMENTO"]);

                                result.Add(e);
                            }
                            dataReader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch(Exception e)
            {
                Log.SaveTxt("EmailDAO.SelecionarCaixasEmail", e.Message, Log.LogType.Erro);
            }

            return result;
        }

        public void AtualizarUltimaVerificacao(EmailInfo ei)
        {
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarUltimaVerificacao]";
                    cmd.Parameters.AddWithValue("@ID", ei.Id);
                    cmd.Parameters.AddWithValue("@TIME", ei.LastReceipt);
                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDAO.AtualizarUltimaVerificacao", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
        }

    }
}
