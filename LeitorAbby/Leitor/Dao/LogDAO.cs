using System;
using System.Configuration;
using System.IO;
using Leitor.Utilities;
using System.Diagnostics;

namespace Leitor.Dao
{
    public class LogDAO : BaseAdoDAO
    {
        public static string LogPath
        {
            get
            {
                return (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogPath"])
                           ? ConfigurationManager.AppSettings["LogPath"]
                           : @"C:\ServicoLeitor\Log\");
            }
        }
        private static readonly string LogFile = LogPath + "Log" + DateTime.Today.ToString("yyyyMMdd") + "{0}.txt";
        private const string LogMessage = "{0}\t|{1}\t|{2};";

        public bool InserirLog(string mensagem, string remetente, string assunto, string corpo)
        {
            int result = -1;

            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"INSERT INTO LOG (LOG_DATA, LOG_METODO, LOG_AVISO, LOG_REMETENTE, LOG_ASSUNTO, LOG_CORPO)
                                        VALUES (@LOG_DATA, @LOG_METODO, @LOG_AVISO, @LOG_REMETENTE, @LOG_ASSUNTO, @LOG_CORPO)";

                    cmd.Parameters.AddWithValue("@LOG_DATA", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LOG_METODO", new StackTrace().GetFrame(1).GetMethod().Name);
                    cmd.Parameters.AddWithValue("@LOG_AVISO", mensagem);
                    cmd.Parameters.AddWithValue("@LOG_REMETENTE", remetente );
                    cmd.Parameters.AddWithValue("@LOG_ASSUNTO", assunto);
                    cmd.Parameters.AddWithValue("@LOG_CORPO", corpo);

                    cmd.Connection = _conn;
                    cmd.Connection.Open();

                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Log.SaveTxt("LogDAO.InserirLog", e.Message, Log.LogType.Erro);
            }
            finally
            {
                _conn.Close();
            }

            return result>0;
        }

        public static void SalvarLog(String classe, String mensagem, int remetenteId)
        {
            if (!File.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            if(!File.Exists(LogFile))
            {
                using (StreamWriter sw = File.CreateText(LogFile))
                {
                    sw.WriteLine(LogMessage, DateTime.Now, classe, mensagem);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(LogFile))
                {
                    sw.WriteLine(LogMessage, DateTime.Now, classe, mensagem);
                }
            }
        }

        public static object Locker = new object();

        public static void SalvarLog(String classe, String mensagem, Log.LogType tipo)
        {
            String logFile = String.Format(LogFile, tipo);
            if (!File.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            if (!File.Exists(logFile))
            {
                using (StreamWriter sw = File.CreateText(logFile))
                {
                    sw.WriteLine(LogMessage, DateTime.Now, classe, mensagem);
                }
            }
            else
            {
                lock (Locker)
                {
                    using (StreamWriter sw = File.AppendText(logFile))
                    {
                        sw.WriteLine(LogMessage, DateTime.Now, classe, mensagem);
                    }
                }
            }
        }
    }
}
