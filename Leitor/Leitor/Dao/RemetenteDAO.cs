using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Leitor.Model;

namespace Leitor.Dao
{
    internal class RemetenteDAO : BaseAdoDAO
    {
        public bool AtualizarNomePrefeitura(int id, String nome)
        {
            bool result = false;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarNomePrefeitura]";

                    cmd.Parameters.AddWithValue("@REM_ID", id);
                    cmd.Parameters.AddWithValue("@REM_NOME", nome);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public int InserirRemetente(Remetente remetente)
        {
            int id = 0;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[InserirRemetente]";

                    cmd.Parameters.AddWithValue("@Nome", remetente.Nome);
                    cmd.Parameters.AddWithValue("@Emails", remetente.Emails);
                    cmd.Parameters.AddWithValue("@Assuntos", remetente.Assuntos);
                    cmd.Parameters.AddWithValue("@RgxLink", remetente.RgxLink);
                    cmd.Parameters.AddWithValue("@RgxSecundario", remetente.RgxSecundario);
                    cmd.Parameters.AddWithValue("@Parametro", remetente.Parametro);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    id = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return id;
        }

        public bool AtualizarRemetente(Remetente remetente)
        {
            bool result = false;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[AtualizarRemetente]";

                    cmd.Parameters.AddWithValue("@Id", remetente.Id);
                    cmd.Parameters.AddWithValue("@Nome", remetente.Nome);
                    cmd.Parameters.AddWithValue("@Emails", remetente.Emails);
                    cmd.Parameters.AddWithValue("@Assuntos", remetente.Assuntos);
                    cmd.Parameters.AddWithValue("@RgxLink", remetente.RgxLink);
                    cmd.Parameters.AddWithValue("@RgxSecundario", remetente.RgxSecundario);
                    cmd.Parameters.AddWithValue("@Parametro", remetente.Parametro);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public List<Remetente> SelecionarRemetenteTodos()
        {
            List<Remetente> result = new List<Remetente>();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarRemetenteTodos]";
                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Remetente r = new Remetente();
                            r.Id = (int)dataReader["REM_ID"];
                            r.Nome = (dataReader["REM_NOME"] != DBNull.Value ? (String)dataReader["REM_NOME"] : String.Empty);
                            r.Emails = (dataReader["REM_EMAILS"] != DBNull.Value ? (String)dataReader["REM_EMAILS"] : String.Empty);
                            r.Assuntos = (dataReader["REM_ASSUNTOS"] != DBNull.Value ? (String)dataReader["REM_ASSUNTOS"] : String.Empty);
                            r.RgxLink = (dataReader["REM_RGXLINK"] != DBNull.Value ? (String)dataReader["REM_RGXLINK"] : String.Empty);
                            r.RgxSecundario = (dataReader["REM_RGXSECUNDARIO"] != DBNull.Value ? (String)dataReader["REM_RGXSECUNDARIO"] : String.Empty);
                            r.Parametro = (dataReader["REM_PARAMETRO"] != DBNull.Value ? (String)dataReader["REM_PARAMETRO"] : String.Empty);
                            r.ArquivoNoCorpo = (dataReader["REM_CORPOARQUIVO"] != DBNull.Value && (bool)dataReader["REM_CORPOARQUIVO"]);
                            result.Add(r);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public Remetente SelecionarRemetentePorEmail(string email)
        {
            Remetente result = null;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarRemetentePorEmail]";
                    cmd.Parameters.AddWithValue("@Emails", email);
                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result = new Remetente();
                            result.Id = (int)dataReader["REM_ID"];
                            result.Nome = (dataReader["REM_NOME"] != DBNull.Value ? (String)dataReader["REM_NOME"] : String.Empty);
                            result.Emails = (dataReader["REM_EMAILS"] != DBNull.Value ? (String)dataReader["REM_EMAILS"] : String.Empty);
                            result.Assuntos = (dataReader["REM_ASSUNTOS"] != DBNull.Value ? (String)dataReader["REM_ASSUNTOS"] : String.Empty);
                            result.RgxLink = (dataReader["REM_RGXLINK"] != DBNull.Value ? (String)dataReader["REM_RGXLINK"] : String.Empty);
                            result.RgxSecundario = (dataReader["REM_RGXSECUNDARIO"] != DBNull.Value ? (String)dataReader["REM_RGXSECUNDARIO"] : String.Empty);
                            result.Parametro = (dataReader["REM_PARAMETRO"] != DBNull.Value ? (String)dataReader["REM_PARAMETRO"] : String.Empty);
                            result.ArquivoNoCorpo = (dataReader["REM_CORPOARQUIVO"] != DBNull.Value && (bool)dataReader["REM_CORPOARQUIVO"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }

        public bool SalvarCorpoEmail(string email)
        {
            bool result = false;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[SalvarCorpoEmail]";
                    cmd.Parameters.AddWithValue("@Emails", email);
                    cmd.Connection = _conn;
                    cmd.Connection.Open();
                    if ((int)cmd.ExecuteScalar() > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _conn.Close();
            }
            return result;
        }
    }
}