using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor.Utilities;

namespace Leitor.Dao
{
    public class PrefeituraDAO : BaseAdoDAO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nome"></param>
        /// <returns>-1 caso já exista, caso contrário retorna o ID</returns>
        public int InserirPrefeitura(String nome)
        {
            int result = 0;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirPrefeitura]";

                    cmd.Parameters.AddWithValue("@PRE_NOME", nome);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.InserirPrefeitura", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

        public Prefeitura SelecionarPrefeitura(String nome)
        {
            Prefeitura result = null;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarPrefeitura]";

                    cmd.Parameters.AddWithValue("@PRE_NOME", nome);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = new Prefeitura();
                            //PRE.PRE_ID, PRE.PRE_NOME, REM_RGXLINK, REM.REM_RGXSECUNDARIO, REM.REM_PARAMETRO
                            result.Id = (int)dr["PRE_ID"];
                            result.Nome = (String)dr["PRE_NOME"];
                            result.RgxLink = (String)dr["REM_RGXLINK"];
                            result.RgxLinkSecundario = (String)dr["REM_RGXSECUNDARIO"];
                            result.RgxLinkFormat = (String)dr["REM_PARAMETRO"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.SelecionarPrefeitura", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public List<Prefeitura> SelecionarPrefeituras()
        {
            List<Prefeitura> result = new List<Prefeitura>();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarPrefeituras]";

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Prefeitura prefeitura = new Prefeitura();

                            //PRE.PRE_ID, PRE.PRE_NOME, REM_RGXLINK, REM.REM_RGXSECUNDARIO, REM.REM_PARAMETRO
                            prefeitura.Id = (int)dr["PRE_ID"];
                            prefeitura.Nome = (String)dr["PRE_NOME"];
                            prefeitura.RgxLink = (dr["PRE_RGXLINK"] == DBNull.Value) ? null : (String)dr["PRE_RGXLINK"];
                            prefeitura.RgxLinkSecundario = (dr["PRE_RGXLINK_SECUNDARIO"] == DBNull.Value) ? null : (String)dr["PRE_RGXLINK_SECUNDARIO"];
                            prefeitura.RgxLinkFormat = (dr["PRE_RGXLINK_FORMAT"] == DBNull.Value) ? null : (String)dr["PRE_RGXLINK_FORMAT"];

                            result.Add(prefeitura);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.SelecionarPrefeituras", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public List<Prefeitura> SelecionarPrefeituraPorEmail(String email)
        {
            List<Prefeitura> result = new List<Prefeitura>();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText =
                        @"SELECT PRE.PRE_ID, PRE.PRE_NOME, REM_RGXLINK, REM.REM_RGXSECUNDARIO, REM.REM_PARAMETRO FROM [PREFEITURA] AS PRE
                        INNER JOIN [REMETENTE] AS REM ON PRE.PRE_NOME = REM.REM_NOME
                        WHERE REM.REM_EMAILS = '" + email +"'";

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var pref = new Prefeitura();
                            //PRE.PRE_ID, PRE.PRE_NOME, REM_RGXLINK, REM.REM_RGXSECUNDARIO, REM.REM_PARAMETRO
                            pref.Id = (int)dr["PRE_ID"];
                            pref.Nome = (String)dr["PRE_NOME"];
                            pref.RgxLink = (String)dr["REM_RGXLINK"];
                            pref.RgxLinkSecundario = (String)dr["REM_RGXSECUNDARIO"];
                            pref.RgxLinkFormat = (String)dr["REM_PARAMETRO"];
                            result.Add(pref);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.SelecionarPrefeituraPorEmail", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        internal List<Prefeitura> SelecionarPrefeiturasPossiveis(string p)
        {
            String ctxt = @"SELECT * FROM REMETENTE 
                WHERE REM_EMAILS='mateus.rocha@dot-insight.net'
                AND REM_RGXLINK != ''";
            List<Prefeitura> result = new List<Prefeitura>();

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = ctxt;
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Prefeitura aux = new Prefeitura();

                            aux.Nome = (String)dr["REM_NOME"];
                            aux.RgxLink = (String)dr["REM_RGXLINK"];
                            aux.RgxLinkSecundario = (String)dr["REM_RGXSECUNDARIO"];
                            aux.RgxLinkFormat = (String)dr["REM_PARAMETRO"];

                            result.Add(aux);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.SelecionarPrefeiturasPossiveis", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return result;
        }

        public List<TpAnexo> SelecionarPrefeituraTpAnexos(string nome)
        {
            List<TpAnexo> result = new List<TpAnexo>();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarPrefeituraTpAnexo]";
                    
                    cmd.Parameters.AddWithValue("@PRE_NOME", nome);
                    
                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            TpAnexo tpAnexo = new TpAnexo();

                            //PRE.PRE_ID, PRE.PRE_NOME, REM_RGXLINK, REM.REM_RGXSECUNDARIO, REM.REM_PARAMETRO
                            tpAnexo.Id = (int)dr["PAN_ID"];
                            tpAnexo.Prefeitura = (String)dr["PRE_NOME"];
                            tpAnexo.Extensao = (String)dr["PAN_EXTENSAO"];
                            tpAnexo.UseOCR = Convert.ToBoolean(dr["PAN_USEOCR"]);
                            
                            result.Add(tpAnexo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("PrefeituraDAO.SelecionarPrefeituraTpAnexos", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }
    }
}
