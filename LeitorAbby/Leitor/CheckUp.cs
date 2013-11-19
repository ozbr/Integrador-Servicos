using System.IO;
using Leitor.Dao;
using System;
using Leitor.Utilities;
using System.Threading;
using Leitor.Tools;
using System.Threading.Tasks;
using Leitor.Email;
using System.Collections.Generic;
using System.Diagnostics;
using Leitor.Document;
using System.Data.SqlClient;
using Leitor.Core;
using Leitor.Model;

namespace Leitor
{
    public static class CheckUp
    {
        public static bool Start()
        {
            bool success = false;

            try
            {
                Log.SaveTxt("CheckUp.Start", "Entrando", Log.LogType.Debug);

                using (SqlConnection connection = new SqlConnection(Repository._connectionString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText += @"UPDATE EMAIL_DATA SET EDA_STATUS = 1 WHERE EDA_STATUS = 2 OR EDA_STATUS = 3; ";
                        command.CommandText += @"UPDATE EMAIL_DATA SET EDA_STATUS = 4 WHERE EDA_STATUS = 5 OR EDA_STATUS = 6; ";

                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();

                        success = true;
                    }
                }

                Log.SaveTxt("CheckUp.Start", "Resultado " + success.ToString(), Log.LogType.Debug);
            }
            catch (Exception e)
            {
                Log.SaveTxt("Start", e.Message, Log.LogType.Erro);
                Console.WriteLine("Impossível iniciar.");
            }

            return success;
        }
    }
}
