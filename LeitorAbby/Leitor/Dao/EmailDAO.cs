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
                                              ,[EMA_USESSL]
                                              ,[EMA_DOMAIN]
                                              ,[EMA_PROVIDER]
                                              ,[EMA_DATA_RECEBIMENTO]
                                              ,[EMA_URLCONSUMO]
                                          FROM EMAIL WHERE EMA_ATIVO = 1";

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
                                        UseSSL = dataReader["EMA_USESSL"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["EMA_USESSL"]),
                                        Domain = dataReader["EMA_DOMAIN"] == DBNull.Value ? null : (string)dataReader["EMA_DOMAIN"],
                                        Provider = (string)dataReader["EMA_PROVIDER"],
                                        ConsumoServicoURL = dataReader["EMA_URLCONSUMO"].ToString()
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

        public EmailInfo GetCaixasPostaisPorId(int id)
        {
            EmailInfo returnData = null;

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
                                              ,[EMA_USESSL]
                                              ,[EMA_DOMAIN]
                                              ,[EMA_PROVIDER]
                                              ,[EMA_DATA_RECEBIMENTO]
                                              ,[EMA_URLCONSUMO]
                                          FROM EMAIL WHERE EMA_ID = " + id.ToString();

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                EmailInfo e = new EmailInfo
                                    {
                                        Id = (int)dataReader["EMA_ID"],
                                        Url = (string)dataReader["EMA_URL"],
                                        EmailAddress = (string)dataReader["EMA_EMAIL"],
                                        Password = (string)dataReader["EMA_SENHA"],
                                        UseSSL = dataReader["EMA_USESSL"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["EMA_USESSL"]),
                                        Domain = dataReader["EMA_DOMAIN"] == DBNull.Value ? null : (string)dataReader["EMA_DOMAIN"],
                                        Provider = (string)dataReader["EMA_PROVIDER"],
                                        ConsumoServicoURL = dataReader["EMA_URLCONSUMO"].ToString()
                                    };

                                if (dataReader["EMA_DATA_RECEBIMENTO"] != DBNull.Value)
                                    e.LastReceipt = Convert.ToDateTime(dataReader["EMA_DATA_RECEBIMENTO"]);

                                returnData =  e;
                            }
                            dataReader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("EmailDAO.GetCaixasPostaisPorId", e.Message, Log.LogType.Erro);
            }

            return returnData;
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
