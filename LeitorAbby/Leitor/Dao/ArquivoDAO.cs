using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor.Utilities;
using Leitor.Model;

namespace Leitor.Dao
{
    public class ArquivoDAO : BaseAdoDAO
    {

        public Anexo SelecionarArquivosControleOCR(string controleOCR)
        {

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT ARQ_ID,EDA_ID, ARQ_NOME, ARQ_LOCAL, ARQ_OCRCONTROLE FROM [ARQUIVO] WHERE ARQ_OCRCONTROLE = '" + controleOCR + "'";

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows && dr.Read())
                    {
                        return new Anexo
                        {
                            Id = (int)dr["ARQ_ID"],
                            EmailDataId = (int)dr["EDA_ID"],
                            CaminhoArquivo = (string)dr["ARQ_LOCAL"],
                            NomeArquivo = (string)dr["ARQ_NOME"],
                            ControleOCR = (string)dr["ARQ_OCRCONTROLE"]
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("ArquivoDAO.SelecionarArquivosNaoLidos", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return null;
        }


        public Anexo AtualizaAnexoOCRProcessado(Anexo anexo)
        {

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    //Atualiza o anexo como processado no OCR
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"UPDATE [ARQUIVO] SET ARQ_LOCAL_ORIGINAL_OCR = ARQ_LOCAL, ARQ_LOCAL = @ARQ_LOCAL, ARQ_OCRPROCESSADO = 1 WHERE ARQ_ID = @ARQ_ID";


                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    
                    cmd.Parameters.AddWithValue("@ARQ_ID",anexo.Id);
                    cmd.Parameters.AddWithValue("@ARQ_LOCAL", anexo.CaminhoArquivo);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        //Efetua a liberação do email para o processamento quando não mais houver anexos pendentes no OCR
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = "UPDATE EMAIL_DATA SET EDA_STATUS = 1 WHERE EDA_ID = @EDA_ID AND NOT EXISTS(" +
                            "SELECT 1 FROM ARQUIVO A WHERE A.EDA_ID = EMAIL_DATA.EDA_ID AND ARQ_OCRCONTROLE IS NOT NULL AND ISNULL(ARQ_OCRPROCESSADO,0) = 0)";

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EDA_ID", anexo.EmailDataId);

                        result = cmd.ExecuteNonQuery();
                    }
                                        
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("ArquivoDAO.AtualizaAnexoOCRProcessado", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return null;

        }
        //public int InserirArquivo(int remetenteId, String nomeArquivo, String local, String status)
        //{
        //    int result = 0;

        //    try
        //    {
        //        using (var cmd = _conn.CreateCommand())
        //        {
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.CommandText = "[InserirArquivo]";

        //            cmd.Parameters.AddWithValue("@REM_ID", remetenteId);
        //            cmd.Parameters.AddWithValue("@ARQ_NOME", nomeArquivo);
        //            cmd.Parameters.AddWithValue("@ARQ_LOCAL", local);
        //            cmd.Parameters.AddWithValue("@ARQ_STATUS", status);

        //            cmd.Connection = _conn;
        //            cmd.Connection.Open();

        //            result = cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Log.SaveTxt("ArquivoDAO.InserirArquivo", e.Message, Log.LogType.Erro);
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //    return result;
        //}

        ///*
        // AtualizarStatusArquivo
        // */

        //public bool AtualizarStatusArquivo(int remetenteId, String local, String status)
        //{
        //    int result = 0;

        //    try
        //    {
        //        using (var cmd = _conn.CreateCommand())
        //        {
        //            cmd.CommandType = System.Data.CommandType.Text;
        //            cmd.CommandText = "UPDATE [dbo].[ARQUIVO] SET [ARQ_STATUS] = '"+status+"', [ARQ_DATAEDICAO] = GETDATE() WHERE [REM_ID] = "+remetenteId+" AND [ARQ_LOCAL] = '"+local+"'";

        //            cmd.Parameters.AddWithValue("@REM_ID", remetenteId);
        //            cmd.Parameters.AddWithValue("@ARQ_LOCAL", local);
        //            cmd.Parameters.AddWithValue("@ARQ_STATUS", status);

        //            cmd.Connection = _conn;
        //            cmd.Connection.Open();

        //            result = cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.SaveTxt("ArquivoDAO.AtualizarStatusArquivo", e.Message, Log.LogType.Erro);
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //    return result > 0;
        //}

        //public bool VerificarArquivoGerado(String nome)
        //{
        //    int result = 0;

        //    try
        //    {
        //        using (var cmd = _conn.CreateCommand())
        //        {
        //            cmd.CommandType = System.Data.CommandType.Text;
        //            cmd.CommandText = "SELECT COUNT(*) FROM ARQUIVO WHERE ARQ_NOME ='" + nome + "'";

        //            //cmd.Parameters.AddWithValue("@ARQ_NOME", nome);

        //            cmd.Connection = _conn;
        //            cmd.Connection.Open();

        //            result = (int)cmd.ExecuteScalar();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.SaveTxt("ArquivoDAO.VerificarArquivoGerado", e.Message, Log.LogType.Erro);
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //    return result > 0;
        //}

        //public Dictionary<String, int> SelecionarArquivosNaoLidos()
        //{
        //    Dictionary<String, int> result = new Dictionary<String, int>();

        //    try
        //    {
        //        using (var cmd = _conn.CreateCommand())
        //        {
        //            cmd.CommandType = System.Data.CommandType.Text;
        //            cmd.CommandText = "SELECT ARQ_LOCAL, REM_ID FROM [DOT_LEITOR].[dbo].[ARQUIVO] WHERE ARQ_STATUS = ''";

        //            cmd.Connection = _conn;
        //            cmd.Connection.Open();

        //            SqlDataReader dr = cmd.ExecuteReader();
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    result.Add(dr["ARQ_LOCAL"] as string, (int)dr["REM_ID"]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.SaveTxt("ArquivoDAO.SelecionarArquivosNaoLidos", e.Message, Log.LogType.Erro);
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //    return result;
        //}

        //public Dictionary<String, int> SelecionarArquivosNaoLidos(int remetenteId)
        //{
        //    Dictionary<String, int> result = new Dictionary<String, int>();

        //    try
        //    {
        //        using (var cmd = _conn.CreateCommand())
        //        {
        //            cmd.CommandType = System.Data.CommandType.Text;
        //            cmd.CommandText = "SELECT * FROM [DOT_LEITOR].[dbo].[ARQUIVO] WHERE ARQ_STATUS = '' AND REM_ID=" + remetenteId;

        //            cmd.Connection = _conn;
        //            cmd.Connection.Open();

        //            SqlDataReader dr = cmd.ExecuteReader();
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    result.Add(dr["ARQ_LOCAL"] as string, (int)dr["REM_ID"]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.SaveTxt("ArquivoDAO.SelecionarArquivosNaoLidos", e.Message, Log.LogType.Erro);
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //    return result;
        //}

    }
}
