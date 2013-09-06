using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor.Model;

namespace Leitor.Dao
{
    public class RegexesDAO : BaseAdoDAO
    {
        public String GetRemetenteLinkRegex(String remetente)
        {
            String result = String.Empty;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "[GetRemetenteLinkRegex]";
                    cmd.Parameters.AddWithValue("@REM_EMAIL", remetente);

                    cmd.Connection = _conn;
                    result = (String)cmd.ExecuteScalar();
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

        public RegexModel SelecionarRegexPorRemetenteId(int remetenteId)
        {
            RegexModel result = new RegexModel();
            DataTable tb = null;
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[SelecionarRegexPorRemetenteId]";
                    cmd.Parameters.AddWithValue("@REM_ID", remetenteId);
                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    using (SqlDataReader myReader = cmd.ExecuteReader())
                    {
                        tb = new DataTable();
                        tb.Load(myReader);
                    }
                    //troquei, adicionando 'dinamicamente' no fim do método com base nas colunas da table
                    //SqlDataReader dr = cmd.ExecuteReader();
                    //if(dr.HasRows)
                    //{
                    //    while (dr.Read())
                    //    {
                    //        result.Geral = dr["RGX_GERAL"] as string;
                    //        result.Id = (int) dr["RGX_ID"];
                    //        result.Item = dr["RGX_ITEM"] as string;
                    //        if (dr["RGX_ide_cUf"] != DBNull.Value)
                    //        {
                    //            result.Groups.Add("RGX_ide_cUf", (int) dr["RGX_ide_cUf"]);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _conn.Close();
            }

            if (tb != null && tb.Rows.Count > 0)
            {
                try
                {
                    result.Id = remetenteId;
                    result.Geral = (String)tb.Rows[0]["RGX_GERAL"];
                    result.Item = (String)tb.Rows[0]["RGX_ITEM"];
                    result.IsXpath = (tb.Rows[0]["RGX_ISXPATH"] != DBNull.Value ? (bool)tb.Rows[0]["RGX_ISXPATH"] : false);

                    foreach (DataColumn dc in tb.Columns)
                    {
                        if (dc.ColumnName != "RGX_GERAL" && dc.ColumnName != "RGX_ITEM" && dc.ColumnName != "RGX_ISXPATH")
                        {
                            if (result.IsXpath)
                            {
                                result.AddXPath(dc.ColumnName, (tb.Rows[0][dc.ColumnName] == DBNull.Value ? string.Empty : tb.Rows[0][dc.ColumnName]as string));
                            }
                            else
                            {
                                result.AddRegex(dc.ColumnName,
                                                (tb.Rows[0][dc.ColumnName] == DBNull.Value
                                                     ? 0
                                                     : Convert.ToInt32(tb.Rows[0][dc.ColumnName])));
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return result;
        }
    }
}
